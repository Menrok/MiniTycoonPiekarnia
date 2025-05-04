namespace MiniTycoonPiekarnia.Models.Products;

public static class RecipeList
{
    public static List<Recipe> GetInitialRecipes() => new()
    {
        new() { ProductName = "Chleb", IsUnlocked = true, UnlockCost = 0 },
        new() { ProductName = "Bułka", IsUnlocked = false, UnlockCost = 100 },
        new() { ProductName = "Ciasto", IsUnlocked = false, UnlockCost = 150 }
    };
}
