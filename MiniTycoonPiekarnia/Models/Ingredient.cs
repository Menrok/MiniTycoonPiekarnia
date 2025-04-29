namespace MiniTycoonPiekarnia.Models;

public class Ingredient
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
    public decimal PurchasePrice { get; set; } = 0m;
}
