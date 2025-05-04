using MiniTycoonPiekarnia.Models.Campaign;
using MiniTycoonPiekarnia.Models.Custromers;
using MiniTycoonPiekarnia.Models.Ingredients;
using MiniTycoonPiekarnia.Models.Products;

namespace MiniTycoonPiekarnia.Models.Bakery;

public class Bakery
{
    public decimal Money { get; set; } = 1000m;
    public decimal TotalEarned { get; set; } = 0;
    public int Experience { get; set; } = 0;
    public int BakeryLevel { get; set; } = 1;
    public int ExperienceToNextLevel => 100 + BakeryLevel * 50;

    public int CustomerSatisfaction { get; set; } = 100;

    public List<Tile> Tiles { get; set; } = new();
    public int MapWidth { get; set; } = 3;
    public int MapHeight { get; set; } = 3;

    public List<Ingredient> Ingredients { get; set; } = new();
    public List<Product> Products { get; set; } = new();

    public List<ProductionTask> ActiveProductions { get; set; } = new();
    public List<Recipe> Recipes { get; set; } = new();

    public List<Customer> CustomersWaiting { get; set; } = new();
    public List<Customer> CustomersHistory { get; set; } = new();

    public CampaignProgress CampaignProgress { get; set; } = new();

    public int MaxIngredientCapacity => 50 * Tiles.Count(t => t.Building == BuildingType.Shelf);
    public int MaxProductCapacity => 50 * Tiles.Count(t => t.Building == BuildingType.Website);
    public decimal CurrentIngredientQuantity => Ingredients.Sum(i => i.Quantity);
    public int CurrentProductQuantity => Products.Sum(p => p.Quantity);

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
        foreach (var (productName, quantity) in customer.RequestedProducts)
        {
            var product = Products.FirstOrDefault(p => p.Name == productName);
            if (product != null)
            {
                exp += product.ExpValue * quantity;
            }
        }
        return exp;
    }
}
