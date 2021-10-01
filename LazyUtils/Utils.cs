using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace LazyUtils
{
    public static class Utils
    {
        private static readonly string combatTextFormat= new string('\n', 20)+"{0}"+new string('\n',20);
        public static Func<bool> Eval(string expression)
        {
            int idx = expression.LastIndexOf('.');
            var @class = expression.Substring(0, idx);
            var name = expression.Substring(idx + 1);
            var field = typeof(Main).Assembly.GetType(@class).GetField(name, BindingFlags.Static | BindingFlags.Public);
            return ()=>(bool)field.GetValue(null);
        }

        public static Func<bool> Eval(List<string> include,List<string> exclude)
        {
            var exps1 = include.Select(e => Eval(e));
            var exps2=exclude.Select(e => Eval(e));
            return () => (exps1.All(e => e()) && exps1.All(e => !e()));
        }

        public static void Send(this Item item)
        {
            var tuple = FindItemRef(item);
            if (tuple == null) return;
            TShock.Players[tuple.Item1].SendData(PacketTypes.PlayerSlot, "", tuple.Item1, tuple.Item2);
        }

        public static void SendCombatText(this TSPlayer player, string message, Color color, bool visableToOthers = true)
        {
            NetMessage.SendData(119, player.TPlayer.whoAmI, visableToOthers ? -1 : player.Index, NetworkText.FromLiteral(message), (int)color.PackedValue, player.X, player.Y, 0, 0, 0, 0);
        }
        public static void SendStatusMessage(this TSPlayer player, string message)
        {
            player.SendData(PacketTypes.Status,string.Format(combatTextFormat,message));
        }

        public static void SendPlayerUpdate(this TSPlayer player) => player.SendData(PacketTypes.PlayerUpdate, "", player.Index);

        public static void SendPlayerSlot(this TSPlayer player, int slot) => player.SendData(PacketTypes.PlayerSlot, "", slot);

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
