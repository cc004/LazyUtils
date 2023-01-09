using System.Collections.Generic;
using System;
using TShockAPI;

namespace LazyUtils.Commands;

internal partial class SingleCommand
{
    private delegate bool Parser(string arg, out object obj);

    private static bool TryParseBool(string arg, out object obj)
    {
        var result = bool.TryParse(arg, out bool t);
        obj = t;
        return result;
    }

    private static bool TryParseInt(string arg, out object obj)
    {
        var result = int.TryParse(arg, out int t);
        obj = t;
        return result;
    }

    private static bool TryParseLong(string arg, out object obj)
    {
        var result = long.TryParse(arg, out long t);
        obj = t;
        return result;
    }

    private static bool TryParseString(string arg, out object obj)
    {
        obj = arg;
        return true;
    }

    private static bool TryParseDateTime(string arg, out object obj)
    {
        var result = DateTime.TryParse(arg, out DateTime t);
        obj = t;
        var a = TryParseDateTime;
        return result;
    }

    private static bool TryParseTSPlayer(string arg, out object obj)
    {
        var maybe = new List<TSPlayer>();
        foreach (var plr in TShock.Players)
            if (plr is {Active: true})
            {
                if (plr.Name == arg)
                {
                    obj = plr;
                    return true;
                }

                if (plr.Name.StartsWith(arg))
                    maybe.Add(plr);
            }

        if (maybe.Count == 1)
        {
            obj = maybe[0];
            return true;
        }

        obj = null;
        return false;
    }

    private static readonly Dictionary<Type, Parser> parsers = new()
    {
        [typeof(bool)] = TryParseBool,
        [typeof(int)] = TryParseInt,
        [typeof(long)] = TryParseLong,
        [typeof(string)] = TryParseString,
        [typeof(DateTime)] = TryParseDateTime,
        [typeof(TSPlayer)] = TryParseTSPlayer
    };

    private static readonly Dictionary<Type, string> _friendlyName = new()
    {
        [typeof(bool)] = "bool",
        [typeof(int)] = "int",
        [typeof(long)] = "long",
        [typeof(string)] = "str",
        [typeof(DateTime)] = "date",
        [typeof(TSPlayer)] = "player"
    };
}