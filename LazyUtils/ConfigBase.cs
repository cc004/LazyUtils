using LinqToDB;
using LinqToDB.Data;
using TShockAPI;
using TShockAPI.DB;
using SqlType = TShockAPI.DB.SqlType;

namespace LazyUtils;

public abstract class ConfigBase<T> where T : ConfigBase<T>
{
    public class Context : DataConnection
    {
        public ITable<T> Config => this.GetTable<T>();

        private static string GetProvider()
        {
            switch (TShock.DB.GetSqlType())
            {
                case SqlType.Mysql: return ProviderName.MySql;
                case SqlType.Sqlite: return ProviderName.SQLiteMS;
                default:
                    return null;
            }
        }
        public Context(string tableName) : base(GetProvider(), ConfigBase<T>.ConnectionString)
        {
            this.CreateTable<T>(tableName, tableOptions: TableOptions.CreateIfNotExists);
        }
    }
        
    internal static Context GetContext(string tableName) => new (tableName);
    // ReSharper disable once StaticMemberInGenericType
    protected static string ConnectionString = TShock.DB.ConnectionString.Replace(",Version=3", "");
}