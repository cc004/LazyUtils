using System;
using System.Collections.Generic;
using OTAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace LazyUtils;

[ApiVersion(2, 1)]
public class PluginContainer : LazyPlugin
{
    public PluginContainer(Main game) : base(game) { }
    public override void Initialize()
    {
        ServerApi.Hooks.GamePostUpdate.Register(this, _ => ++timer);
    }
}