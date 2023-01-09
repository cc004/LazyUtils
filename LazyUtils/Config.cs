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

namespace LazyUtils;

public abstract class Config<T> where T : Config<T>, new()
{
    private static T _instance;
    protected virtual string Filename => typeof(T).Namespace;
    private string FullFilename => Path.Combine(TShock.SavePath, Filename + ".json");

    private static T GetConfig()
    {
        var t = new T();
        var file = t.FullFilename;
        if (File.Exists(file))
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
        File.WriteAllText(file, JsonConvert.SerializeObject(t,Formatting.Indented));
        return t;
    }

    // .cctor is lazy load
    public static string Load()
    {
        GeneralHooks.ReloadEvent += _ => _instance = GetConfig();
        return Instance.Filename;
    }

    public static T Instance => _instance = _instance ?? GetConfig();
}