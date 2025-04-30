namespace MiniTycoonPiekarnia.Models;

public static class IngredientList
{
    public static List<Ingredient> GetInitialIngredients() => new()
    {
        new Ingredient { Name = "Mąka", Quantity = 0, PurchasePrice = 2m },
        new Ingredient { Name = "Cukier", Quantity = 0, PurchasePrice = 3m },
        new Ingredient { Name = "Drożdże", Quantity = 0, PurchasePrice = 4m },
        new Ingredient { Name = "Jajka", Quantity = 0, PurchasePrice = 2.5m }
    };
}