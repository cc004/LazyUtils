using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Wolfje.Plugins.SEconomy.Lang;

namespace Wolfje.Plugins.SEconomy
{
	public class SEconomyPlugin : TerrariaPlugin
	{
		public static Localization Locale
		{
			get;
			private set;
		}

		public override string Author => "Wolfje 汉化:恋";

		public override string Description => "Provides server-sided currency tools for servers running TShock";

		public override string Name => string.Concat("SEconomy" + Version.Build, " 汉化版");

		public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        private static SEconomy _instance;
		public static SEconomy Instance
		{
            get
            {
                if (_instance != null) return _instance;
                Localization.PrepareLanguages();
                Locale = new Localization("zh-CN");
                PrintIntro();
				return _instance = new SEconomy(new SEconomyPlugin(null));
			}
            set
            {
                _instance = value;
            }
		}

		public static event EventHandler SEconomyLoaded;

		public static event EventHandler SEconomyUnloaded;

		public SEconomyPlugin(Main Game)
			: base(Game)
		{
		}

		public override void Initialize()
		{
		}

		private static void PrintIntro()
		{
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write(" SEconomy ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(Assembly.GetExecutingAssembly().GetName().Version.Build);
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("汉化版");
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write(" Copyright (C) Wolfje, 2014-2016 - ");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write("http://github.com/tylerjwatson/SEconomy");
			Console.WriteLine("\r\n");
			ConsoleColor backgroundColor = Console.BackgroundColor;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.WriteLine(" SEconomy是个免费软件。如果你购买了它，那么你受骗了。");
			Console.BackgroundColor = backgroundColor;
			Console.WriteLine("\r\n");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(" 请稍候...");
			Console.WriteLine();
			Console.ResetColor();
		}

		public string GetVersionString()
		{
			StringBuilder stringBuilder = new StringBuilder("SEconomy Update");
			stringBuilder.AppendFormat(" {0}", Version.Build);
			stringBuilder.Append(" 汉化版");
			return stringBuilder.ToString();
		}

		protected void RaiseUnloadedEvent()
		{
			if (SEconomyPlugin.SEconomyUnloaded != null)
			{
				SEconomyPlugin.SEconomyUnloaded(this, new EventArgs());
			}
		}

		protected void RaiseLoadedEvent()
		{
			if (SEconomyPlugin.SEconomyLoaded != null)
			{
				SEconomyPlugin.SEconomyLoaded(this, new EventArgs());
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing && Instance != null)
			{
				Instance.Dispose();
				Instance = null;
			}
			base.Dispose(disposing);
		}
	}
}
