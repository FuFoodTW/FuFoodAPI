using FuFood.Models.Entities;
using FuFood.Models.Enums;
using FuFood.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Services;

public class ProductService(ProductRepository repository)
{
    public async Task<List<ProductCategory>> GetCategoriesAsync(Guid refrigeratorId)
    {
        var products = await repository.GetProductsByRefrigeratorAsync(refrigeratorId);

        var categories = products
            .Select(p => p.Categories)
            .Aggregate(ProductCategory.None, (acc, c) => acc | c);

        return Enum.GetValues<ProductCategory>()
            .Where(c => c != ProductCategory.None && categories.HasFlag(c))
            .ToList();
    }

    public async Task<List<Product>> GetProductsAsync(Guid refrigeratorId, ProductCategory? categories, int page,
        int pageSize, bool orderDescending)
    {
        IQueryable<Product> query = repository.GetProductsQueryByRefrigerator(refrigeratorId);

        if (categories.HasValue)
        {
            query = query.Where(p => (p.Categories & categories.Value) == categories.Value);
        }

        query = orderDescending
            ? query.OrderByDescending(p => p.CreatedAt)
            : query.OrderBy(p => p.CreatedAt);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}