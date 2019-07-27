using System.Data.Common;

namespace System.Data
{
    public static partial class DbConnectionExtensions
    {
        public static void ExecuteNonQuery(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType, DbTransaction transaction)
        {
            using (var command = @this.CreateCommand())
            {
                command.CommandText = cmdText;
                command.CommandType = commandType;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                command.ExecuteNonQuery();
            }
        }

        public static void ExecuteNonQuery(this DbConnection @this, Action<DbCommand> commandFactory)
        {
            using (var command = @this.CreateCommand())
            {
                commandFactory(command);

                command.ExecuteNonQuery();
            }
        }

        public static void ExecuteNonQuery(this DbConnection @this, string cmdText)
        {
            @this.ExecuteNonQuery(cmdText, null, CommandType.Text, null);
        }

        public static void ExecuteNonQuery(this DbConnection @this, string cmdText, DbTransaction transaction)
        {
            @this.ExecuteNonQuery(cmdText, null, CommandType.Text, transaction);
        }

        public static void ExecuteNonQuery(this DbConnection @this, string cmdText, CommandType commandType)
        {
            @this.ExecuteNonQuery(cmdText, null, commandType, null);
        }

        public static void ExecuteNonQuery(this DbConnection @this, string cmdText, CommandType commandType, DbTransaction transaction)
        {
            @this.ExecuteNonQuery(cmdText, null, commandType, transaction);
        }

        public static void ExecuteNonQuery(this DbConnection @this, string cmdText, DbParameter[] parameters)
        {
            @this.ExecuteNonQuery(cmdText, parameters, CommandType.Text, null);
        }

        public static void ExecuteNonQuery(this DbConnection @this, string cmdText, DbParameter[] parameters, DbTransaction transaction)
        {
            @this.ExecuteNonQuery(cmdText, parameters, CommandType.Text, transaction);
        }

        public static void ExecuteNonQuery(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType)
        {
            @this.ExecuteNonQuery(cmdText, parameters, commandType, null);
        }
    }
}