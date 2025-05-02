using MiniTycoonPiekarnia.Models;

namespace MiniTycoonPiekarnia.Services;

public class CustomerService
{
    private readonly Func<Bakery> _getBakery;
    private readonly Func<Task> _saveCallback;
    private readonly Action _notifyCallback;

    private System.Timers.Timer? _customerTimer;
    public DateTime LastCustomerTime { get; private set; } = DateTime.Now;
    public TimeSpan CustomerInterval => TimeSpan.FromSeconds(30);

    public CustomerService(Func<Bakery> getBakery, Func<Task> saveCallback, Action notifyCallback)
    {
        _getBakery = getBakery;
        _saveCallback = saveCallback;
        _notifyCallback = notifyCallback;
    }

    public void StartCustomerTimer()
    {
        if (_customerTimer != null) return;

        _customerTimer = new System.Timers.Timer(1000);
        _customerTimer.Elapsed += async (_, _) => await CustomerTick();
        _customerTimer.AutoReset = true;
        _customerTimer.Start();
    }

    private async Task CustomerTick()
    {
        if (DateTime.Now - LastCustomerTime >= CustomerInterval)
        {
            await HandleCustomerVisit();
            LastCustomerTime = DateTime.Now;
        }

        var bakery = _getBakery();
        var now = DateTime.Now;
        var toRemove = bakery.CustomersWaiting
            .Where(c => now - c.TimeCreated > c.MaxWaitTime)
            .ToList();

        foreach (var c in toRemove)
        {
            bakery.CustomersWaiting.Remove(c);
            bakery.CustomerSatisfaction = Math.Max(0, bakery.CustomerSatisfaction - 5);
        }

        _notifyCallback();
    }

    private async Task HandleCustomerVisit()
    {
        var bakery = _getBakery();
        var random = new Random();
        var possibleProducts = bakery.Products.Where(p => p.RequiredIngredients.Any()).ToList();
        if (!possibleProducts.Any()) return;

        var requested = new Dictionary<string, int>();
        int count = random.Next(1, 3);
        for (int i = 0; i < count; i++)
        {
            var product = possibleProducts[random.Next(possibleProducts.Count)];
            if (requested.ContainsKey(product.Name))
                requested[product.Name]++;
            else
                requested[product.Name] = 1;
        }

        bakery.CustomersWaiting.Add(new Customer
        {
            RequestedProducts = requested,
            TimeCreated = DateTime.Now,
        });

        await _saveCallback();
        _notifyCallback();
    }

    public void FulfillCustomer(Customer customer)
    {
        var bakery = _getBakery();

        if (!bakery.CustomersWaiting.Contains(customer))
            return;

        bool canFulfill = customer.RequestedProducts.All(req =>
        {
            var prod = bakery.Products.FirstOrDefault(p => p.Name == req.Key);
            return prod != null && prod.Quantity >= req.Value;
        });

        if (!canFulfill)
            return;

        foreach (var req in customer.RequestedProducts)
        {
            var prod = bakery.Products.First(p => p.Name == req.Key);
            prod.Quantity -= req.Value;
            prod.QuantitySold += req.Value;
            bakery.Money += prod.SalePrice * req.Value;
        }

        bakery.CustomersWaiting.Remove(customer);
        bakery.CustomersHistory.Add(customer);
        bakery.CustomerSatisfaction = Math.Min(100, bakery.CustomerSatisfaction + 3);

        _notifyCallback();
    }

    public void NotifyStateChanged() => _notifyCallback();
}
