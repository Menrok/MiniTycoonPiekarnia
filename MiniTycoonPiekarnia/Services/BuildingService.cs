using MiniTycoonPiekarnia.Models.Bakery;

namespace MiniTycoonPiekarnia.Services;

public class BuildingService
{
    private readonly CampaignService _campaignService;
    private readonly Func<Bakery> _getBakery;
    private readonly Func<Task> _saveCallback;
    private readonly Action _notifyCallback;

    public BuildingService(Func<Bakery> getBakery, Func<Task> saveCallback, Action notifyCallback, CampaignService campaignService)
    {
        _getBakery = getBakery;
        _saveCallback = saveCallback;
        _notifyCallback = notifyCallback;
        _campaignService = campaignService;
    }

    public bool PlaceBuilding(BuildingType type, decimal cost, float x, float y, int rotation)
    {
        var bakery = _getBakery();

        if (bakery.Money < cost) return false;

        const int buildingSize = 80;
        if (x < 0 || y < 0 || x + buildingSize > bakery.BakeryWidthPx || y + buildingSize > bakery.BakeryHeightPx)
            return false;

        var overlapping = bakery.Buildings.Any(b =>
            Math.Abs(b.X - x) < buildingSize && Math.Abs(b.Y - y) < buildingSize);

        if (overlapping) return false;

        bakery.Money -= cost;
        bakery.Buildings.Add(new PlacedBuilding
        {
            Type = type,
            X = x,
            Y = y,
            Rotation = rotation
        });

        _campaignService.MarkObjectiveComplete((type switch
        {
            BuildingType.Oven => "buy-oven",
            BuildingType.Shelf => "buy-shelf",
            BuildingType.DisplayShelf => "buy-cooling",
            _ => null
        })!);

        _notifyCallback();
        return true;
    }

    public async void ExpandBakeryRight(int cost)
    {
        var bakery = _getBakery();
        if (bakery.Money < cost || bakery.BakeryWidthPx >= 2000) return;

        bakery.Money -= cost;
        bakery.BakeryWidthPx += 100;

        _notifyCallback();
        await _saveCallback();
    }

    public async void ExpandBakeryDown(int cost)
    {
        var bakery = _getBakery();
        if (bakery.Money < cost || bakery.BakeryHeightPx >= 2000) return;

        bakery.Money -= cost;
        bakery.BakeryHeightPx += 100;

        _notifyCallback();
        await _saveCallback();
    }

    public void RotateBuilding(Guid id, int degrees)
    {
        var bakery = _getBakery();
        var building = bakery.Buildings.FirstOrDefault(b => b.Id == id);
        if (building != null)
        {
            building.Rotation = (building.Rotation + degrees) % 360;
            _notifyCallback();
        }
    }

    public void RemoveBuilding(Guid id, bool refund = true)
    {
        var bakery = _getBakery();
        var building = bakery.Buildings.FirstOrDefault(b => b.Id == id);
        if (building != null)
        {
            if (refund)
            {
                decimal refundAmount = GetRefundAmount(building.Type);
                bakery.Money += refundAmount;
            }

            bakery.Buildings.Remove(building);
            _notifyCallback();
        }
    }

    private decimal GetRefundAmount(BuildingType type)
    {
        var baseCost = type switch
        {
            BuildingType.Oven => 200m,
            BuildingType.Shelf => 100m,
            BuildingType.DisplayShelf => 150m,
            _ => 0m
        };

        return baseCost * 0.5m;
    }

    public void NotifyStateChanged() => _notifyCallback();
}