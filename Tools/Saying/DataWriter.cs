using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.Logging;

namespace Tools.Saying
{
    public class DataWriter
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task<int> UpdateSayingsToDBAsync(string connectionString, ICollection<string> sayings)
        {
            if (sayings != null && sayings.Count == 0)
            {
                return 0;
            }
            // values(),()
            string updateSql = $"insert into {SayingManager.SayingsTableName}({SayingManager.SayingsTableSayingColumn}) values";
            StringBuilder valuesBuilder = new StringBuilder();
            valuesBuilder.Append(updateSql);
            foreach (var saying in sayings)
            {
                valuesBuilder.Append($"('{saying}'),");
            }
            string sql = valuesBuilder.ToString().TrimEnd(',');

            object result = DBNull.Value;
            var factory = SQLiteFactory.Instance;
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                var command = factory.CreateCommand();
                command.CommandText = sql;
                command.Connection = connection;
                result = await command.ExecuteNonQueryAsync();
            }
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }

        public static async Task<bool> UpdateSayingsIndexToDBAsync(string connectionString, int index)
        {
            object result = DBNull.Value;
            var factory = SQLiteFactory.Instance;
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                var command = factory.CreateCommand();
                command.CommandText = $"insert into {SayingManager.IndexTableName}({SayingManager.IndexTableIdColumn}) values({index})";
                command.Connection = connection;
                result = await command.ExecuteNonQueryAsync();
            }
            if (result != null && result != DBNull.Value)
            {
                return string.Equals(result.ToString(), "1");
            }
            return false;
        }
    }
}
