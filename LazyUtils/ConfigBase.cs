using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.MySql;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.DataProvider.SqlServer;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace LazyUtils
{
    public abstract class ConfigBase<T> where T : ConfigBase<T>
    {
        public class Context : DataConnection
        {
            public ITable<T> Config => GetTable<T>();

            private static IDataProvider GetProvider()
            {
                switch (TShock.DB.GetSqlType())
                {
                    case SqlType.Mysql: return new MySqlDataProvider(string.Empty);
                    case SqlType.Sqlite: return new SQLiteDataProvider("SQLite.Classic");
                    default:
                        return null;
                }
            }
            public Context(string tableName) : base(GetProvider(), ConfigBase<T>.ConnectionString)
            {
                this.CreateTable<T>(tableName, tableOptions: TableOptions.CreateIfNotExists);
            }
        }
        
        internal static Context GetContext(string tableName) => new Context(tableName);
        // ReSharper disable once StaticMemberInGenericType
        protected static string ConnectionString = TShock.DB.ConnectionString.Replace(",Version=3", "");
    }
}
