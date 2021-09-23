using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace LazyUtils
{
    public static class Utils
    {
        private sealed class LegacyDisposable : IDisposable
        {
            private Action action;
            public LegacyDisposable(Action action)
            {
                this.action = action;
            }
            public void Dispose()
            {
                action();
            }
        }

        public static void Send(this Item item)
        {
            var tuple = FindItemRef(item);
            if (tuple == null) return;
            TShock.Players[tuple.Item1].SendData(PacketTypes.PlayerSlot, "", tuple.Item1, tuple.Item2);
        }

        private static Tuple<int, int> FindItemRef(object item)
        {
            for (var i = 0; i < Main.maxPlayers; ++i)
            {
                var plr = Main.player[i];
                if (plr?.inventory == null) continue;
                var n = plr.inventory.Length;
                for (var j = 0; j < n; ++j)
                    if (plr.inventory[j] == item)
                        return new Tuple<int, int>(i, j);
            }

            return null;
        }
    }
}
