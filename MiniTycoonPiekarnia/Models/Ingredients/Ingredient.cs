namespace MiniTycoonPiekarnia.Models.Ingredients;

public class Ingredient
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 0;
    public decimal PurchasePrice { get; set; } = 0m;

    public string IconFile => $"{FileNameHelper.Normalize(Name)}.png";

}
