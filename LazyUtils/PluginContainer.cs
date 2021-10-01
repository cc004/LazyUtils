using OTAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace LazyUtils
{
    [ApiVersion(2, 1)]
    public class LazyPlugin : TerrariaPlugin
    {
        public static long timer;
        public override string Name => "LazyUtils";

        public LazyPlugin(Main game) : base(game) { }
        public override void Initialize()
        {
            ServerApi.Hooks.GamePostUpdate.Register(this, _ => ++timer);
        }
    }
}
