namespace MiniTycoonPiekarnia.Models;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
    public decimal SalePrice { get; set; } = 0m;
    public Dictionary<string, int> RequiredIngredients { get; set; } = new();
}
