namespace MiniTycoonPiekarnia.Models;

public class Bakery
{
    public List<Ingredient> Ingredients { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public decimal Money { get; set; } = 1000m;
    public int BakeryLevel { get; set; } = 1;
    public int CustomerSatisfaction { get; set; } = 100;
    public List<Tile> Tiles { get; set; } = new();
    public int MapSize { get; set; } = 5;
    public List<Customer> CustomersWaiting { get; set; } = new();
    public List<Customer> CustomersHistory { get; set; } = new();


    public int MaxProductCapacity => 50 * Tiles.Count(t => t.Building == BuildingType.Shelf);
    public int MaxIngredientCapacity => 50 * Tiles.Count(t => t.Building == BuildingType.Shelf);
    public int CurrentProductQuantity => Products.Sum(p => p.Quantity);
    public int CurrentIngredientQuantity => Ingredients.Sum(i => i.Quantity);

}
