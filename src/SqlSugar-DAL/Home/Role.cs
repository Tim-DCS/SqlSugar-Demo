using SqlSugar;

namespace SqlSugar_DAL.Home
{
    public class Role
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        public string RoleName { get; set; }

        public int Level { get; set; }

        public string Remark { get; set; }

        public long UserId { get; set; }
    }
}