namespace MiniTycoonPiekarnia.Models;

public class Bakery
{
    public List<Ingredient> Ingredients { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public decimal Money { get; set; } = 100m;
    public int BakeryLevel { get; set; } = 1;
    public int CustomerSatisfaction { get; set; } = 100;
    public List<Tile> Tiles { get; set; } = new();
    public int MapSize { get; set; } = 5;
}
