namespace MiniTycoonPiekarnia.Models.Customers;

public class Customer
{
    public Dictionary<string, int> RequestedProducts { get; set; } = new();
    public DateTime TimeCreated { get; set; } = DateTime.Now;
    public TimeSpan MaxWaitTime { get; set; } = TimeSpan.FromSeconds(300);
}
