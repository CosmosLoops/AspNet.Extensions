using System.Collections.Generic;
using System.Data.Common;

namespace System.Data
{
    public static partial class DbConnectionExtensions
    {
        public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType, DbTransaction transaction)
            where T : new()
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
                    return reader.ToEntities<T>();
                }
            }
        }

        public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, Action<DbCommand> commandFactory) where T : new()
        {
            using (var command = @this.CreateCommand())
            {
                commandFactory(command);

                using (IDataReader reader = command.ExecuteReader())
                {
                    return reader.ToEntities<T>();
                }
            }
        }

        public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText) where T : new()
        {
            return @this.ExecuteEntities<T>(cmdText, null, CommandType.Text, null);
        }

        public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbTransaction transaction) where T : new()
        {
            return @this.ExecuteEntities<T>(cmdText, null, CommandType.Text, transaction);
        }

        public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, CommandType commandType) where T : new()
        {
            return @this.ExecuteEntities<T>(cmdText, null, commandType, null);
        }

        public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, CommandType commandType, DbTransaction transaction) where T : new()
        {
            return @this.ExecuteEntities<T>(cmdText, null, commandType, transaction);
        }

        public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbParameter[] parameters) where T : new()
        {
            return @this.ExecuteEntities<T>(cmdText, parameters, CommandType.Text, null);
        }

        public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbParameter[] parameters, DbTransaction transaction) where T : new()
        {
            return @this.ExecuteEntities<T>(cmdText, parameters, CommandType.Text, transaction);
        }

        public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType) where T : new()
        {
            return @this.ExecuteEntities<T>(cmdText, parameters, commandType, null);
        }


    }
}