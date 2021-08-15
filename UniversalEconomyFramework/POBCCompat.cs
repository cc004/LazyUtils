using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalEconomyFramework;
using LinqToDB;

namespace POBC2
{
    public static class Db
    {
        private const string FRAMEWORK = "POBC";

        public static void Connect()
        {
        }

        public static bool Queryuser(string user)
        {
            using (var query = Economy.Query(FRAMEWORK, user))
                return query.Single().bank == 0;
        }

        public static void UpC(string user, int data, string str = "未填写原因")
        {
            using (var query = Economy.Query(FRAMEWORK, user))
                query.Set(e => e.bank, e => e.bank + data).Update();
        }
        public static void DownC(string user, int data, string str = "未填写原因")
        {
            using (var query = Economy.Query(FRAMEWORK, user))
                query.Set(e => e.bank, e => e.bank - data).Update();
        }

        public static void Adduser(string user, int data, string str = "未填写原因")
        {
            using (var query = Economy.Query(FRAMEWORK, user))
                query.Set(e => e.bank, data).Update();
        }
        public static int QueryCurrency(string user)
        {
            using (var query = Economy.Query(FRAMEWORK, user))
                return (int)query.Single().bank;
        }
    }
}
