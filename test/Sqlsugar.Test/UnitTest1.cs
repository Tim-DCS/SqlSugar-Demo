using DCS_CommonTools.CommonHandle.Extensions;
using NUnit.Framework;
using SqlSugar;
using SqlSugar_DAL.Home;
using SqlSugar_ORM.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sqlsugar.Test
{
    public class Tests
    {
        private SqlSugarScope sqlScope;

        [SetUp]
        public void Setup()

        {
            var dbContext = SqlSugarHandel.CreateDBContext();
            sqlScope = dbContext.SqlSugarScope();
        }

        [Test]
        public void CreateTable()
        {
            SqlSugarHandel.CreateTable();
        }

        [Test]
        public void InsertInfo()
        {
            List<User> users = new List<User>();
            for (int i = 0; i < 10; i++)
            {
                users.Add(new User
                {
                    UserName = "user1" + i,
                    Mobile = "1888888888" + i,
                    Email = "user" + i + "@qq.com",
                    Account = "user" + i,
                    IsDeleted = false,
                    Password = "user" + i,
                });
            }

            sqlScope.Insertable(users).ExecuteCommand();
        }

        [Test]
        public void UpdateInfo()
        {
            List<User> users = sqlScope.Queryable<User>().Where(m => m.IsDeleted == false).ToList();
            foreach (var item in users)
            {
                item.Account = item.Account.Replace("test", "Account");
                item.Password = item.Password.Replace("Account", "Password");
                item.Email = "test_" + item.Id + "@qq.com";
            }

            // sqlScope.Updateable(users).ExecuteCommand();
            sqlScope.Updateable(users).UpdateColumns(c => new { c.Account, c.Password }).ExecuteCommand();
        }

        [Test]
        public void GetInfo()
        {
            var userInfo = sqlScope.Queryable<User>().Where(m => m.IsDeleted == false).First();
            CurrentLogHelper.WriteLog($"userInfo: {userInfo.ToJson()}", new[] { "Executings" });
        }

        [Test]
        public void GetList()
        {
            var userInfos = sqlScope.Queryable<User>().Where(m => m.IsDeleted == false).ToList();
            CurrentLogHelper.WriteLog($"userInfos: {userInfos.ToJson()}", new[] { "Executings" });
        }

        [Test]
        public void GetListWithSub()
        {
            // 关联子表
            var userList = sqlScope.Queryable<User>().Where(m => m.IsDeleted == false)
                .Mapper(m => m.Roles, m => m.Id, m => m.Roles.First().UserId)
                .ToList();
            CurrentLogHelper.WriteLog($"userInfos: {userList.ToJson()}", new[] { "Executings" });
        }

        [Test]
        public void GetPages()
        {
            int pageIndex = 1;
            int pageSize = 5;
            int totalCount = 0;
            var userInfos = sqlScope.Queryable<User>().Where(m => m.IsDeleted == false)
                .OrderBy(m => m.Id)
                .ToPageList(pageIndex, pageSize, ref totalCount);
            CurrentLogHelper.WriteLog($"userInfos Page: {userInfos.ToJson()}", new[] { "Executings" });
        }

        [Test]
        public void GetListWithSql()
        {
            string sql = "select * from [User] where IsDeleted = @IsDeleted";
            SugarParameter[] parameters = {
                new SugarParameter("@IsDeleted", 0),
            };

            var result = sqlScope.Ado.GetDataTable(sql, parameters);
            if (result.Rows.Count > 0)
            {
                CurrentLogHelper.WriteLog($"userInfos By Sql: {result.ToJson()}", new[] { "Executings" });
            }
        }

        [Test]
        public void QueryMuch()
        {
            // 多表联查
            var query = sqlScope.Queryable<User, Role>((u, r) => new object[] {
                JoinType.Left, u.Id == r.UserId,
            }).With(SqlWith.NoLock);

            var list = query.Where((u, r) => !u.IsDeleted && r.UserId == 1).Select((u, r) => u).ToList();
            CurrentLogHelper.WriteLog($"QueryMuch: {list.ToJson()}", new[] { "Executings" });
        }

        [Test]
        public void DeleteInfo()
        {
            sqlScope.Deleteable<User>().Where(m => m.Id == 6).ExecuteCommand();
        }
    }
}