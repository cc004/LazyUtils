using LinqToDB;
using LinqToDB.Mapping;
using System;
using System.Linq;
using TShockAPI;

namespace LazyUtils;

public abstract class PlayerConfigBase<T> : ConfigBase<T> where T : PlayerConfigBase<T>
{
    public new class Context : ConfigBase<T>.Context
    {
        public IQueryable<T> Get(string name)
        {
            if (!this.Config.Any(t => t.name == name))
            {
                var t = Activator.CreateInstance<T>();
                t.name = name;
                this.Insert(t);
            }

            return this.Config.Where(t => t.name == name);
        }
        public IQueryable<T> Get(TSPlayer player)
        {
            return this.Get(player.Account.Name);
        }

        public Context(string tableName) : base(tableName)
        {
        }
    }

    internal new static Context GetContext(string tableName)
    {
        return new Context(tableName);
    }

    [PrimaryKey, NotNull]
    public string name { get; set; }
}