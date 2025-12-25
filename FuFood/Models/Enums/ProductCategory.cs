using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace FuFood.Models.Enums;

[Flags]
public enum ProductCategory
{
    None = 0,
    [PgName("乳製品")] Dairy = 1 << 0,
    [PgName("蔬果類")] Vegetable = 1 << 1,
    [PgName("水果類")] Fruit = 1 << 2,
    [PgName("肉品類")] Meat = 1 << 3,
    [PgName("海鮮類")] Seafood = 1 << 4,
    [PgName("冷凍食品")] Frozen = 1 << 5,
    [PgName("乳品飲料類")] Beverage = 1 << 6,
    [PgName("點心類")] Snack = 1 << 7,
    [PgName("熟食類")] Prepared = 1 << 8,
    [PgName("乾貨醬料類")] Condiment = 1 << 9
}