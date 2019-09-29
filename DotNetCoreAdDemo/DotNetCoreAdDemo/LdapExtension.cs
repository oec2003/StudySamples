using System;
using System.Collections.Generic;
using Novell.Directory.Ldap;

namespace DotNetCoreAdDemo
{
    public static class LdapExtension
    {
        public static string Guid(this LdapEntry entry)
        {
            var bytes = (byte[])(entry.getAttribute("objectGUID").ByteValue as object);
            var guid = new Guid(bytes);
            return guid.ToString();
        }

        public static List<LdapEntry> Children(this LdapEntry entry, LdapConnection connection)
        {
            //string filter = "(&(objectclass=user))";
            List<LdapEntry> entryList = new List<LdapEntry>();
            LdapSearchResults lsc = connection.Search(entry.DN, LdapConnection.SCOPE_ONE, "objectClass=*", null, false);
            if (lsc == null) return entryList;

            while (lsc.HasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();

                    if (nextEntry.IsUser() || nextEntry.IsOrganizationalUnit())
                    {
                        entryList.Add(nextEntry);
                    }
                }
                catch (LdapException e)
                {
                    continue;
                }
            }
            return entryList;
        }

        public static List<string> ObjectClass(this LdapEntry entry)
        {
            List<string> list = new List<string>();
            byte[][] bytes = (byte[][])(entry.getAttribute("objectClass").ByteValueArray as object);
            for (var i = 0; i < bytes.Length; i++)
            {
                string str = System.Text.Encoding.Default.GetString(bytes[i]);
                list.Add(str.ToLower());
            }
            return list;
        }

        public static bool IsUser(this LdapEntry entry)
        {
            return entry.ObjectClass().Contains("user");
        }

        public static bool IsOrganizationalUnit(this LdapEntry entry)
        {
            return entry.ObjectClass().Contains("organizationalunit");
        }

        public static DateTime WhenChanged(this LdapEntry entry)
        {
            string value = entry.getAttribute("whenChanged").StringValue;
            if (value.Split('.').Length > 1)
            {
                value = value.Split('.')[0];
            }
            DateTime whenChanged = DateTime.ParseExact(value, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            return whenChanged;
        }

        public static bool ContainsAttr(this LdapEntry entry, string attrName)
        {
            LdapAttribute ldapAttribute = new LdapAttribute(attrName);
            return entry.getAttributeSet().Contains(ldapAttribute);
        }

        public static string AttrStringValue(this LdapEntry entry, string attrName)
        {
            if (!entry.ContainsAttr(attrName))
            {
                return string.Empty;
            }
            return entry.getAttribute(attrName).StringValue;
        }
    }
}
