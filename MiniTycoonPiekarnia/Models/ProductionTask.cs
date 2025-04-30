namespace MiniTycoonPiekarnia.Models;

public class ProductionTask
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantityRemaining { get; set; }
    public int TotalQuantity { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? LastStarted { get; set; }
    public double CurrentProgress { get; set; }

    public bool IsRunning { get; set; } = false;
}
