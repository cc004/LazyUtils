using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace LazyUtils.Commands;

internal partial class Command : CommandBase
{
    private readonly Dictionary<string, List<CommandBase>> _dict = new ();
    private readonly List<CommandBase> _main = new();
    private readonly string _infoPrefix;

    public Command(MemberInfo type, string infoPrefix) : base(type)
    {
        info = $"{infoPrefix} <...>";
        _infoPrefix = infoPrefix;
    }

    public void PostBuildTree()
    {
        _main.Add(new HelpCommand(this, _infoPrefix));
    }

    public void Add(string cmd, CommandBase sub)
    {
        if (string.IsNullOrEmpty(cmd))
            _main.Add(sub);
        else
            if (_dict.TryGetValue(cmd, out var lst))
                lst.Add(sub);
            else
                _dict.Add(cmd, new List<CommandBase>
                {
                    sub
                });
    }
    
    public override ParseResult TryParse(CommandArgs args, int current)
    {
        if (!CheckPlayer(args.Player)) return GetResult(0);

        var most = GetResult(args.Parameters.Count - current + 1);
        
        if (current < args.Parameters.Count && _dict.TryGetValue(args.Parameters[current], out var subs))
        {
            foreach (var sub in subs)
            {
                var res = sub.TryParse(args, current + 1);
                if (res.unmatched == 0) return res;
                if (res.unmatched < most.unmatched) most = res;

            }
        }

        foreach (var sub in _main)
        {
            var res = sub.TryParse(args, current);
            if (res.unmatched == 0) return res;
            if (res.unmatched < most.unmatched) most = res;
        }

        return most;
    }

}