using LazyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using LinqToDB;

namespace Demo
{
    public class BankCommand
    {
        [Permission("lazyutil.bank.create")]
        public static void Create(CommandArgs args)
        {
            using (var query = args.Player.Get<Bank>())
                query.Set(b => b.bank, 100).Update();
        }

        [Permission("lazyutil.bank.set")]
        public static void Set(CommandArgs args, string name, int money)
        {
            using (var query = Db.Get<Bank>(name))
                query.Set(b => b.bank, money).Update();
            args.Player.SendInfoMessage($"成功设定{name}的金币为{money}");
        }

        [Permission("lazyutil.bank.query")]
        public static void Query(CommandArgs args, string name)
        {
            using (var query = Db.Get<Bank>(name))
                args.Player.SendInfoMessage($"{name}的金币为{query.Single().bank}");
        }

        [Permission("lazyutil.bank.query")]
        public static void Query(CommandArgs args)
        {
            using (var query = Db.Get<Bank>(args.Player))
                args.Player.SendInfoMessage($"你的金币为{query.Single().bank}");
        }
    }

}
