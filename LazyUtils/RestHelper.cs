using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Rests;
using TShockAPI;

namespace LazyUtils;

public static class RestHelper
{
    private static object ParseCommand(MethodInfo method, RestRequestArgs args)
    {
        var objs = new List<object>();
        foreach (var param in method.GetParameters())
        {
            if (objs.Count == 0)
            {
                if (param.ParameterType != typeof(RestRequestArgs))
                    return null;
                objs.Add(args);
                continue;
            }

            try
            {
                var reqparam = args.Parameters[param.Name];
                if (param.ParameterType == typeof(string))
                    objs.Add(reqparam);
                else
                    objs.Add(param.ParameterType
                        .GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null)
                        .Invoke(null, new object[] { reqparam }));
            }
            catch
            {
                return null;
            }
        }
            
        return method.Invoke(null, objs.ToArray());
    }

    internal static void Register(Type type, string name, LazyPlugin plugin)
    {
        foreach (var method in type.GetMethods())
            if (method.IsDefined(typeof(Permission)))
            {
                TShock.RestApi.Register(new SecureRestCommand($"/{name}/{method.Name}", args => ParseCommand(method, args),
                    method.GetCustomAttribute<Permission>().Name));
                TShock.Log.ConsoleInfo($"[{plugin.Name}] rest endpoint registered: /{name}/{method.Name}");
            }
    }
}