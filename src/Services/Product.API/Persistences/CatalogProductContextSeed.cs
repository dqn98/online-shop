﻿using Product.API.Entities;
using ILogger = Serilog.ILogger;

namespace Product.API.Persistences
{
    public class CatalogProductContextSeed
    {
        public static async Task SeedProductAsync(CatalogProductContext productContext, ILogger logger)
        {
            if (!productContext.Products.Any())
            {
                productContext.AddRange(getCatalogProducts());
                await productContext.SaveChangesAsync();
                logger.Information("Seeded data for Product DB associated with context {DbContextName}",
                    nameof(CatalogProductContext));
            }
        }

        private static IEnumerable<CatalogProduct> getCatalogProducts()
        {
            return new List<CatalogProduct>
            {
                new()
                {
                    No = "Lotus",
                    Name = "Esprit",
                    Summary = "Nondisplaced fracture of greater trochanter of right femur",
                    Description = "Nondisplaced fracture of greater trochanter of right femur",
                    Price = (decimal)177940.49
                },
                new()
                {
                    No = "Cadillac",
                    Name = "CTS",
                    Summary = "Carbuncle of trunk",
                    Description = "Carbuncle of trunk",
                    Price = (decimal)114728.21
                }
            };
        }
    }
}
