using System.ComponentModel.DataAnnotations;

namespace FuFood.Models.Enums;

[Flags]
public enum ProductCategory
{
    [Display(Name = "其他")] None = 0,
    [Display(Name = "乳製品")] Dairy = 1 << 0,
    [Display(Name = "蔬果類")] Vegetable = 1 << 1,
    [Display(Name = "水果類")] Fruit = 1 << 2,
    [Display(Name = "肉品類")] Meat = 1 << 3,
    [Display(Name = "海鮮類")] Seafood = 1 << 4,
    [Display(Name = "冷凍食品")] Frozen = 1 << 5,
    [Display(Name = "乳品飲料類")] Beverage = 1 << 6,
    [Display(Name = "點心類")] Snack = 1 << 7,
    [Display(Name = "熟食類")] Prepared = 1 << 8,
    [Display(Name = "乾貨醬料類")] Condiment = 1 << 9
}