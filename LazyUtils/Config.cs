using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace LazyUtils
{
    public abstract class Config<T> where T : Config<T>, new()
    {
       
        private static T _instance;
        private static bool hooked;
        protected virtual string Filename => typeof(T).Namespace;
        
        private static T GetConfig()
        {
            var t = new T().DefaultConfig();
            if (!hooked)
            {
                hooked = true;
                GeneralHooks.ReloadEvent += OnReload;
            }
            var file = Path.Combine(TShock.SavePath, t.Filename + ".json");
            if (File.Exists(file))
            {
                Console.WriteLine("Read");
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
            }
            Console.WriteLine("Default");
            File.WriteAllText(file, JsonConvert.SerializeObject(t,Formatting.Indented));
            return t;
        }

        private static void OnReload(ReloadEventArgs args) => Load();
        protected abstract T DefaultConfig();

        public static T Instance => _instance = _instance ?? GetConfig();
        public static void Load() => _instance = GetConfig();
    }
}
