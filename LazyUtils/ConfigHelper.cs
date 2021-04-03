using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazyUtils
{
    public static class ConfigHelper
    {
        private static Dictionary<Type, IJournal> journals = new Dictionary<Type, IJournal>();
        private static Dictionary<Type, Dictionary<string, object>> caches = new Dictionary<Type, Dictionary<string, object>>();
        public static event Func<Type, string> OnGetFilename;

        private static string GetFilename<T>()
        {
            return OnGetFilename?.Invoke(typeof(T)) ?? $@"tshock\{typeof(T).FullName}.json";
        }

        public static void SetJournal<T>(IJournal journal)
        {
            journals.Add(typeof(T), journal);
        }

        public static void Refresh<T>()
        {
            if (!caches.ContainsKey(typeof(T)))
            {
                caches.Add(typeof(T), new Dictionary<string, object>());
                LoadOrDefault<T>(false);
            }
            else
            {
                caches[typeof(T)] = new Dictionary<string, object>();
                LoadOrDefault<T>(true);
            }
        }

        public static List<Tuple<string, T>> List<T>()
        {
            return caches[typeof(T)].Select(pair => new Tuple<string, T>(pair.Key, (T)pair.Value)).ToList();
        }

        public static T Get<T>(string name)
        {
            if (!caches[typeof(T)].ContainsKey(name))
                caches[typeof(T)].Add(name, Activator.CreateInstance(typeof(T)));
            return (T)caches[typeof(T)][name];
        }

        public static void Save<T>()
        {
            var obj = new JObject();
            foreach (var pair in caches[typeof(T)])
                obj.Add(pair.Key, JsonConvert.SerializeObject(pair.Value));
            journals[typeof(T)].WriteAllText(GetFilename<T>(), obj.ToString());
        }

        public static void LoadOrDefault<T>(bool refreshing = false)
        {
            try
            {
                var obj = JObject.Parse(journals[typeof(T)].ReadAllText(GetFilename<T>()));
                foreach (var name in obj.Properties())
                    caches[typeof(T)].Add(name.Name, JsonConvert.DeserializeObject<T>((string)name.Value));
            }
            catch (Exception e)
            {
                CompatHelper.Error("Failed to load from journal: " + e.ToString());
                if (!refreshing)
                {
                    caches[typeof(T)] = new Dictionary<string, object>();
                }
            }
        }

    }
}
