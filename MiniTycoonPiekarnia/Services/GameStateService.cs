using MiniTycoonPiekarnia.Models;
using Blazored.LocalStorage;

namespace MiniTycoonPiekarnia.Services;

public class GameStateService
{
    public Bakery Bakery { get; private set; } = new();

    private readonly ILocalStorageService _localStorage;
    private readonly ProductionService _productionService;
    private readonly CustomerService _customerService;
    private readonly BuildingService _buildingService;
    private readonly EconomyService _economyService;
    private readonly IngredientService _ingredientService;

    private const string SaveKey = "BakeryGameSave";

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();

    public GameStateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _economyService = new EconomyService(() => Bakery, NotifyStateChanged);
        _ingredientService = new IngredientService(() => Bakery, _economyService, NotifyStateChanged);
        _productionService = new ProductionService(() => Bakery, SaveGameAsync, NotifyStateChanged);
        _customerService = new CustomerService(() => Bakery, SaveGameAsync, NotifyStateChanged);
        _buildingService = new BuildingService(() => Bakery, SaveGameAsync, NotifyStateChanged);
    }

    public IReadOnlyList<ProductionTask> ActiveProductions => Bakery.ActiveProductions;
    public ProductionService Production => _productionService;
    public CustomerService Customer => _customerService;
    public BuildingService Building => _buildingService;
    public EconomyService Economy => _economyService;
    public IngredientService Ingredients => _ingredientService;

    public async Task SaveGameAsync() => await _localStorage.SetItemAsync(SaveKey, Bakery);

    public async Task LoadGameAsync()
    {
        var loaded = await _localStorage.GetItemAsync<Bakery>(SaveKey);
        Bakery = loaded ?? new Bakery();

        if (loaded == null)
        {
            InitializeBakery();
            await SaveGameAsync();
        }

        NotifyStateChanged();
        _customerService.StartCustomerTimer();

        if (Bakery.ActiveProductions.Any())
            _productionService.StartProductionLoop();
    }

    private void InitializeBakery()
    {
        Bakery.MapSize = 3;
        Bakery.Tiles = new List<Tile>();
        Bakery.Products = ProductList.GetInitialProducts();
        Bakery.Ingredients = IngredientList.GetInitialIngredients();
        Bakery.CustomersWaiting = new List<Customer>();
        Bakery.CustomersHistory = new List<Customer>();
        Bakery.ActiveProductions = new List<ProductionTask>();

        for (int y = 0; y < Bakery.MapSize; y++)
        {
            for (int x = 0; x < Bakery.MapSize; x++)
            {
                Bakery.Tiles.Add(new Tile { X = x, Y = y });
            }
        }
    }
}