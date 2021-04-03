using Terraria;
using TerrariaApi.Server;

namespace LazyUtils
{
    [ApiVersion(2, 1)]
    public class PluginContainer : TerrariaPlugin
    {
        public override string Name => "LazyUtils";

        public PluginContainer(Main game) : base(game) { }

        public override void Initialize() { }
    }
}
