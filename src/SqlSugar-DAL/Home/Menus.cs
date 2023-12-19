using SqlSugar;

namespace SqlSugar_DAL.Home
{
    [SugarTable("Menus")]
    public class Menus
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        public string MenuName { get; set; }

        public string Level { get; set; }
    }
}