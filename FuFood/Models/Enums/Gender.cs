using NpgsqlTypes;

namespace FuFood.Models.Enums;

public enum Gender
{
    [PgName("不透露")] NotSpecified, // 未填
    [PgName("女孩兒")] Female,
    [PgName("男孩紙")] Male,
    [PgName("無性別")] NonBinary,
    [PgName("其他")] Other
}