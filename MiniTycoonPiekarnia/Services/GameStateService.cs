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
        Bakery.Products = ProductList.GetInitialProducts();
        Bakery.Ingredients = IngredientList.GetInitialIngredients();
        Bakery.CustomersWaiting = new List<Customer>();
        Bakery.CustomersHistory = new List<Customer>();
        Bakery.ActiveProductions = new List<ProductionTask>();
        Bakery.Recipes = RecipeList.GetInitialRecipes();

        Bakery.BakeryWidthPx = 300;
        Bakery.BakeryHeightPx = 300;
        Bakery.Buildings = new List<PlacedBuilding>();
    }

    public BuildingPlacement? ActivePlacement { get; private set; }

    public void StartBuildingPlacement(BuildingPlacement placement)
    {
        ActivePlacement = placement;
        NotifyStateChanged();
    }

    public void CancelPlacement()
    {
        ActivePlacement = null;
        NotifyStateChanged();
    }

    public async Task ConfirmPlacement(float x, float y)
    {
        if (ActivePlacement == null) return;

        var halfSize = 40;

        if (x < 0 || y < 0 ||
            x + halfSize > Bakery.BakeryWidthPx ||
            y + halfSize > Bakery.BakeryHeightPx)
        {
            return;
        }

        var success = Building.PlaceBuilding(
            ActivePlacement.Type,
            ActivePlacement.Cost,
            x,
            y,
            ActivePlacement.Rotation);

        if (success)
        {
            ActivePlacement = null;
            await SaveGameAsync();
        }

        NotifyStateChanged();
    }

    public bool IsPlacementFrozen { get; private set; }
    public float PlacementFrozenX { get; private set; }
    public float PlacementFrozenY { get; private set; }
    public void FreezePlacement(float x, float y)
    {
        PlacementFrozenX = x;
        PlacementFrozenY = y;
        IsPlacementFrozen = true;
        NotifyStateChanged();
    }

    public async Task FinalizePlacement()
    {
        if (ActivePlacement == null) return;

        var success = Building.PlaceBuilding(
            ActivePlacement.Type,
            ActivePlacement.Cost,
            PlacementFrozenX,
            PlacementFrozenY,
            ActivePlacement.Rotation);

        if (success)
        {
            ActivePlacement = null;
            IsPlacementFrozen = false;
            await SaveGameAsync();
            NotifyStateChanged();
        }
    }

    public Guid? SelectedBuildingId { get; private set; }

    public void SelectBuilding(Guid id)
    {
        SelectedBuildingId = id;
        NotifyStateChanged();
    }

    public void DeselectBuilding()
    {
        SelectedBuildingId = null;
        NotifyStateChanged();
    }
}