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
            var t = new T();
            if(!hooked)
                GeneralHooks.ReloadEvent += OnReload;
            var file = Path.Combine(TShock.SavePath, t.Filename + ".json");
            if (File.Exists(file)) return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
            File.WriteAllText(file, JsonConvert.SerializeObject(t));
            return t;
        }
        private static void OnReload(ReloadEventArgs args)
        {
            _instance = GetConfig();
        }
        
        public static T Instance => _instance = _instance ?? GetConfig();
    }
}
