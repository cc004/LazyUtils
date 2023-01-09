using LazyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using LazyUtils.Commands;
using LinqToDB;
using TShockAPI;
using TShockAPI.DB;

namespace Demo;

[Command("bank")]
public static class Commands
{
    [Permissions("lazyutil.bank.create")]
    public static void Create(CommandArgs args)
    {
        using var query = args.Player.Get<Bank>();
        query.Set(b => b.bank, 100).Update();
    }

    [Permissions("lazyutil.bank.set")]
    public static class Set
    {
        [Main]
        public static void Main(CommandArgs args, TSPlayer plr, int money)
        {
            using (var query = plr.Get<Bank>())
                query.Set(b => b.bank, money).Update();
            args.Player.SendInfoMessage($"成功设定玩家{plr.Name}的金币为{money}");
        }

        [Main]
        public static void Main(CommandArgs args, string name, int money)
        {
            using (var query = Db.Get<Bank>(name))
                query.Set(b => b.bank, money).Update();
            args.Player.SendInfoMessage($"成功设定账户{name}的金币为{money}");
        }

    }

    [Permissions("lazyutil.bank.query")]
    public static class Query
    {
        [Main]
        public static void Main(CommandArgs args, string name)
        {
            using var query = Db.Get<Bank>(name);
            args.Player.SendInfoMessage($"{name}的金币为{query.Single().bank}");
        }

        [Main]
        public static void Main(CommandArgs args)
        {
            using var query = args.Player.Get<Bank>();
            args.Player.SendInfoMessage($"你的金币为{query.Single().bank}");
        }
    }
}