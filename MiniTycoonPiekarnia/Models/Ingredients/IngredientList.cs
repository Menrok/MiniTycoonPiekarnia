namespace MiniTycoonPiekarnia.Models.Ingredients;

public static class IngredientList
{
    public static List<Ingredient> GetInitialIngredients() => new()
    {
        new Ingredient { Name = "Mąka", Quantity = 0, PurchasePrice = 2.5m },
        new Ingredient { Name = "Cukier", Quantity = 0, PurchasePrice = 3.5m },
        new Ingredient { Name = "Drożdże", Quantity = 0, PurchasePrice = 1.2m },
        new Ingredient { Name = "Jajka", Quantity = 0, PurchasePrice = 1m }
    };
}