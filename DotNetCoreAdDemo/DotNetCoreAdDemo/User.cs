using System;
namespace DotNetCoreAdDemo
{
    public class User
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }

        public bool AddUser(User user)
        {
            //数据库操作
            return true;
        }
        public bool UpdateUser(User user)
        {
            //数据库操作
            return true;
        }
        public User GetUserById(string id)
        {
            //根据Id获取Org
            return new User();
        }
    }
}
