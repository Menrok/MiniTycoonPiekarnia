namespace MiniTycoonPiekarnia.Models;

public class Bakery
{
    public List<Ingredient> Ingredients { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public decimal Money { get; set; } = 1000m;
    public int BakeryLevel { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int ExperienceToNextLevel => 100 + (BakeryLevel * 50);
    public int CustomerSatisfaction { get; set; } = 100;
    public List<Tile> Tiles { get; set; } = new();
    public int MapWidth { get; set; } = 3;
    public int MapHeight { get; set; } = 3;
    public List<Customer> CustomersWaiting { get; set; } = new();
    public List<Customer> CustomersHistory { get; set; } = new();
    public List<ProductionTask> ActiveProductions { get; set; } = new();


    public int MaxIngredientCapacity => 50 * Tiles.Count(t => t.Building == BuildingType.Shelf);
    public int MaxProductCapacity => 50 * Tiles.Count(t => t.Building == BuildingType.Website);
    public int CurrentProductQuantity => Products.Sum(p => p.Quantity);
    public decimal CurrentIngredientQuantity => Ingredients.Sum(i => i.Quantity);

    public void AddExperience(int amount)
    {
        Experience += amount;
        while (Experience >= ExperienceToNextLevel)
        {
            Experience -= ExperienceToNextLevel;
            BakeryLevel++;
        }
    }

    public int GetExpForCustomerOrder(Customer customer)
    {
        int exp = 0;
        foreach (var kvp in customer.RequestedProducts)
        {
            var productName = kvp.Key;
            var quantity = kvp.Value;

            var product = Products.FirstOrDefault(p => p.Name == productName);
            if (product != null)
            {
                exp += product.ExpValue * quantity;
            }
        }
        return exp;
    }
}