using LinqToDB.DataProvider.SQLite;
using System.Reflection;
using TShockAPI;

namespace LazyUtils
{
    public static class Db
    {
        internal class DbConnection { }
        internal class DbDataReader { }
        internal class DbParameter { }
        internal class DbCommand { }
        internal class DbTransaction { }

        static Db()
        {
            var obj = typeof(SQLiteProviderAdapter).GetMethod("CreateAdapter", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { Assembly.GetExecutingAssembly().FullName, "LazyUtils", "Db+Db" });
            typeof(SQLiteProviderAdapter).GetField("_microsoftDataSQLite", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, obj);
        }

        public static PlayerConfigBase<T>.Context PlayerContext<T>(string tableName = null) where T : PlayerConfigBase<T> => PlayerConfigBase<T>.GetContext(tableName);
        
        public static ConfigBase<T>.Context Context<T>(string tableName = null) where T : ConfigBase<T> => ConfigBase<T>.GetContext(tableName);

        public static DisposableQuery<T> Get<T>(string name, string tableName = null) where T : PlayerConfigBase<T>
        {
            var context = PlayerConfigBase<T>.GetContext(tableName);
            return new DisposableQuery<T>(context.Get(name), context);
        }

        public static DisposableQuery<T> Get<T>(this TSPlayer player, string tableName = null) where T : PlayerConfigBase<T> => Get<T>(player.GetName(), tableName);
    }
}
