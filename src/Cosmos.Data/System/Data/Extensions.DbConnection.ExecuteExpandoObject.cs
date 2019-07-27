using System.Data.Common;

namespace System.Data
{
    public static partial class DbConnectionExtensions
    {
        public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType, DbTransaction transaction)
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

                using (IDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    return reader.ToExpandoObject();
                }
            }
        }

        public static dynamic ExecuteExpandoObject(this DbConnection @this, Action<DbCommand> commandFactory)
        {
            using (var command = @this.CreateCommand())
            {
                commandFactory(command);

                using (IDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    return reader.ToExpandoObject();
                }
            }
        }

        public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText)
        {
            return @this.ExecuteExpandoObject(cmdText, null, CommandType.Text, null);
        }

        public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbTransaction transaction)
        {
            return @this.ExecuteExpandoObject(cmdText, null, CommandType.Text, transaction);
        }

        public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, CommandType commandType)
        {
            return @this.ExecuteExpandoObject(cmdText, null, commandType, null);
        }

        public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, CommandType commandType, DbTransaction transaction)
        {
            return @this.ExecuteExpandoObject(cmdText, null, commandType, transaction);
        }

        public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbParameter[] parameters)
        {
            return @this.ExecuteExpandoObject(cmdText, parameters, CommandType.Text, null);
        }

        public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbParameter[] parameters, DbTransaction transaction)
        {
            return @this.ExecuteExpandoObject(cmdText, parameters, CommandType.Text, transaction);
        }

        public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType)
        {
            return @this.ExecuteExpandoObject(cmdText, parameters, commandType, null);
        }
    }
}