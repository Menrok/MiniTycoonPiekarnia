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


    public bool BuildBuilding(int x, int y, BuildingType type, decimal cost, int rotation)
    {
        var bakery = _getBakery();

        if (bakery.Money < cost) return false;

        var tile = bakery.Tiles.FirstOrDefault(t => t.X == x && t.Y == y);
        if (tile == null || tile.Building != null) return false;

        bakery.Money -= cost;
        tile.Building = type;
        tile.Rotation = rotation;

        switch (type)
        {
            case BuildingType.Oven:
                _campaignService.MarkObjectiveComplete("buy-oven");
                break;
            case BuildingType.Shelf:
                _campaignService.MarkObjectiveComplete("buy-shelf");
                break;
            case BuildingType.Website:
                _campaignService.MarkObjectiveComplete("buy-cooling");
                break;
        }

        _notifyCallback();
        return true;
    }


    public async void ExpandBakeryRight(int cost)
    {
        var bakery = _getBakery();
        if (bakery.Money < cost || bakery.MapWidth >= 10) return;

        int newX = bakery.MapWidth;
        for (int y = 0; y < bakery.MapHeight; y++)
            bakery.Tiles.Add(new Tile { X = newX, Y = y });

        bakery.Money -= cost;
        bakery.MapWidth++;

        _notifyCallback();
        await _saveCallback();
    }

    public async void ExpandBakeryDown(int cost)
    {
        var bakery = _getBakery();
        if (bakery.Money < cost || bakery.MapHeight >= 10) return;

        int newY = bakery.MapHeight;
        for (int x = 0; x < bakery.MapWidth; x++)
            bakery.Tiles.Add(new Tile { X = x, Y = newY });

        bakery.Money -= cost;
        bakery.MapHeight++;

        _notifyCallback();
        await _saveCallback();
    }

    public void NotifyStateChanged() => _notifyCallback();
}
