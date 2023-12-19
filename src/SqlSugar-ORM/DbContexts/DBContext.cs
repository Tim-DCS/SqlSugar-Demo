using SqlSugar;
using SqlSugar_ORM.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SqlSugar_ORM.DbContexts
{
    public class DBContext
    {
        /// <summary>
        /// DbType。
        /// </summary>
        public DbType DbType { get; set; }

        private SqlSugarScope _sugarScope;

        public DBContext()
        {
            if (_sugarScope == null)
                Init();
        }

        public SqlSugarScope SqlSugarScope()
        {
            if (_sugarScope == null)
                Init();

            return _sugarScope;
        }

        public void Init()
        {
            var settings = ConfigurationManager.ConnectionStrings;
            int settingCount = settings.Count;
            List<ConnectionConfig> configs = new List<ConnectionConfig>();
            for (int i = 0; i < settingCount; i++)
            {
                var connSetting = settings[i];
                DbType = GetSugarDbType(connSetting);
                configs.Add(new ConnectionConfig
                {
                    ConfigId = connSetting.Name,
                    DbType = this.DbType,
                    ConnectionString = connSetting.ConnectionString,
                    InitKeyType = InitKeyType.Attribute, //Read primary key and autoincrement column information from attributes
                    IsAutoCloseConnection = true,
                    MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoRemoveDataCache = true,
                    }
                });
            }

            _sugarScope = new SqlSugarScope(configs, db =>
            {
                db.Ado.IsDisableMasterSlaveSeparation = true;
                foreach (var item in configs)
                {
                    db.GetConnectionScope(item.ConfigId).Aop.OnLogExecuting = (sql, pars) => LogExecuting(sql, pars);
                    db.GetConnectionScope(item.ConfigId).Aop.OnLogExecuted = (sql, pars) => LogExecuted(sql, pars);
                }
            });
        }

        /// <summary>
        /// Determine the database type based on the providerName of the link string
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        private DbType GetSugarDbType(ConnectionStringSettings setting)
        {
            DbType dbType = DbType.SqlServer; //Default Value
            var providerName = setting.ProviderName;
            if (providerName != null)
            {
                // DB providerName：SqlClient MySqlClient SQLite OracleManaged/OracleClient Npgsql
                if (providerName.EndsWith(".SqlClient", StringComparison.OrdinalIgnoreCase))
                {
                    dbType = DbType.SqlServer;
                }
                else if (providerName.EndsWith(".MySqlClient", StringComparison.OrdinalIgnoreCase))
                {
                    dbType = DbType.MySql;
                }
                else if (providerName.EndsWith(".SQLite", StringComparison.OrdinalIgnoreCase))
                {
                    dbType = DbType.Sqlite;
                }
                else if (providerName.EndsWith("OracleManaged", StringComparison.OrdinalIgnoreCase))
                {
                    dbType = DbType.Oracle;
                }
                else if (providerName.EndsWith(".OracleClient", StringComparison.OrdinalIgnoreCase))
                {
                    dbType = DbType.Oracle;
                }
                else if (providerName.EndsWith("Npgsql", StringComparison.OrdinalIgnoreCase))
                {
                    dbType = DbType.PostgreSQL;
                }
                else if (providerName.EndsWith("Dm", StringComparison.OrdinalIgnoreCase))
                {
                    dbType = DbType.Dm;
                }
            }
            return dbType;
        }

        /// <summary>
        /// Arguments Concatenate strings
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        private string GetParams(SugarParameter[] pars)
        {
            return pars.Aggregate("", (current, p) => current + $"{p.ParameterName}:{p.Value}\n");
        }

        private void LogExecuting(string sql, SugarParameter[] pars)
        {
            System.Diagnostics.Debug.WriteLine("LogExecuting: " + sql);
            if (ConfigurationManager.AppSettings["IsSqlAOP"].ToLower() == "true")
            {
                CurrentLogHelper.WriteSqlLog($"SqlLog{DateTime.Now:yyyy-MM-dd}",
                    new[] { $"【Sql Executeting】{DateTime.Now.ToString("G")} \n【SQL parameters inquiry】：\n" + GetParams(pars), "【T-SQL】" + sql });
            }
        }

        private void LogExecuted(string sql, SugarParameter[] pars)
        {
            System.Diagnostics.Debug.WriteLine("LogExecuted: " + sql);
            if (ConfigurationManager.AppSettings["IsSqlAOP"].ToLower() == "true")
            {
                CurrentLogHelper.WriteSqlLog($"SqlLog{DateTime.Now:yyyy-MM-dd}",
                    new[] { $"【Sql Executed】 {DateTime.Now.ToString("G")}" });
            }
        }
    }
}