﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TShockAPI;

namespace LazyUtils
{
    public abstract class Config<T> where T : Config<T>, new()
    {
        private static T _instance;
        protected abstract string Filename { get; }
        private static T GetConfig()
        {
            var t = new T();
            var file = Path.Combine(TShock.SavePath, t.Filename);
            if (File.Exists(file)) return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
            File.WriteAllText(file, JsonConvert.SerializeObject(t));
            return t;
        }

        public static T Instance => _instance = _instance ?? GetConfig();
    }
}
