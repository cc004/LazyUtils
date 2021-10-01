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
            var file = Path.Combine(TShock.SavePath, t.Filename + ".json");
            if (File.Exists(file))
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
            }
            File.WriteAllText(file, JsonConvert.SerializeObject(t,Formatting.Indented));
            return t;
        }

        private static void OnReload(ReloadEventArgs args) => Load();
        protected virtual T DefaultConfig() => new T();
        static Config()
        {
                GeneralHooks.ReloadEvent += OnReload;
        }
        public static T Instance => _instance = _instance ?? GetConfig();
        public static void Load() => _instance = GetConfig();
    }
}
