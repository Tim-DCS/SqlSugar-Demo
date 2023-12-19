using SqlSugar;
using SqlSugar_DAL.Home;
using SqlSugar_ORM.DbContexts;
using System;

namespace SqlSugar_ORM.Extensions
{
    public class SqlSugarHandel
    {
        private static DBContext dBContext;
        private static readonly object _dbLock = new object();

        public static DBContext CreateDBContext()
        {
            if (dBContext == null)
            {
                lock (_dbLock)
                {
                    if (dBContext == null)
                    {
                        dBContext = new DBContext();
                    }
                }
            }

            return dBContext;
        }

        public static void CreateTable()
        {
            try
            {
                #region The statements that create the database and tables are executed only once

                //dBContext.SqlSugarScope().CodeFirst.InitTables(typeof(User));
                //dBContext.SqlSugarScope().CodeFirst.InitTables(typeof(Role));
                dBContext.SqlSugarScope().CodeFirst.InitTables(typeof(Menus));

                #endregion The statements that create the database and tables are executed only once

                dBContext.SqlSugarScope().Aop.OnLogExecuted = (sql, pra) =>
                {
                    Console.WriteLine("******************************************************************");
                    Console.WriteLine($"T-Sql：{sql}");
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}