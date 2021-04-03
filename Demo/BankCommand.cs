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
            args.Player.Get<Bank>().bank = 100;
            ConfigHelper.Save<Bank>();
        }

        [Permission("lazyutil.bank.set")]
        public static void Set(CommandArgs args, string name, int money)
        {
            ConfigHelper.Get<Bank>(name).bank = money;
            args.Player.SendInfoMessage($"成功设定{name}的金币为{money}");
            ConfigHelper.Save<Bank>();
        }

        [Permission("lazyutil.bank.query")]
        public static void Query(CommandArgs args, string name)
        {
            args.Player.SendInfoMessage($"{name}的金币为{ConfigHelper.Get<Bank>(name).bank}");
        }

        [Permission("lazyutil.bank.query")]
        public static void Query(CommandArgs args)
        {
            args.Player.SendInfoMessage($"你的金币为{args.Player.Get<Bank>().bank}");
        }
    }

}
