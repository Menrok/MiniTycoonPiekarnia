using MiniTycoonPiekarnia.Models;

namespace MiniTycoonPiekarnia.Services;

public class GameStateService
{
    public Bakery Bakery { get; private set; } = new();

    public GameStateService()
    {
        InitializeBakery();
    }

    private void InitializeBakery()
    {
        for (int x = 0; x < Bakery.MapSize; x++)
        {
            for (int y = 0; y < Bakery.MapSize; y++)
            {
                Bakery.Tiles.Add(new Tile { X = x, Y = y });
            }
        }
    }

    public bool CanAfford(decimal amount)
    {
        return Bakery.Money >= amount;
    }

    public bool SpendMoney(decimal amount)
    {
        if (CanAfford(amount))
        {
            Bakery.Money -= amount;
            return true;
        }
        return false;
    }

    public void EarnMoney(decimal amount)
    {
        Bakery.Money += amount;
    }

    public void BuyIngredient(string name, decimal price, int quantity)
    {
        var ingredient = Bakery.Ingredients.FirstOrDefault(i => i.Name == name);
        if (ingredient == null)
        {
            ingredient = new Ingredient { Name = name, PurchasePrice = price };
            Bakery.Ingredients.Add(ingredient);
        }
        ingredient.Quantity += quantity;
        SpendMoney(price * quantity);
    }

    public void StartProduction(string productName, int quantity)
    {
        var product = Bakery.Products.FirstOrDefault(p => p.Name == productName);
        if (product == null)
        {
            return;
        }

        foreach (var required in product.RequiredIngredients)
        {
            var ingredient = Bakery.Ingredients.FirstOrDefault(i => i.Name == required.Key);
            if (ingredient == null || ingredient.Quantity < required.Value * quantity)
            {
                return;
            }
        }

        foreach (var required in product.RequiredIngredients)
        {
            var ingredient = Bakery.Ingredients.First(i => i.Name == required.Key);
            ingredient.Quantity -= required.Value * quantity;
        }

        product.Quantity += quantity;
    }

    public void BuildBuilding(int x, int y, BuildingType type, decimal cost)
    {
        if (!SpendMoney(cost))
            return;

        var tile = Bakery.Tiles.FirstOrDefault(t => t.X == x && t.Y == y);
        if (tile != null && tile.Building == null)
        {
            tile.Building = type;
        }
    }

    public void ExpandBakery(int cost)
    {
        if (!SpendMoney(cost))
            return;

        Bakery.MapSize++;
        for (int i = 0; i < Bakery.MapSize; i++)
        {
            Bakery.Tiles.Add(new Tile { X = Bakery.MapSize - 1, Y = i });
            Bakery.Tiles.Add(new Tile { X = i, Y = Bakery.MapSize - 1 });
        }
    }
}