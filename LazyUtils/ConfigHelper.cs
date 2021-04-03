using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace LazyUtils
{
    public static class ConfigHelper
    {
        private static Dictionary<Type, Dictionary<string, object>> caches = new Dictionary<Type, Dictionary<string, object>>();
        public static event Func<Type, string> OnGetFilename;

        private static string GetFilename<T>()
        {
            return OnGetFilename?.Invoke(typeof(T)) ?? $@"tshock\{typeof(T).FullName}.json";
        }

        public static List<Tuple<string, T>> List<T>()
        {
            if (!caches.ContainsKey(typeof(T)))
                LoadOrDefault<T>();
            return caches[typeof(T)].Select(pair => new Tuple<string, T>(pair.Key, (T)pair.Value)).ToList();
        }

        public static T Get<T>(string name)
        {
            if (!caches.ContainsKey(typeof(T)))
                LoadOrDefault<T>();
            if (!caches[typeof(T)].ContainsKey(name))
                caches[typeof(T)].Add(name, Activator.CreateInstance(typeof(T)));
            return (T)caches[typeof(T)][name];
        }

        public static T Get<T>(this TSPlayer player) => Get<T>(player.GetName());

        public static void Save<T>()
        {
            var obj = new JObject();
            if (!caches.ContainsKey(typeof(T)))
                LoadOrDefault<T>();
            foreach (var pair in caches[typeof(T)])
                obj.Add(pair.Key, JsonConvert.SerializeObject(pair.Value));
            File.WriteAllText(GetFilename<T>(), obj.ToString());
        }

        public static void LoadOrDefault<T>()
        {
            caches.Add(typeof(T), new Dictionary<string, object>());
            try
            {
                var obj = JObject.Parse(File.ReadAllText(GetFilename<T>()));
                foreach (var name in obj.Properties())
                    caches[typeof(T)].Add(name.Name, JsonConvert.DeserializeObject<T>((string)name.Value));
            }
            catch
            {
                caches[typeof(T)] = new Dictionary<string, object>();
            }
        }

    }
}
