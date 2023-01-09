using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UniversalEconomyFramework;

public sealed class EcoConfig
{
    public string table;
    public float multiplier;
}

internal class TableConfig
{
    private const string NEW = "economy";
    private const string CONFIG_FILE = "tshock/uef_table.json";

    private Dictionary<string, EcoConfig> _table;

    private Dictionary<string, EcoConfig> _Table => _table = _table ?? LoadConfig();

    private void SaveConfig()
    {
        File.WriteAllText(CONFIG_FILE, JsonConvert.SerializeObject(_Table));
    }

    private static Dictionary<string, EcoConfig> LoadConfig()
    {
        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, EcoConfig>>(
                File.ReadAllText(CONFIG_FILE));
        }
        catch (Exception e)
        {
            Console.WriteLine("config load error :" + e);
            throw;
        }
    }

    public static TableConfig Table = new TableConfig();

    public EcoConfig this[string framework]
    {
        get
        {
            if (_Table.TryGetValue(framework, out var name)) return name;
            Table[framework] = new EcoConfig(){table = NEW, multiplier = 1f};
            SaveConfig();
            return Table[framework];
        }
        set => Table[framework] = value;
    }
}