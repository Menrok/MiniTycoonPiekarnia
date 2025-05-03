using MiniTycoonPiekarnia.Models;

namespace MiniTycoonPiekarnia.Services;

public class CustomerService
{
    private readonly Func<Bakery> _getBakery;
    private readonly Func<Task> _saveCallback;
    private readonly Action _notifyCallback;

    private System.Timers.Timer? _customerTimer;
    private readonly Random _random = new();

    public DateTime LastCustomerTime { get; private set; } = DateTime.Now;
    private TimeSpan _currentCustomerInterval = TimeSpan.FromSeconds(45);
    public TimeSpan CustomerInterval => _currentCustomerInterval;

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
        if (DateTime.Now - LastCustomerTime >= _currentCustomerInterval)
        {
            await HandleCustomerVisit();
            LastCustomerTime = DateTime.Now;
            _currentCustomerInterval = TimeSpan.FromSeconds(_random.Next(30, 60)); // nowy losowy interwał
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
        var possibleProducts = bakery.Products.Where(p => p.RequiredIngredients.Any()).ToList();
        if (!possibleProducts.Any()) return;

        var requested = new Dictionary<string, int>();

        foreach (var product in possibleProducts)
        {
            int quantity = product.Name switch
            {
                "Ciasto" => _random.Next(0, 2),
                "Bułka" => _random.Next(0, 9),
                "Chleb" => _random.Next(0, 3),
                _ => 0
            };

            if (quantity > 0)
                requested[product.Name] = quantity;
        }

        if (requested.Count > 0)
        {
            bakery.CustomersWaiting.Add(new Customer
            {
                RequestedProducts = requested,
                TimeCreated = DateTime.Now,
                MaxWaitTime = TimeSpan.FromSeconds(_random.Next(300, 420))
            });

            await _saveCallback();
            _notifyCallback();
        }
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
