using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;

namespace LazyUtils
{
    public static class ConfigHelper
    {
        public static T Get<T>(this TSPlayer player, Config<T> config) => config.Get(player);

        public sealed class Config<T> : IDisposable
        {
            public Config()
            {
                LoadOrDefault();
            }

            public T Get(TSPlayer player) => Get(player.GetName());

            public List<Tuple<string, T>> List()
            {
                return caches[typeof(T)].Select(pair => new Tuple<string, T>(pair.Key, (T)pair.Value)).ToList();
            }

            public T Get(string name)
            {
                if (!caches[typeof(T)].ContainsKey(name))
                    caches[typeof(T)].Add(name, Activator.CreateInstance(typeof(T)));
                return (T)caches[typeof(T)][name];
            }

            public void Save()
            {
                var obj = new JObject();
                foreach (var pair in caches[typeof(T)])
                    obj.Add(pair.Key, JsonConvert.SerializeObject(pair.Value));
                journals[typeof(T)].WriteAllText(GetFilename(), obj.ToString());
            }

            private string GetFilename()
            {
                return OnGetFilename?.Invoke(typeof(T)) ?? $@"tshock\{typeof(T).FullName}.json";
            }

            public void LoadOrDefault()
            {
                try
                {
                    caches[typeof(T)] = JsonConvert.DeserializeObject<Dictionary<string, T>>(journals[typeof(T)].ReadAllText(GetFilename()))
                        .ToDictionary(pair => pair.Key, pair => (object)pair.Value);
                }
                catch (Exception e)
                {
                    CompatHelper.Error("Failed to load from journal: " + e.ToString());
                    caches[typeof(T)] = new Dictionary<string, object>();
                }
            }

            public void Dispose()
            {
                Save();
            }
        }

        private static Dictionary<Type, IJournal> journals = new Dictionary<Type, IJournal>();
        private static Dictionary<Type, Dictionary<string, object>> caches = new Dictionary<Type, Dictionary<string, object>>();
        public static event Func<Type, string> OnGetFilename;

        public static Config<T> Get<T>() => new Config<T>();
        public static void SetJournal<T>(IJournal journal)
        {
            journals.Add(typeof(T), journal);
        }


    }
}
