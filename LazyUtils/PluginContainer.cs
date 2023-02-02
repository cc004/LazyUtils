using OTAPI;
using System;
using System.Collections.Generic;
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
        ServerApi.Hooks.GamePostUpdate.Register(this, PostUpdate);
    }

    private void PostUpdate(EventArgs _)
    {
        ++TimingUtils.Timer;

        while (TimingUtils.scheduled.TryPeek(out var action, out var time))
        {
            if (time > TimingUtils.Timer) break;
            action();
            TimingUtils.scheduled.Dequeue();
        }
    }
}