using LazyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Demo
{
    public class BankCommand
    {
        [Permission("lazyutil.bank.create")]
        public static void Create(CommandArgs args)
        {
            using (var bank = ConfigHelper.Get<Bank>())
                args.Player.Get(bank).bank = 100;
        }

        [Permission("lazyutil.bank.set")]
        public static void Set(CommandArgs args, string name, int money)
        {
            using (var bank = ConfigHelper.Get<Bank>())
                bank.Get(name).bank = money;

            args.Player.SendInfoMessage($"成功设定{name}的金币为{money}");
        }

        [Permission("lazyutil.bank.query")]
        public static void Query(CommandArgs args, string name)
        {
            using (var bank = ConfigHelper.Get<Bank>())
                args.Player.SendInfoMessage($"{name}的金币为{bank.Get(name).bank}");
        }

        [Permission("lazyutil.bank.query")]
        public static void Query(CommandArgs args)
        {
            using (var bank = ConfigHelper.Get<Bank>())
                args.Player.SendInfoMessage($"你的金币为{bank.Get(args.Player).bank}");
        }
    }

}
