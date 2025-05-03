namespace MiniTycoonPiekarnia.Models;

public static class ProductList
{
    public static List<Product> GetInitialProducts() => new()
    {
        new Product
        {
            Name = "Chleb",
            Quantity = 0,
            SalePrice = 7.5m,
            ExpValue = 3,
            ProducedAmount = 1,
            ProductionTimeSeconds = 60,
            RequiredIngredients = new Dictionary<string, decimal>
            {
                { "Mąka", 0.35m },
                { "Drożdże", 1.0m }
            }
        },
        new Product
        {
            Name = "Bułka",
            Quantity = 0,
            SalePrice = 1.2m,
            ExpValue = 1,
            ProducedAmount = 6,
            ProductionTimeSeconds = 45,
            RequiredIngredients = new Dictionary<string, decimal>
            {
                { "Mąka", 0.3m },
                { "Drożdże", 1.0m }
            }
        },
        new Product
        {
            Name = "Ciasto",
            Quantity = 0,
            SalePrice = 15.5m,
            ExpValue = 5,
            ProducedAmount = 1,
            ProductionTimeSeconds = 90,
            RequiredIngredients = new Dictionary<string, decimal>
            {
                { "Mąka", 0.25m },
                { "Jajka", 2.0m },
                { "Cukier", 0.2m }
            }
        }
    };
}
