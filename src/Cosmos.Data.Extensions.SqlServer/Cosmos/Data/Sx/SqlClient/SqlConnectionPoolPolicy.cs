/*
 * A copy of https://github.com/2881099/dng.Mssql/blob/master/Mssql/SqlConnectionPool.cs
 * Author: 2881099
 * MIT
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cosmos.Data.Core.Pools;
using Cosmos.Disposables.ObjectPools;

namespace Cosmos.Data.Sx.SqlClient
{
    /// <summary>
    /// SqlConnection pool policy
    /// </summary>
    public class SqlConnectionPoolPolicy : IConnectionPolicy<SqlConnection>
    {
        // ReSharper disable once InconsistentNaming
        internal SqlConnectionPool _pool;

        /// <inheritdoc />
        public string Name { get; set; } = "Microsoft SQLServer SqlConnection Object Pool";

        /// <inheritdoc />
        public int PoolSize { get; set; } = 100;

        /// <inheritdoc />
        public TimeSpan SyncGetTimeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <inheritdoc />
        public TimeSpan IdleTimeout { get; set; } = TimeSpan.Zero;

        /// <inheritdoc />
        public int AsyncGetCapacity { get; set; } = 10000;

        /// <inheritdoc />
        public bool IsThrowGetTimeoutException { get; set; } = true;

        /// <inheritdoc />
        public bool IsAutoDisposeWithSystem { get; set; }

        /// <inheritdoc />
        public int CheckAvailableInterval { get; set; } = 5;

        private string _connectionString;

        /// <summary>
        /// Gets or sets connection string
        /// </summary>
        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value ?? "";

                var m = Regex.Match(_connectionString, @"Max\s*pool\s*size\s*=\s*(\d+)", RegexOptions.IgnoreCase);

                if (m.Success == false
                 || int.TryParse(m.Groups[1].Value, out var poolSize) == false
                 || poolSize <= 0)
                    poolSize = 100;

                PoolSize = poolSize;

                var initConnArray = new ObjectOut<SqlConnection>[poolSize];

                for (var a = 0; a < poolSize; a++)
                {
                    try
                    {
                        initConnArray[a] = _pool.Get();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                foreach (var conn in initConnArray)
                    _pool.Return(conn);
            }
        }

        /// <inheritdoc />
        public bool OnCheckAvailable(ObjectOut<SqlConnection> obj)
        {
            if (obj.Value.State == ConnectionState.Closed)
                obj.Value.Open();
            var cmd = obj.Value.CreateCommand();
            cmd.CommandText = "select 1";
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <inheritdoc />
        public SqlConnection OnCreate() => new SqlConnection(_connectionString);

        /// <inheritdoc />
        public void OnDestroy(SqlConnection obj)
        {
            if (obj.State != ConnectionState.Closed)
                obj.Close();
            obj.Dispose();
        }

        /// <inheritdoc />
        public void OnGet(ObjectOut<SqlConnection> obj)
        {
            if (_pool.IsAvailable)
            {
                if (obj.Value.State != ConnectionState.Open || DateTime.Now.Subtract(obj.LastReturnTime).TotalSeconds > 60 && obj.Value.Ping() == false)
                {
                    try
                    {
                        obj.Value.Open();
                    }
                    catch (Exception ex)
                    {
                        if (_pool.SetUnavailable(ex))
                            throw new Exception($"【{Name}】状态不可用，等待后台检查程序恢复方可使用。{ex.Message}");
                    }
                }
            }
        }

        /// <inheritdoc />
        public async Task OnGetAsync(ObjectOut<SqlConnection> obj)
        {
            if (_pool.IsAvailable)
            {
                if (obj.Value.State != ConnectionState.Open || DateTime.Now.Subtract(obj.LastReturnTime).TotalSeconds > 60 && obj.Value.Ping() == false)
                {
                    try
                    {
                        await obj.Value.OpenAsync();
                    }
                    catch (Exception ex)
                    {
                        if (_pool.SetUnavailable(ex))
                            throw new Exception($"【{Name}】状态不可用，等待后台检查程序恢复方可使用。{ex.Message}");
                    }
                }
            }
        }

        /// <inheritdoc />
        public void OnGetTimeout() { }

        /// <inheritdoc />
        public void OnReturn(ObjectOut<SqlConnection> obj)
        {
            if (obj.Value.State != ConnectionState.Closed)
            {
                try
                {
                    obj.Value.Close();
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <inheritdoc />
        public void OnAvailable() => _pool.AvailableHandler?.Invoke();

        /// <inheritdoc />
        public void OnUnavailable() => _pool.UnavailableHandler?.Invoke();
    }
}