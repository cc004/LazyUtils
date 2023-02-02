using LazyUtils.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace LazyUtils;

[AttributeUsage(AttributeTargets.Class)]
public class RestAttribute : Attribute
{
    public HashSet<string> alias;

    public RestAttribute(params string[] aliases)
    {
        this.alias = new HashSet<string>(aliases);
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
    public HashSet<string> alias;

    public CommandAttribute(params string[] aliases)
    {
        this.alias = new HashSet<string>(aliases);
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute : Attribute
{
}

public abstract class LazyPlugin : TerrariaPlugin
{
    public override string Name => this.GetType().Namespace;
    public sealed override Version Version => this.GetType().Assembly.GetName().Version;

    protected LazyPlugin(Main game) : base(game)
    {
        AutoLoad();
    }

    public override void Initialize()
    {
    }

    internal void AutoLoad()
    {
        foreach (var type in this.GetType().Assembly.GetTypes())
        {
            if (type.IsDefined(typeof(ConfigAttribute), false))
            {
                var name = type.BaseType.GetMethod("Load").Invoke(null, new object[0]); ;
                Console.WriteLine($"[{this.Name}] config registered: {name}");
            }
            else if (type.IsDefined(typeof(CommandAttribute), false))
            {
                var names = CommandHelper.Register(type);
                Console.WriteLine($"[{this.Name}] command registered: {string.Join(",", names)}");
            }
            else if (type.IsDefined(typeof(RestAttribute), false))
            {
                foreach (var name in type.GetCustomAttributes<RestAttribute>(false).SelectMany(attr => attr.alias))
                {
                    RestHelper.Register(type, name, this);
                }
            }
        }
    }
}