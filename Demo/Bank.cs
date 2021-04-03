using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    [JsonObject]
    public class Bank
    {
        public int bank = 100;
    }

    public class Record
    {
        public static List<Record> records;
        public string name;
        public int npcID, D, netID;
        public DateTime createTime;
    }

    
}

