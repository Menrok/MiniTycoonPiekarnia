namespace MiniTycoonPiekarnia.Models;

public class IngredientShop
{
    public string Name { get; }
    public decimal Price { get; }
    public int QuantityToBuy { get; set; }

    public string IconFile => $"{Normalize(Name)}.png";

    private string Normalize(string input) =>
        input.ToLower()
             .Replace("ą", "a").Replace("ć", "c").Replace("ę", "e")
             .Replace("ł", "l").Replace("ń", "n").Replace("ó", "o")
             .Replace("ś", "s").Replace("ź", "z").Replace("ż", "z")
             .Replace(" ", "");

    public IngredientShop(string name, decimal price)
    {
        Name = name;
        Price = price;
        QuantityToBuy = 0;
    }
}
