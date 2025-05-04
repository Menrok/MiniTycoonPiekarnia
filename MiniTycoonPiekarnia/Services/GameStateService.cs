using Blazored.LocalStorage;
using MiniTycoonPiekarnia.Models.Bakery;
using MiniTycoonPiekarnia.Models.Ingredients;
using MiniTycoonPiekarnia.Models.Products;
using MiniTycoonPiekarnia.Models.Custromers;

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
    private readonly CampaignService _campaignService;

    private const string SaveKey = "BakeryGameSave";

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();

    public GameStateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;

        _campaignService = new CampaignService(() => Bakery);

        _economyService = new EconomyService(() => Bakery, NotifyStateChanged);
        _ingredientService = new IngredientService(() => Bakery, _economyService, NotifyStateChanged, _campaignService);
        _productionService = new ProductionService(() => Bakery, SaveGameAsync, NotifyStateChanged, _campaignService);
        _customerService = new CustomerService(() => Bakery, SaveGameAsync, NotifyStateChanged, _campaignService);

        _buildingService = new BuildingService(() => Bakery, SaveGameAsync, NotifyStateChanged, _campaignService);
    }


    public List<ProductionTask> ActiveProductions => Bakery.ActiveProductions;
    public ProductionService Production => _productionService;
    public CustomerService Customer => _customerService;
    public BuildingService Building => _buildingService;
    public EconomyService Economy => _economyService;
    public IngredientService Ingredients => _ingredientService;
    public CampaignService Campaign => _campaignService;

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
        Bakery.Tiles = new List<Tile>();
        Bakery.Products = ProductList.GetInitialProducts();
        Bakery.Ingredients = IngredientList.GetInitialIngredients();
        Bakery.CustomersWaiting = new List<Customer>();
        Bakery.CustomersHistory = new List<Customer>();
        Bakery.ActiveProductions = new List<ProductionTask>();
        Bakery.Recipes = RecipeList.GetInitialRecipes();

        for (int y = 0; y < Bakery.MapHeight; y++)
        {
            for (int x = 0; x < Bakery.MapWidth; x++)
            {
                Bakery.Tiles.Add(new Tile { X = x, Y = y });
            }
        }
    }
}
