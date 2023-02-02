using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyUtils.Commands;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RealPlayerAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method)]
public class MainAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PermissionsAttribute : Attribute
{
    public string perm;

    public PermissionsAttribute(string perm)
    {
        this.perm = perm;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AliasAttribute : Attribute
{
    public HashSet<string> alias;

    public AliasAttribute(params string[] aliases)
    {
        this.alias = new HashSet<string>(aliases);
    }
}