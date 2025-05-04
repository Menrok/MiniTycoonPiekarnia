namespace MiniTycoonPiekarnia.Models.Products;

public class Recipe
{
    public string ProductName { get; set; } = "";
    public bool IsUnlocked { get; set; } = false;
    public decimal UnlockCost { get; set; } = 0;
}
