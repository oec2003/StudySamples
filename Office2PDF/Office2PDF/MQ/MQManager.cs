using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Office2PDF.Extension;

namespace Office2PDF.MQ
{
    public class MQManager
    {
        /// <summary>
        /// 持久化发送和订阅对象
        /// </summary>
        private static readonly ConcurrentDictionary<string, IModel> _publishModelDic =
           new ConcurrentDictionary<string, IModel>();
        private static readonly ConcurrentDictionary<string, IModel> _subscribeModelDic =
         new ConcurrentDictionary<string, IModel>();
        private static readonly ConcurrentDictionary<string, string> _exchangeQueue = new ConcurrentDictionary<string, string>();

        private static IConnection _connection;
        private static readonly object LockObj = new object();

        private static RabbitMqAttribute _rabbitMqAttribute;
        private const string RabbitMqAttribute = "RabbitMqAttribute";

        private bool _connected;
        public bool Connected => this._connected;

        public MQManager(MqConfig config)
        {
            Open(config);
            if (_connection != null)
            {
                this._connected = true;
            }
        }

        private static void Open(MqConfig config)
        {
            try
            {

                if (_connection != null) return;
                lock (LockObj)
                {
                    int mqport = 5672;
                    if (!string.IsNullOrEmpty(config.Port))
                    {
                        int.TryParse(config.Port, out mqport);
                    }
                    var factory = new ConnectionFactory
                    {
                        //设置主机名
                        HostName = config.Host,

                        //设置心跳时间
                        RequestedHeartbeat = config.HeartBeat,

                        //设置自动重连
                        AutomaticRecoveryEnabled = config.AutomaticRecoveryEnabled,

                        //重连时间
                        NetworkRecoveryInterval = config.NetworkRecoveryInterval,

                        //用户名
                        UserName = config.UserName,

                        //密码
                        Password = config.Password,
                        //端口
                        Port = mqport
                    };
                    factory.AutomaticRecoveryEnabled = true;
                    factory.NetworkRecoveryInterval = new TimeSpan(1000);
                    _connection = _connection ?? factory.CreateConnection();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("RabbitMQ连接初始化({0})出错!{1}", config.Host, ex.Message));
            }
        }

        #region 发布消息
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message">消息实体</param>
        public void Publish(Messages message)
        {
            Type t = message.GetType();
            var queueInfo = GetRabbitMqAttribute(message);

            if (queueInfo.IsNull())
                throw new ArgumentException(RabbitMqAttribute);

            //var body = message.ToJson();


            var exchange = queueInfo.ExchangeName;
            var queue = queueInfo.QueueName;
            var routingKey = queueInfo.ExchangeName;
            var isProperties = queueInfo.IsProperties;
            var channel = GetModel(exchange, queue, routingKey, isProperties);

            //把消息queue写入Message,以便广播时发送方不用重新处理消息
            if (_exchangeQueue.ContainsKey(queueInfo.ExchangeName))
            {
                message.Queues = _exchangeQueue[queueInfo.ExchangeName];
            }

            byte[] messByte = Convertor.ObjectToByteArray(message, true);
            try
            {
                //channel.BasicPublish(exchange, routingKey, null, body.SerializeUtf8());
                channel.BasicPublish(exchange, routingKey, null, messByte);
                Console.WriteLine(string.Format("Publish:{0}-{1}", queueInfo.ExchangeName, queue));
            }
            catch (Exception ex)
            {
                //TODO:发送失败处理
                //_logWriter.Write(string.Format("发送消息失败：{0}", ex.Message));
            }
        }

        #endregion

        #region 接受消息
        /// <summary>
        /// 广播消息 queue自动生成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void SubscribeFanout<T>(Action<T> handler) where T : Messages
        {


            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo.IsNull())
                throw new ArgumentException(RabbitMqAttribute);

            var channel = _connection.CreateModel();
            ExchangeDeclare(channel, queueInfo.ExchangeName, ExchangeType.Fanout, queueInfo.IsProperties);

            string queue = channel.QueueDeclare().QueueName;


            channel.QueueBind(queue, queueInfo.ExchangeName, "");
            _publishModelDic[queue] = channel;


            _exchangeQueue[queueInfo.ExchangeName] = queue;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var obj = Convertor.ByteArrayToObject(body, true);
                var msg = obj as T;
                try
                {
                    if (msg.Queues != _exchangeQueue[queueInfo.ExchangeName])
                    {
                        handler(msg);

                        Console.WriteLine(string.Format("Subscribe:{0}-{1}", queueInfo.ExchangeName, queue));
                    }
                    else
                    {
                        Console.WriteLine("自动发布自己");
                    }
                }
                catch (Exception ex)
                {
                    //_logWriter.Write(string.Format("消息接收失败：{0}", ex.Message));
                }
                finally
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue, false, consumer);
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="handler">消息处理</param>
        public void Subscribe<T>(Action<T> handler) where T : Messages
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo.IsNull())
                throw new ArgumentException(RabbitMqAttribute);
            //var isDeadLetter = typeof(T) == typeof(DeadLetterQueue);
            Subscribe(queueInfo.ExchangeName, queueInfo.QueueName, queueInfo.IsProperties, handler, false);
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue">队列名称</param>
        /// <param name="isProperties"></param>
        /// <param name="handler">消费处理</param>
        /// <param name="isDeadLetter"></param>
        public void Subscribe<T>(string exchangeName, string queue, bool isProperties, Action<T> handler, bool isDeadLetter) where T : Messages
        {
            //队列声明
            var channel = GetModel(exchangeName, queue, isProperties);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                //var msgStr = SerializeExtension.DeserializeUtf8(body);
                //var msg = msgStr.FromJson<T>();
                var obj = Convertor.ByteArrayToObject(body, true);
                var msg = obj as T;
                try
                {
                    handler(msg);
                }
                catch (Exception ex)
                {
                    //TODO:订阅失败处理
                    //_logWriter.Write(string.Format("消息接收失败：{0}", ex.Message));
                    //if (ex.InnerException != null)
                    //{
                    //    _logWriter.Write(string.Format("消息接收失败InnerException：{0}", ex.InnerException.Message));
                    //}
                    //if (!isDeadLetter)
                    //    PublishToDead<DeadLetterQueue>(queue, msgStr, ex);
                }
                finally
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            //- 自动应答
            //消费者确认或者说消费者应答指的是RabbitMQ需要确认消息到底有没有被收到
            channel.BasicConsume(queue, false, consumer);
        }

        #endregion

        #region 释放资源
        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            foreach (var item in _publishModelDic)
            {
                item.Value.Dispose();
            }
            foreach (var item in _subscribeModelDic)
            {
                item.Value.Dispose();
            }
            _connection.Dispose();
        }
        #endregion

        /// <summary>
        /// 主动拉消息
        /// </summary>
        /// <param name="handler"></param>
        public void Pull<T>(Action<T> handler) where T : Messages
        {
            throw new NotImplementedException();
        }


        #region 获取Model 
        /// <summary>
        /// 获取Model
        /// </summary>
        /// <param name="exchange">交换机名称</param>
        /// <param name="queue">队列名称</param>
        /// <param name="routingKey"></param>
        /// <param name="isProperties">是否持久化</param>
        /// <returns></returns>
        private static IModel GetModel(string exchange, string queue, string routingKey, bool isProperties = false)
        {
            return _publishModelDic.GetOrAdd(queue, key =>
            {
                var model = _connection.CreateModel();
                ExchangeDeclare(model, exchange, ExchangeType.Fanout, isProperties);
                QueueDeclare(model, queue, isProperties);
                model.QueueBind(queue, exchange, routingKey);
                _publishModelDic[queue] = model;
                return model;
            });
        }

        private static IModel GetFanoutModel(string exchange, bool isProperties = false)
        {
            var model = _connection.CreateModel();
            ExchangeDeclare(model, exchange, ExchangeType.Fanout, isProperties);

            string queue = model.QueueDeclare().QueueName;


            model.QueueBind(queue, exchange, "");
            _publishModelDic[queue] = model;
            return model;


        }

        /// <summary>
        /// 获取Model
        /// </summary>
        /// <param name="queue">队列名称</param>
        /// <param name="isProperties"></param>
        /// <returns></returns>
        private static IModel GetModel(string exchange, string queue, bool isProperties = false)
        {
            return _subscribeModelDic.GetOrAdd(queue, value =>
            {
                var model = _connection.CreateModel();
                QueueDeclare(model, queue, isProperties);

                //每次消费的消息数
                model.BasicQos(0, 1, false);
                _subscribeModelDic[queue] = model;

                return model;
            });
        }
        #endregion

        #region 交换器声明
        /// <summary>
        /// 交换器声明
        /// </summary>
        /// <param name="iModel"></param>
        /// <param name="exchange">交换器</param>
        /// <param name="type">交换器类型：
        /// 1、Direct Exchange – 处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全
        /// 匹配。这是一个完整的匹配。如果一个队列绑定到该交换机上要求路由键 “dog”，则只有被标记为“dog”的
        /// 消息才被转发，不会转发dog.puppy，也不会转发dog.guard，只会转发dog
        /// 2、Fanout Exchange – 不处理路由键。你只需要简单的将队列绑定到交换机上。一个发送到交换机的消息都
        /// 会被转发到与该交换机绑定的所有队列上。很像子网广播，每台子网内的主机都获得了一份复制的消息。Fanout
        /// 交换机转发消息是最快的。
        /// 3、Topic Exchange – 将路由键和某模式进行匹配。此时队列需要绑定要一个模式上。符号“#”匹配一个或多
        /// 个词，符号“*”匹配不多不少一个词。因此“audit.#”能够匹配到“audit.irs.corporate”，但是“audit.*”
        /// 只会匹配到“audit.irs”。</param>
        /// <param name="durable">持久化</param>
        /// <param name="autoDelete">自动删除</param>
        /// <param name="arguments">参数</param>
        private static void ExchangeDeclare(IModel iModel, string exchange, string type = ExchangeType.Direct,
            bool durable = true,
            bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            exchange = string.IsNullOrWhiteSpace(exchange) ? "" : exchange.Trim();
            iModel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }
        #endregion

        #region 队列声明
        /// <summary>
        /// 队列声明
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queue">队列</param>
        /// <param name="durable">持久化</param>
        /// <param name="exclusive">排他队列，如果一个队列被声明为排他队列，该队列仅对首次声明它的连接可见，
        /// 并在连接断开时自动删除。这里需要注意三点：其一，排他队列是基于连接可见的，同一连接的不同信道是可
        /// 以同时访问同一个连接创建的排他队列的。其二，“首次”，如果一个连接已经声明了一个排他队列，其他连
        /// 接是不允许建立同名的排他队列的，这个与普通队列不同。其三，即使该队列是持久化的，一旦连接关闭或者
        /// 客户端退出，该排他队列都会被自动删除的。这种队列适用于只限于一个客户端发送读取消息的应用场景。</param>
        /// <param name="autoDelete">自动删除</param>
        /// <param name="arguments">参数</param>
        private static void QueueDeclare(IModel channel, string queue, bool durable = true, bool exclusive = false,
            bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            queue = string.IsNullOrWhiteSpace(queue) ? "UndefinedQueueName" : queue.Trim();
            channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }
        #endregion

        #region 获取属性
        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="msgEnt"></param>
        /// <returns></returns>
        private static RabbitMqAttribute GetRabbitMqAttribute(Messages mesEnt)
        {
            //var typeOfT = typeof(T);
            var typeOfT = mesEnt.GetType();
            return typeOfT.GetAttribute<RabbitMqAttribute>();
        }
        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="msgEnt"></param>
        /// <returns></returns>
        private static RabbitMqAttribute GetRabbitMqAttribute<T>()
        {
            var typeOfT = typeof(T);
            return typeOfT.GetAttribute<RabbitMqAttribute>();
        }
        #endregion

    }
}
