using System;
namespace DotNetCoreAdDemo
{
    public class Org
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }

        public bool AddOrg(Org org)
        {
            //数据库操作
            return true;
        }
        public bool UpdateOrg(Org org)
        {
            //数据库操作
            return true;
        }
        public Org GetRootOrg()
        {
            //获取根Org
            return new Org();
        }
        public Org GetOrgById(string id)
        {
            //根据Id获取Org
            return new Org();
        }
    }
}
