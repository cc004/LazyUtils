using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TShockAPI;

namespace LazyUtils.Commands;

internal abstract class CommandBase
{
    protected internal struct ParseResult
    {
        public int unmatched;
        public CommandBase current;

        public ParseResult(CommandBase current, int num)
        {
            this.current = current;
            unmatched = num;
        }
    }

    private const string NoPerm = "没有权限";
    private const string MustReal = "你必须在游戏内使用该指令";

    protected string[] permissions;
    protected bool realPlayer;
    protected string info;

    public abstract ParseResult TryParse(CommandArgs args, int current);
    public override string ToString() => info;

    protected CommandBase(MemberInfo member)
    {
        permissions = member.GetCustomAttributes<Permission>().Select(p => p.Name).ToArray();
        if (member.GetCustomAttribute<RealPlayerAttribute>() != null) realPlayer = true;
    }

    protected CommandBase()
    {

    }

    public bool CanExec(TSPlayer plr) =>
        !(realPlayer && plr.RealPlayer) && permissions.All(plr.HasPermission);

    protected bool CheckPlayer(TSPlayer plr)
    {
        if (realPlayer && !plr.RealPlayer)
            plr.SendErrorMessage(MustReal);
        else if (permissions.Any(perm => !plr.HasPermission(perm)))
            plr.SendErrorMessage(NoPerm);
        else
            return true;
        return false;
    }
    protected ParseResult GetResult(int num) => new(this, num);

}