using SqlSugar;
using System.Collections.Generic;

namespace SqlSugar_DAL.Home
{
    [SugarTable("User")]
    public class User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        public string UserName { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public bool IsDeleted { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<Role> Roles { get; set; }
    }
}