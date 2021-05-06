using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace LazyUtils
{
    public class User
    {
        public string Name, Group, Ip;
        public int ID;
    }

    public static class CompatHelper
    {
        public static void Error(string s)
        {
            TShock.Log.Error(s);
        }

        public static string GetName(this TSPlayer player)
        {
            dynamic t = player;
            try
            {
                return t?.User?.Name;
            }
            catch
            {
                return t?.Account?.Name;
            }
        }

        public static List<User> GetUsers()
        {
            try
            {
                var manager = typeof(TShock).GetField("Users").GetValue(null);
                var users = manager.GetType().GetMethod("GetUsers").Invoke(manager, new object[0]) as IEnumerable<dynamic>;
                return users.Select(u => new User
                {
                    Name = u.Name,
                    ID = u.ID,
                    Group = u.Group,
                    Ip = u.KnownIps
                }).ToList();
            }
            catch
            {
                var manager = typeof(TShock).GetField("UserAccounts").GetValue(null);
                var users = manager.GetType().GetMethod("GetUserAccounts").Invoke(manager, new object[0]) as IEnumerable<dynamic>;
                return users.Select(u => new User
                {
                    Name = u.Name,
                    ID = u.ID,
                    Group = u.Group,
                    Ip = u.KnownIps
                }).ToList();
            }
        }
    }
}
