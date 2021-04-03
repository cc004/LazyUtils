using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace LazyUtils
{
    public static class CompatHelper
    {
        public static List<dynamic> GetUsers()
        {
            try
            {
                return TShock.Users.get
            }
        }

        public static string GetName(this TSPlayer player)
        {
            dynamic p = player;
            try
            {
                return p?.User?.Name;
            }
            catch
            {
                return p?.UserAccount?.Name;
            }
        }
    }
}
