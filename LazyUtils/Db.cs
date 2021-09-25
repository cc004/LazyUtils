﻿using LinqToDB.DataProvider.SQLite;
using System.Reflection;
using TShockAPI;

namespace LazyUtils
{
    public static class Db
    {
        public static PlayerConfigBase<T>.Context PlayerContext<T>(string tableName = null) where T : PlayerConfigBase<T> => PlayerConfigBase<T>.GetContext(tableName);
        
        public static ConfigBase<T>.Context Context<T>(string tableName = null) where T : ConfigBase<T> => ConfigBase<T>.GetContext(tableName);

        public static DisposableQuery<T> Get<T>(string name, string tableName = null) where T : PlayerConfigBase<T>
        {
            var context = PlayerConfigBase<T>.GetContext(tableName);
            return new DisposableQuery<T>(context.Get(name), context);
        }

        public static DisposableQuery<T> Get<T>(this TSPlayer player, string tableName = null) where T : PlayerConfigBase<T> => Get<T>(player.Account.Name, tableName);
    }
}
