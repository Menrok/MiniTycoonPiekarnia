using MiniTycoonPiekarnia.Models;
using Blazored.LocalStorage;

namespace MiniTycoonPiekarnia.Services;

public class GameStateService
{
    public Bakery Bakery { get; private set; } = new();

    private readonly ILocalStorageService _localStorage;
    private const string SaveKey = "BakeryGameSave";

    private readonly List<ProductionTask> _activeProductions = new();
    public IReadOnlyList<ProductionTask> ActiveProductions => _activeProductions;

    private System.Timers.Timer? _productionTimer;
    private System.Timers.Timer? _customerTimer;
    public DateTime LastCustomerTime { get; private set; } = DateTime.Now;
    public TimeSpan CustomerInterval => TimeSpan.FromSeconds(30);

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();

    public GameStateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SaveGameAsync()
    {
        await _localStorage.SetItemAsync(SaveKey, Bakery);
    }

    public async Task LoadGameAsync()
    {
        var loaded = await _localStorage.GetItemAsync<Bakery>(SaveKey);
        Bakery = loaded ?? new Bakery();

        if (loaded == null)
            InitializeBakery();

        NotifyStateChanged();

        StartCustomerTimer();
    }

    private void InitializeBakery()
    {
        Bakery.MapSize = 3;

        for (int y = 0; y < Bakery.MapSize; y++)
        {
            for (int x = 0; x < Bakery.MapSize; x++)
            {
                Bakery.Tiles.Add(new Tile { X = x, Y = y });
            }
        }

        Bakery.Products = ProductList.GetInitialProducts();
        Bakery.Ingredients = IngredientList.GetInitialIngredients();
    }
    private void StartCustomerTimer()
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

        var now = DateTime.Now;
        var toRemove = Bakery.CustomersWaiting
            .Where(c => now - c.TimeCreated > c.MaxWaitTime)
            .ToList();

        foreach (var c in toRemove)
        {
            Bakery.CustomersWaiting.Remove(c);
            Bakery.CustomerSatisfaction = Math.Max(0, Bakery.CustomerSatisfaction - 5);
        }

        NotifyStateChanged();
    }

    private async Task HandleCustomerVisit()
    {
        var random = new Random();

        var possibleProducts = Bakery.Products.Where(p => p.RequiredIngredients.Any()).ToList();
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

        Bakery.CustomersWaiting.Add(new Customer
        {
            RequestedProducts = requested,
            TimeCreated = DateTime.Now,
            MaxWaitTime = TimeSpan.FromSeconds(60)
        });

        await SaveGameAsync();
        NotifyStateChanged();
    }
    public void FulfillCustomer(Customer customer)
    {
        if (!Bakery.CustomersWaiting.Contains(customer))
            return;

        bool canFulfill = customer.RequestedProducts.All(req =>
        {
            var prod = Bakery.Products.FirstOrDefault(p => p.Name == req.Key);
            return prod != null && prod.Quantity >= req.Value;
        });

        if (!canFulfill)
            return;

        foreach (var req in customer.RequestedProducts)
        {
            var prod = Bakery.Products.First(p => p.Name == req.Key);
            prod.Quantity -= req.Value;
            prod.QuantitySold += req.Value;
            EarnMoney(prod.SalePrice * req.Value);
        }

        Bakery.CustomersWaiting.Remove(customer);
        Bakery.CustomersHistory.Add(customer);
        Bakery.CustomerSatisfaction = Math.Min(100, Bakery.CustomerSatisfaction + 3);

        NotifyStateChanged();
    }

    public bool CanAfford(decimal amount) => Bakery.Money >= amount;

    public bool SpendMoney(decimal amount)
    {
        if (!CanAfford(amount)) return false;

        Bakery.Money -= amount;
        NotifyStateChanged();
        return true;
    }

    public void EarnMoney(decimal amount)
    {
        Bakery.Money += amount;
        NotifyStateChanged();
    }

    public void BuyIngredient(string name, decimal price, int quantity)
    {
        if (Bakery.CurrentIngredientQuantity + quantity > Bakery.MaxIngredientCapacity)
            return;

        var ingredient = Bakery.Ingredients.FirstOrDefault(i => i.Name == name);
        if (ingredient == null)
        {
            ingredient = new Ingredient { Name = name, PurchasePrice = price };
            Bakery.Ingredients.Add(ingredient);
        }
        ingredient.Quantity += quantity;
        SpendMoney(price * quantity);
        NotifyStateChanged();
    }

    public bool BuildBuilding(int x, int y, BuildingType type, decimal cost, int rotation)
    {
        if (!SpendMoney(cost)) return false;

        var tile = Bakery.Tiles.FirstOrDefault(t => t.X == x && t.Y == y);
        if (tile == null || tile.Building != null) return false;

        tile.Building = type;
        tile.Rotation = rotation;

        NotifyStateChanged();
        return true;
    }

    public void ExpandBakery(int cost)
    {
        if (!SpendMoney(cost)) return;

        Bakery.MapSize++;
        for (int i = 0; i < Bakery.MapSize; i++)
        {
            Bakery.Tiles.Add(new Tile { X = Bakery.MapSize - 1, Y = i });
            Bakery.Tiles.Add(new Tile { X = i, Y = Bakery.MapSize - 1 });
        }
    }

    public void StartTimedProduction(string productName, int quantity)
    {
        if (quantity <= 0) return;

        var product = Bakery.Products.FirstOrDefault(p => p.Name == productName);
        if (product == null) return;

        if (Bakery.CurrentProductQuantity + quantity > Bakery.MaxProductCapacity)
            return;

        foreach (var required in product.RequiredIngredients)
        {
            var ing = Bakery.Ingredients.FirstOrDefault(i => i.Name == required.Key);
            if (ing == null || ing.Quantity < required.Value * quantity)
                return;
        }

        foreach (var required in product.RequiredIngredients)
        {
            var ing = Bakery.Ingredients.First(i => i.Name == required.Key);
            ing.Quantity -= required.Value * quantity;
        }

        var task = new ProductionTask
        {
            ProductName = productName,
            QuantityRemaining = quantity,
            TotalQuantity = quantity,
            StartTime = DateTime.Now,
            LastStarted = null,
            CurrentProgress = 0.0,
            IsRunning = false
        };

        _activeProductions.Add(task);
        StartProductionLoop();
        NotifyStateChanged();
    }

    private void StartProductionLoop()
    {
        if (_productionTimer != null) return;

        _productionTimer = new System.Timers.Timer(1000);
        _productionTimer.Elapsed += async (_, _) => await HandleProductionTick();
        _productionTimer.AutoReset = true;
        _productionTimer.Start();
    }

    private static readonly TimeSpan TimePerItem = TimeSpan.FromSeconds(15);
    private async Task HandleProductionTick()
    {
        var ovens = Bakery.Tiles.Count(t => t.Building == BuildingType.Oven);
        if (ovens == 0 || !_activeProductions.Any())
            return;

        var now = DateTime.Now;
        var runningTasks = _activeProductions.Where(t => t.IsRunning).ToList();

        int availableOvens = ovens - runningTasks.Count;
        if (availableOvens > 0)
        {
            var waitingTasks = _activeProductions.Where(t => !t.IsRunning).Take(availableOvens);
            foreach (var task in waitingTasks)
            {
                task.IsRunning = true;
                task.LastStarted = now;
                task.CurrentProgress = 0;
            }
        }

        var completedTasks = new List<ProductionTask>();

        foreach (var task in runningTasks)
        {
            if (task.QuantityRemaining <= 0)
            {
                completedTasks.Add(task);
                continue;
            }

            var elapsed = now - task.LastStarted!.Value;
            task.CurrentProgress = elapsed.TotalSeconds / TimePerItem.TotalSeconds;

            if (task.CurrentProgress >= 1.0)
            {
                var product = Bakery.Products.First(p => p.Name == task.ProductName);
                product.Quantity++;
                task.QuantityRemaining--;

                if (task.QuantityRemaining <= 0)
                {
                    completedTasks.Add(task);
                }
                else
                {
                    task.LastStarted = now;
                    task.CurrentProgress = 0;
                }
            }
        }

        foreach (var done in completedTasks)
            _activeProductions.Remove(done);

        foreach (var done in completedTasks)
            done.IsRunning = false;

        await SaveGameAsync();
        NotifyStateChanged();

        if (!_activeProductions.Any())
        {
            _productionTimer?.Stop();
            _productionTimer?.Dispose();
            _productionTimer = null;
        }
    }

}