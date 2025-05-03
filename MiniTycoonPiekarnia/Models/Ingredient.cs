namespace MiniTycoonPiekarnia.Models;

public class Ingredient
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 0;
    public decimal PurchasePrice { get; set; } = 0m;

    public string IconFile => $"{Normalize(Name)}.png";

    private string Normalize(string input) =>
        input.ToLower()
             .Replace("ą", "a").Replace("ć", "c").Replace("ę", "e")
             .Replace("ł", "l").Replace("ń", "n").Replace("ó", "o")
             .Replace("ś", "s").Replace("ź", "z").Replace("ż", "z")
             .Replace(" ", "");
}
