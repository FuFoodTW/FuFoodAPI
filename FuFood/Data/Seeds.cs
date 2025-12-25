using System.Transactions;
using FuFood.Models.Entities;
using FuFood.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Data;

public static class Seeds
{
    public static async Task Run(AppDbContext dbContext)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await dbContext.UpsertRange(new User
                {
                    Name = "J",
                    LineId = "U7bf624699aeaafbf4912865b063a6e23",
                    ProfilePictureUrl =
                        "https://profile.line-scdn.net/0huz1iFvi7KlUMEDSb9GRVAnFVJDh7PiwddCU2Zn4YJGJ2c24ENiFiYSkZJGxxJG1UMCJgNyxCJjEoP2xTcC0AeyhVBh1XKW5lZC0VVHcWITxKV2tnMHYENCoSNiBkcDZ-SCUYY2BYCRJkWxlDMHImen9APDBRWTpjZiA"
                },
                new User
                {
                    Name = "楊學民",
                    LineId = "U78d1bb86f8410a52d375306295c06503",
                    ProfilePictureUrl =
                        "https://profile.line-scdn.net/0hrFgyP3w9LWlaADi3UlVSPmZFIwQtLishImZiXXtTdFl0Yj48bjVqDyhXcF5zZ2k9YmRhD39VJ153"
                },
                new User
                {
                    Name = "Karol Moroz",
                    LineId = "Ua067ec44bf8ca982d32ec217f9c084ae",
                    ProfilePictureUrl =
                        "https://profile.line-scdn.net/0h-FSDR_EPcmluPWa8k9ENPlJ4fAQZE3QhFg88CRtqeVhEDDQ4AVk4DRw5LF8QBTE9Vls_Dkw-fAkR"
                })
            .On(u => u.LineId)
            .NoUpdate()
            .RunAsync();

        // Create a default refrigerator for all users that don't have one yet
        await dbContext.Database.ExecuteSqlRawAsync(
            """
            insert into "Refrigerators" ("Id", "Name", "Colour", "CreatedById", "IsDefault", "CreatedAt", "UpdatedAt")
            select uuidv7(), '我的冰箱', 'blue', u."Id", true, now() at time zone 'utc', now() at time zone 'utc'
            from "Users" u
            left join "Refrigerators" r on r."CreatedById" = u."Id" and r."IsDefault" = true
            where r."Id" IS NULL;
            """);

        var milkId = Guid.Parse("019b5602-93f2-7c4b-a4ed-716fcb28597d");
        var sushiId = Guid.Parse("019b560a-4072-73dc-9278-7266d9fea210");

        await dbContext.Products
            .UpsertRange(new Product
                {
                    Id = milkId,
                    Name = "Kirkland Signature 鮮奶",
                    Quantity = 1.892706m,
                    Unit = UnitType.公升,
                    Categories = ProductCategory.Dairy
                },
                new Product
                {
                    Id = sushiId,
                    Name = "Kirkland Salmon Sushi",
                    Quantity = 500m,
                    Unit = UnitType.公克,
                    Categories = ProductCategory.Prepared | ProductCategory.Seafood
                }
            )
            .On(p => p.Id)
            .NoUpdate()
            .RunAsync();

        var jo = await dbContext.Users.FirstOrDefaultAsync(u => u.LineId == "U7bf624699aeaafbf4912865b063a6e23");
        var joFridge = await dbContext.Refrigerators.FirstOrDefaultAsync(r => r.CreatedById == jo!.Id);

        var transactionId = Guid.Parse("019b5610-1a1b-748d-966b-c16cd0d74c16");

        await dbContext.InventoryTransactions
            .Upsert(new InventoryTransaction
                {
                    Id = transactionId,
                    UserId = jo!.Id,
                    RefrigeratorId = joFridge!.Id,
                    FinalizedAt = DateTime.UtcNow
                }
            )
            .On(t => t.Id)
            .NoUpdate()
            .RunAsync();

        await dbContext.InventoryTransactionsItems
            .UpsertRange(new InventoryTransactionItem
                {
                    Id = Guid.Parse("019b5613-3a28-70fe-b74a-79be4368352b"),
                    InventoryTransactionId = transactionId,
                    ProductId = milkId,
                    Quantity = 2,
                    ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
                },
                new InventoryTransactionItem
                {
                    Id = Guid.Parse("019b5618-01a3-77be-97fd-0b99cc24004f"),
                    InventoryTransactionId = transactionId,
                    ProductId = sushiId,
                    Quantity = 1,
                    ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2))
                })
            .On(i => i.Id)
            .WhenMatched((fromDb, newRecord) => new InventoryTransactionItem
            {
                ExpirationDate = newRecord.ExpirationDate
            })
            .RunAsync();

        scope.Complete();
    }
}