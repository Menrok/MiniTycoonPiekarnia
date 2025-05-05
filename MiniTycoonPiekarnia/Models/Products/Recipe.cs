using System.Xml.Linq;

namespace MiniTycoonPiekarnia.Models.Products;

public class Recipe
{
    public string ProductName { get; set; } = "";
    public bool IsUnlocked { get; set; } = false;
    public decimal UnlockCost { get; set; } = 0;
    public Product? ProductRef { get; set; }

    public string IconFile => $"{FileNameHelper.Normalize(ProductName)}.png";

}
