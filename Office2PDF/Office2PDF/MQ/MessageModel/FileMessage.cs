using Office2PDF.Common;
using Office2PDF.MQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace Office2PDF.Messages
{
    [Serializable]
    [MQ.RabbitMq("PptConvertMessageQueue", ExchangeName = "PptConvertMessageExchange", IsProperties = false)]
    public class PowerPointConvertMessage : MQ.Messages
    {
        private MessageType _MessageType;

        private FileInfo _FileInfo;

        public PowerPointConvertMessage() : base(2)
        {
        }

        public PowerPointConvertMessage(MessageEmergencyType MessageEmergency, MessageType MessageType, FileInfo fileInfo) : base((int)MessageEmergency)
        {
            this._MessageType = MessageType;
            this._FileInfo = fileInfo;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public new MessageEmergencyType MessageEmergency
        {
            get
            {
                return (MessageEmergencyType)base.MessageEmergency;
            }
        }

        public FileInfo FileInfo
        {
            get
            {
                return this._FileInfo;
            }
        }

        public override string MessageCatalogID
        {
            get
            {
                return this._FileInfo.FileId;
            }
        }

        public MessageType MessageType
        {
            get
            {
                return this._MessageType;
            }
        }

        public bool IsTempStorage { get; set; } = false;
    }

    [Serializable()]
    public enum MessageType
    {
        ActivateInstance = 3,
        ForwardInstance = 2,
        OriginateInstance = 0,
        ConvertFile = -1
    }

    public enum MessageEmergencyType
    {
        Lowest,
        Low,
        Normal,
        High,
        Highest
    }

}
