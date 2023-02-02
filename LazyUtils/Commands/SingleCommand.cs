using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using TShockAPI;

namespace LazyUtils.Commands;

internal partial class SingleCommand : CommandBase
{
    private readonly Parser[] argParsers;
    private readonly FastReflectionDelegate method;

    public SingleCommand(MethodInfo method, string infoPrefix) : base(method)
    {
        var param = method.GetParameters();
        var ap = new List<Parser>();
        var sb = new StringBuilder();
        sb.Append(infoPrefix);

        foreach (var p in param.Skip(1))
        {
            ap.Add(parsers[p.ParameterType]);
            sb.Append($"<{p.Name}: {_friendlyName[p.ParameterType]}> ");
        }

        this.argParsers = ap.ToArray();
        this.info = sb.ToString();
        this.method = method.CreateFastDelegate();
    }

    public override ParseResult TryParse(CommandArgs args, int current)
    {
        var p = args.Parameters;
        var n = this.argParsers.Length;
        if (p.Count != n + current)
        {
            return this.GetResult(Math.Abs(n + current - p.Count));
        }

        var a = new object[n + 1];
        a[0] = args;
        var unmatched = this.argParsers.Where((t, i) => !t(p[current + i], out a[i + 1])).Count();
        if (unmatched != 0)
        {
            return this.GetResult(unmatched);
        }

        if (this.CheckPlayer(args.Player))
        {
            this.method(null, a);
        }

        return this.GetResult(0);
    }

}