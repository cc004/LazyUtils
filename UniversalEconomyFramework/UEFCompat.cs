using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using UniversalEconomyFramework;

namespace UnifiedEconomyFramework
{
    public interface IEconomy
    {
    }

    public class UEF
    {
        private const string FRAMEWORK = "UnifiedEconomyFramework";

        public static void ResisterEconomy(IEconomy framework)
        {
            throw new NotImplementedException();
        }

        public static void DeresisterEconomy(IEconomy framework)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 增加玩家余额, 返回是否成功调用.
        /// </summary>
        /// <param name="num">金额</param>
        /// <returns></returns>
        public static bool MoneyUp(string name, long num)
        {
            using (var query = Economy.Query(FRAMEWORK, name))
                query.Set(e => e.bank, e => e.bank + num).Update();
            return true;
        }

        /// <summary>
        /// 减少玩家余额, 返回是否成功调用.
        /// </summary>
        /// <param name="num">金额</param>
        /// <returns></returns>
        public static bool MoneyDown(string name, long num)
        {
            using (var query = Economy.Query(FRAMEWORK, name))
                query.Set(e => e.bank, e => e.bank - num).Update();
            return true;
        }

        /// <summary>
        /// 显示玩家余额
        /// </summary>
        /// <param name="name">玩家名</param>
        /// <returns></returns>
        public static long Balance(string name)
        {
            using (var query = Economy.Query(FRAMEWORK, name))
                return (long) query.Single().bank;
        }
    }
}

