using MiniTycoonPiekarnia.Models.Bakery;
using MiniTycoonPiekarnia.Models.Products;

namespace MiniTycoonPiekarnia.Services;

public class ProductionService
{
    private readonly CampaignService _campaignService;
    private readonly Func<Bakery> _getBakery;
    private readonly Func<Task> _saveCallback;
    private readonly Action _notifyCallback;

    private System.Timers.Timer? _productionTimer;
    private static readonly TimeSpan TimePerItem = TimeSpan.FromSeconds(15);

    public ProductionService(Func<Bakery> getBakery, Func<Task> saveCallback, Action notifyCallback, CampaignService campaignService)
    {
        _getBakery = getBakery;
        _saveCallback = saveCallback;
        _notifyCallback = notifyCallback;
        _campaignService = campaignService;
    }

    public void StartProductionLoop()
    {
        if (_productionTimer != null) return;

        _productionTimer = new System.Timers.Timer(1000);
        _productionTimer.Elapsed += async (_, _) => await HandleProductionTick();
        _productionTimer.AutoReset = true;
        _productionTimer.Start();
    }

    public void StartTimedProduction(string productName, int quantity)
    {
        var bakery = _getBakery();
        if (quantity <= 0) return;

        var product = bakery.Products.FirstOrDefault(p => p.Name == productName);
        if (product == null) return;

        if (bakery.CurrentProductQuantity + quantity > bakery.MaxProductCapacity)
            return;

        foreach (var required in product.RequiredIngredients)
        {
            var ing = bakery.Ingredients.FirstOrDefault(i => i.Name == required.Key);
            if (ing == null || ing.Quantity < required.Value * quantity)
                return;
        }

        foreach (var required in product.RequiredIngredients)
        {
            var ing = bakery.Ingredients.First(i => i.Name == required.Key);
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

        bakery.ActiveProductions.Add(task);
        StartProductionLoop();
        _notifyCallback();
    }

    private async Task HandleProductionTick()
    {
        var bakery = _getBakery();
        var ovens = bakery.Tiles.Count(t => t.Building == BuildingType.Oven);
        if (ovens == 0 || !bakery.ActiveProductions.Any()) return;

        var now = DateTime.Now;
        var runningTasks = bakery.ActiveProductions.Where(t => t.IsRunning).ToList();

        int availableOvens = ovens - runningTasks.Count;
        if (availableOvens > 0)
        {
            var waitingTasks = bakery.ActiveProductions.Where(t => !t.IsRunning).Take(availableOvens);
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

            var product = bakery.Products.FirstOrDefault(p => p.Name == task.ProductName);
            if (product == null) continue;

            var timePerItem = TimeSpan.FromSeconds(product.ProductionTimeSeconds);
            var elapsed = now - task.LastStarted!.Value;
            task.CurrentProgress = elapsed.TotalSeconds / timePerItem.TotalSeconds;

            if (task.CurrentProgress >= 1.0)
            {
                product.Quantity += product.ProducedAmount;

                if (task.ProductName == "Chleb")
                {
                    _campaignService.MarkObjectiveComplete("bake-bread");
                }

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
        {
            done.IsRunning = false;
            bakery.ActiveProductions.Remove(done);
        }

        await _saveCallback();
        _notifyCallback();

        if (!bakery.ActiveProductions.Any())
        {
            _productionTimer?.Stop();
            _productionTimer?.Dispose();
            _productionTimer = null;
        }
    }

    public void NotifyStateChanged() => _notifyCallback();
}