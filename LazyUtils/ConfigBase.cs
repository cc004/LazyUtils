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
                if (TShock.DB is SqliteConnection)
                    return new SQLiteDataProvider("");
                if (TShock.DB is MySqlConnection)
                    return new MySqlDataProvider("");
                return null;
            }

            public Context(string tableName) : base(GetProvider(), TShock.DB.ConnectionString)
            {
                this.CreateTable<T>(tableName, tableOptions: TableOptions.CreateIfNotExists);
            }
        }
        
        internal static Context GetContext(string tableName) => new Context(tableName);
    }
}
