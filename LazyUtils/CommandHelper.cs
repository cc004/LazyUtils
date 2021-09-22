using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TShockAPI;

namespace LazyUtils
{
    public class RealPlayerAttribute : Attribute
    {

    }

    public class AliasAttribute : Attribute
    {
        public HashSet<string> alias;

        public AliasAttribute(params string[] aliases)
        {
            alias = new HashSet<string>(aliases);
        }
    }

    public static class CommandHelper
    {
        private const string noperm = "没有权限";
        private const string mustreal = "你必须在游戏内使用该指令";

        private static bool ParseCommand(Type t, CommandArgs arg, List<string> args)
        {
            if (args.Count > 0)
            {
                foreach (var type in t.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
                {
                    if (args[0] == type.Name.ToLower() && ParseCommand(type, arg, args.Skip(1).ToList()))
                    {
                        return true;
                    }
                }
            }

            foreach (var method in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var lst = args;
                if (method.Name != "Main")
                {
                    var cmd = args.FirstOrDefault();
                    if (method.IsDefined(typeof(AliasAttribute)) &&
                        method.GetCustomAttribute<AliasAttribute>().alias.Contains(cmd) || cmd == method.Name.ToLower())
                    {
                        lst = args.Skip(1).ToList();
                    }
                    else continue;
                }

                List<object> objs = new List<object>();

                bool error = false;
                var index = 0;

                foreach (var param in method.GetParameters())
                {
                    if (objs.Count == 0)
                    {
                        if (param.ParameterType != typeof(CommandArgs))
                        {
                            error = true;
                            break;
                        }
                        objs.Add(arg);
                        continue;
                    }

                    try
                    {
                        if (param.ParameterType == typeof(string))
                            objs.Add(lst[index++]);
                        else
                            objs.Add(param.ParameterType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null)
                                .Invoke(null, new object[] { lst[index++] }));
                    }
                    catch
                    {
                        error = true;
                        break;
                    }
                }

                if (error || index != lst.Count) continue;

                if (method.IsDefined(typeof(Permission)) && !arg.Player.HasPermission(method.GetCustomAttribute<Permission>().Name))
                {
                    arg.Player.SendErrorMessage(noperm);
                    return true;
                }

                if (method.IsDefined(typeof(RealPlayerAttribute)) && !arg.Player.RealPlayer)
                {
                    arg.Player.SendErrorMessage(mustreal);
                    return true;
                }
                method.Invoke(null, objs.ToArray());
                return true;
            }
            var method2 = t.GetMethod("Default", BindingFlags.Public | BindingFlags.Static);
            if (method2 == null) return false;
            if (method2.IsDefined(typeof(Permission)) && !arg.Player.HasPermission(method2.GetCustomAttribute<Permission>().Name))
            {
                arg.Player.SendErrorMessage(noperm);
                return true;
            }

            method2.Invoke(null, new object[] { arg });
            return true;
        }

        public static void Register<T>(params string[] names)
        {
            Commands.ChatCommands.Add(new Command((args) => ParseCommand(typeof(T), args, args.Parameters), names.Length > 0 ? names : new string[] { typeof(T).Name.ToLower() }));
        }
    }
}
