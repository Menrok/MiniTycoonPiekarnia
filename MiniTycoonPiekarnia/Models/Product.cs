namespace MiniTycoonPiekarnia.Models;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
    public int QuantitySold { get; set; }
    public decimal SalePrice { get; set; }
    public int ProducedAmount { get; set; }
    public int ProductionTimeSeconds { get; set; }
    public int ExpValue { get; set; }

    public Dictionary<string, decimal> RequiredIngredients { get; set; } = new();

    public string IconFile => $"{Normalize(Name)}.png";

    private string Normalize(string input) =>
        input.ToLower()
             .Replace("ą", "a").Replace("ć", "c").Replace("ę", "e")
             .Replace("ł", "l").Replace("ń", "n").Replace("ó", "o")
             .Replace("ś", "s").Replace("ź", "z").Replace("ż", "z")
             .Replace(" ", "");
}
