namespace MiniTycoonPiekarnia.Models.Ingredients;

public class IngredientShop
{
    public string Name { get; }
    public decimal Price { get; }
    public int QuantityToBuy { get; set; }

    public string IconFile => $"{FileNameHelper.Normalize(Name)}.png";

    public IngredientShop(string name, decimal price)
    {
        Name = name;
        Price = price;
        QuantityToBuy = 0;
    }
}
