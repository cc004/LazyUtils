using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Wolfje.Plugins.SEconomy.Journal;

namespace Wolfje.Plugins.SEconomy;

public class WorldEconomy : IDisposable
{
    protected SEconomy Parent
    {
        get;
        set;
    }

    public int CustomMultiplier
    {
        get;
        set;
    }
		
    public WorldEconomy(SEconomy parent)
    {
        Parent = parent;
        CustomMultiplier = 1;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
		
    public static bool HasBuff(Player p, int buff)
    {
        for (int i = 0; i < 22; i++)
        {
            if (p.buffType[i] == buff && p.buffTime[i] > 0)
            {
                return true;
            }
        }
        return false;
    }
}