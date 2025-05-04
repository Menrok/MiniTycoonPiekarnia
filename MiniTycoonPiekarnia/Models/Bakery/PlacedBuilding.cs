namespace MiniTycoonPiekarnia.Models.Bakery;

public class PlacedBuilding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public BuildingType Type { get; set; }
    public float X { get; set; } 
    public float Y { get; set; }
    public int Rotation { get; set; } = 0;
}