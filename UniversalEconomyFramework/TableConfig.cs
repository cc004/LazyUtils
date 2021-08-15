using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UniversalEconomyFramework
{
    public class TableConfig
    {
        private const string NEW = "economy";
        private const string CONFIG_FILE = "tshock/uef_table.json";

        private Dictionary<string, string> _table;

        private Dictionary<string, string> Table => _table = _table ?? LoadConfig();

        private void SaveConfig()
        {
            File.WriteAllText(CONFIG_FILE, JsonConvert.SerializeObject(Table));
        }

        private static Dictionary<string, string> LoadConfig()
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    File.ReadAllText(CONFIG_FILE));
            }
            catch (Exception e)
            {
                Console.WriteLine("config load error :" + e);
                throw;
            }
        }

        public static TableConfig TableName = new TableConfig();

        public string this[string framework]
        {
            get
            {
                if (Table.TryGetValue(framework, out var name)) return name;
                Table[framework] = NEW;
                SaveConfig();
                return NEW;
            }
            set => Table[framework] = value;
        }
    }
}
