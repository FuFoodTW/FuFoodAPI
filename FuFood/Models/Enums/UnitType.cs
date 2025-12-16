using NpgsqlTypes;

namespace FuFood.Models.Enums;

public enum UnitType
{
    // 基本
    [PgName("個")] 個,
    [PgName("件")] 件,
    [PgName("包")] 包,
    [PgName("盒")] 盒,
    [PgName("箱")] 箱,
    [PgName("袋")] 袋,
    [PgName("條")] 條,
    [PgName("片")] 片,

    // 飲品與容器
    [PgName("杯")] 杯,
    [PgName("瓶")] 瓶,
    [PgName("罐")] 罐,
    [PgName("壺")] 壺,
    [PgName("桶")] 桶,
    [PgName("袋裝")] 袋裝,

    // 重量
    [PgName("公斤")] 公斤,
    [PgName("公克")] 公克,
    [PgName("毫克")] 毫克,

    // 容量
    [PgName("公升")] 公升,
    [PgName("毫升")] 毫升,

    //其他
    [PgName("把")] 把,
    [PgName("塊")] 塊,
    [PgName("粒")] 粒,
    [PgName("顆")] 顆
}