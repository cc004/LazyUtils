using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;

namespace UniversalEconomyFramework
{
    public class Economy : PlayerConfigBase<Economy>
    {
        public double bank;

        internal static DisposableQuery<Economy> Query(string fxname, string user) =>
            Db.Get<Economy>(user, TableConfig.TableName[fxname]);
    }
}
