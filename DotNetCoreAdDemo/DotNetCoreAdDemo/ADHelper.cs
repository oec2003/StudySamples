using System;
using System.Collections.Generic;
using Novell.Directory.Ldap;

namespace DotNetCoreAdDemo
{
    public class ADHelper
    {
        private LdapConnection _connection;
        private string[] _adPaths;
        private string _adHost;
        private Org _org;
        private User _user;
        public bool ADConnect()
        {
            _adHost = "192.168.16.160";
            string adAdminUserName = "administrator";
            string adAdminPassword = "123456";
            _adPaths =new string[] { "OU=oec2003,DC=COM,DC=cn" };

            if ((string.IsNullOrEmpty(_adHost) || string.IsNullOrEmpty(adAdminUserName)) ||
                string.IsNullOrEmpty(adAdminPassword))
            {
                return false;
            }
            try
            {
                _connection = new LdapConnection();
                _connection.Connect(_adHost, LdapConnection.DEFAULT_PORT);
                _connection.Bind(adAdminUserName, adAdminPassword);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public List<LdapEntry> GetRootEntries(string[] adPathes, string host)
        {
            List<LdapEntry> list = new List<LdapEntry>();
            foreach (string path in adPathes)
            {
                if (!string.IsNullOrEmpty(host))
                {
                    LdapEntry entry = _connection.Read(path);
                    list.Add(entry);
                }
            }
            return list;
        }

        public bool Sync()
        {
            ADConnect();

            if (_connection == null)
            {
                throw new Exception("AD连接错误，请确认AD相关信息配置正确!");
            }
            bool result = true;
            List<LdapEntry> entryList = this.GetRootEntries(_adPaths, _adHost);
            _org = new Org();
            _user = new User();
            Org rootOrg = _org.GetRootOrg();
            foreach (LdapEntry entry in entryList)
            {
                SyncDirectoryEntry(entry, rootOrg, entry);
            }

            return result;
        }

        private void SyncDirectoryEntry(LdapEntry rootEntry, Org parentOrg, LdapEntry currentEntry)
        {
            List<LdapEntry> entryList = currentEntry.Children(_connection);
            foreach (LdapEntry entry in entryList)
            {
                if (entry.IsOrganizationalUnit())
                {
                    Org org = this.SyncOrgFromEntry(rootEntry, parentOrg, entry);
                    this.SyncDirectoryEntry(rootEntry, org, entry);
                }
                else if (entry.IsUser())
                {
                    this.SyncUserFromEntry(rootEntry, parentOrg, entry);
                }
            }
        }

        private Org SyncOrgFromEntry(LdapEntry rootEntry, Org parentOrg, LdapEntry entry)
        {
            string orgId = entry.Guid().ToLower();
            Org org = this._org.GetOrgById(orgId) as Org;
            if (org != null)
            {
                if (entry.ContainsAttr("ou"))
                {
                    org.Name = entry.getAttribute("ou").StringValue + string.Empty;
                }
                //设置其他属性的值
                _org.UpdateOrg(org);
                return org;
            }
            org = new Org
            {
                Id = orgId,
                ParentId = parentOrg.Id,
            };

            //设置其他属性的值
            this._org.AddOrg(org);
            return org;
        }

        private User SyncUserFromEntry(LdapEntry rootEntry, Org parentOrg, LdapEntry entry)
        {
            string userId = entry.Guid().ToLower();
            User user = this._user.GetUserById(userId);
            if (user != null)
            {
                user.ParentId = parentOrg.Id;
                //设置其他属性的值
                this._user.UpdateUser(user);
                  
                return user;
            }
            user = new User
            {
                Id = userId,
                ParentId = parentOrg.Id
            };
            //设置其他属性的值
            this._user.AddUser(user);
            return user;
        }
    }
}
