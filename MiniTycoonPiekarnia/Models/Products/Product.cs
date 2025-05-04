namespace MiniTycoonPiekarnia.Models.Products;

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

    public string IconFile => $"{FileNameHelper.Normalize(Name)}.png";

}
