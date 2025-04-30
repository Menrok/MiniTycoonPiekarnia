namespace MiniTycoonPiekarnia.Models;

public static class ProductList
{
    public static List<Product> GetInitialProducts() => new()
    {
        new Product
        {
            Name = "Chleb",
            Quantity = 0,
            SalePrice = 12,
            RequiredIngredients = new Dictionary<string, int>
            {
                { "Mąka", 2 },
                { "Drożdże", 1 }
            }
        },
        new Product
        {
            Name = "Bułka",
            Quantity = 0,
            SalePrice = 8,
            RequiredIngredients = new Dictionary<string, int>
            {
                { "Mąka", 1 },
                { "Drożdże", 1 }
            }
        },
        new Product
        {
            Name = "Ciasto",
            Quantity = 0,
            SalePrice = 19,
            RequiredIngredients = new Dictionary<string, int>
            {
                { "Mąka", 2 },
                { "Jajka", 2 },
                { "Cukier", 2 }
            }
        }
    };
}
