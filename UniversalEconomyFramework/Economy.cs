using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;

namespace UniversalEconomyFramework
{
    public sealed class Economy : PlayerConfigBase<Economy>
    {
        public double bank;

        internal static DisposableQuery<Economy> Query(string fxname, string user) =>
            Db.Get<Economy>(user, TableConfig.Table[fxname].table);

        internal static float Multiplier(string fxname)
        {
            return TableConfig.Table[fxname].multiplier;
        }
    }
}
