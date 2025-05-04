using MiniTycoonPiekarnia.Models.Bakery;
using MiniTycoonPiekarnia.Models.Ingredients;

namespace MiniTycoonPiekarnia.Services;

public class IngredientService
{
    private readonly CampaignService _campaign;
    private readonly Func<Bakery> _getBakery;
    private readonly EconomyService _economy;
    private readonly Action _notify;

    public IngredientService(Func<Bakery> getBakery, EconomyService economy, Action notify, CampaignService campaign)
    {
        _getBakery = getBakery;
        _economy = economy;
        _notify = notify;
        _campaign = campaign;
    }


    public void BuyIngredient(string name, decimal price, int quantity)
    {
        var bakery = _getBakery();
        if (bakery.CurrentIngredientQuantity + quantity > bakery.MaxIngredientCapacity)
            return;

        var ingredient = bakery.Ingredients.FirstOrDefault(i => i.Name == name);
        if (ingredient == null)
        {
            ingredient = new Ingredient { Name = name, PurchasePrice = price };
            bakery.Ingredients.Add(ingredient);
        }

        ingredient.Quantity += quantity;
        _economy.SpendMoney(price * quantity);

        if (name.ToLower() == "mąka")
            _campaign.MarkObjectiveComplete("buy-maka");
        else if (name.ToLower() == "drożdże")
            _campaign.MarkObjectiveComplete("buy-drozdze");

        _notify();
    }
}
