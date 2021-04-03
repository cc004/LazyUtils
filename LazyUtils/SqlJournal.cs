using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace LazyUtils
{
    public class SqlJournal : IJournal
	{
		private IDbConnection database;

        public SqlJournal()
        {
			database = TShock.DB;
			SqlTable table = new SqlTable("Files", new SqlColumn[]
			{
				new SqlColumn("Path", MySqlDbType.Text),
				new SqlColumn("Content", MySqlDbType.Text),
			});
			IQueryBuilder provider;
			if (database.GetSqlType() != SqlType.Sqlite)
			{
				IQueryBuilder queryBuilder = new MysqlQueryCreator();
				provider = queryBuilder;
			}
			else
			{
				IQueryBuilder queryBuilder = new SqliteQueryCreator();
				provider = queryBuilder;
			}
			SqlTableCreator sqlTableCreator = new SqlTableCreator(database, provider);
			sqlTableCreator.EnsureTableStructure(table);
		}

		public string ReadAllText(string path)
		{
			try
			{
				using (QueryResult queryResult = database.QueryReader("SELECT * FROM Files WHERE Path=@0", new object[]
				{
					path
				}))
					if (queryResult.Read())
						return queryResult.Get<string>("Content");
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return null;
		}
		public void WriteAllText(string path, string contents)
		{
			try
			{
				if (ReadAllText(path) != null)
				{
					database.Query("UPDATE Files SET Content=@0 WHERE Path = @1", new object[]
					{
						contents, path
					});
					return;
				}
				else
				{
					database.Query("INSERT INTO Files (Path, Content) VALUES (@0, @1);", new object[] { path, contents });
					return;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
		}
	}
}
