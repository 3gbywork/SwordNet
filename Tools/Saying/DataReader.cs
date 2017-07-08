using CommonUtility.Logging;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using System.Threading.Tasks;

namespace Tools.Saying
{
    public class DataReader
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        static public List<string> GetSayingsFromUrl(string url, string xpath)
        {
            var sayings = new List<string>();
            try
            {
                var client = new HtmlWeb();
                var htmlDoc = client.Load(url);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);

                foreach (var node in nodes)
                {
                    sayings.Add(node.InnerText.Trim(new char[] { '\n', ' ' }));
                }
            }
            catch (Exception ex)
            {
                mLogger.Error($"Error while get sayings from:{url}, xpath:{xpath}, due to:{ex}");
            }

            return sayings;
        }

        #region DB相关操作
        static public int QuerySayingsCountFromDB(string connectionString)
        {
            string querySql = $"select count(*) from {SayingManager.SayingsTableName}";
            return QueryFromDBAsync(connectionString, querySql).Result;
        }

        static public int QuerySayingsIndexFromDB(string connectionString)
        {
            string querySql = $"select Id from {SayingManager.IndexTableName} order by Id desc limit 1";
            return QueryFromDBAsync(connectionString, querySql).Result;
        }

        static private async Task<int> QueryFromDBAsync(string connectionString, string commandText)
        {
            object result = DBNull.Value;
            var factory = SQLiteFactory.Instance;
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                var command = factory.CreateCommand();
                command.CommandText = commandText;
                command.Connection = connection;
                result = await command.ExecuteScalarAsync();
            }

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }

        static public async Task<string> GetSayingFromDBAsync(string connectionString, int id)
        {
            object result = DBNull.Value;
            var factory = SQLiteFactory.Instance;
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                var command = factory.CreateCommand();
                command.CommandText = $"select Saying from {SayingManager.SayingsTableName} where id=@ID limit 1";
                command.Parameters.Add(new SQLiteParameter("ID", id));
                command.Connection = connection;
                result = await command.ExecuteScalarAsync();
            }

            if (result != null && result != DBNull.Value)
            {
                return result.ToString();
            }
            return null;
        }

        static public async Task<bool> EnsureDBSchemaAsync(string connectionString)
        {
            object result = DBNull.Value;
            var factory = SQLiteFactory.Instance;
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                var command = factory.CreateCommand();
                command.CommandText =
$@"CREATE TABLE IF NOT EXISTS {SayingManager.SayingsTableName} (
{SayingManager.SayingsTableIdColumn} INTEGER NOT NULL,
{SayingManager.SayingsTableSayingColumn} TEXT NOT NULL,
PRIMARY KEY ({SayingManager.SayingsTableIdColumn}));
CREATE TABLE IF NOT EXISTS {SayingManager.IndexTableName} (
{SayingManager.IndexTableIdColumn} INTEGER NOT NULL,
PRIMARY KEY ({SayingManager.IndexTableIdColumn}));
SELECT count(*) FROM sqlite_master
WHERE type = 'table' AND 
(name = '{SayingManager.SayingsTableName}' OR name = '{SayingManager.IndexTableName}')";
                command.Connection = connection;
                result = await command.ExecuteScalarAsync();
            }
            if (result != null && result != DBNull.Value)
            {
                // 判断SayingsTableName和IndexTableName这两个表是否存在
                // 若都存在，则返回值为2
                return string.Equals(result.ToString(), "2");
            }
            return false;
        }
    }
    #endregion
}
