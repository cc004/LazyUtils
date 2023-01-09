using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace LazyUtils.JsonData;

[JsonObject]
public class ItemData
{
    public int type, stack;
    public byte prefix;
    public bool favorited;
    public static implicit operator ItemData(Item item)
    {
        return new ItemData
        {
            type = item.type,
            stack = item.stack,
            prefix = item.prefix,
            favorited = item.favorited
        };
    }

    public static implicit operator Item(ItemData data)
    {
        var item = new Item();
        item.SetDefaults(data.type);
        item.stack = data.stack;
        item.prefix = data.prefix;
        item.favorited = data.favorited;
        return item;
    }
}