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
        public readonly int unmatched;
        public readonly CommandBase current;

        public ParseResult(CommandBase current, int num)
        {
            this.current = current;
            this.unmatched = num;
        }
    }

    private const string NoPerm = "没有权限";
    private const string MustReal = "你必须在游戏内使用该指令";

    protected string[] permissions;
    private readonly bool _realPlayer;
    protected string info;

    public abstract ParseResult TryParse(CommandArgs args, int current);
    public override string ToString()
    {
        return this.info;
    }

    protected CommandBase(MemberInfo member)
    {
        this.permissions = member.GetCustomAttributes<Permission>().Select(p => p.Name).ToArray();
        if (member.GetCustomAttribute<RealPlayerAttribute>() != null)
        {
            this._realPlayer = true;
        }
    }

    protected CommandBase()
    {
        
    }

    public bool CanExec(TSPlayer plr)
    {
        return (!this._realPlayer || plr.RealPlayer) && this.permissions.All(plr.HasPermission);
    }

    protected bool CheckPlayer(TSPlayer plr)
    {
        if (this._realPlayer && !plr.RealPlayer)
        {
            plr.SendErrorMessage(MustReal);
        }
        else if (this.permissions.Any(perm => !plr.HasPermission(perm)))
        {
            plr.SendErrorMessage(NoPerm);
        }
        else
        {
            return true;
        }

        return false;
    }
    protected ParseResult GetResult(int num)
    {
        return new(this, num);
    }
}