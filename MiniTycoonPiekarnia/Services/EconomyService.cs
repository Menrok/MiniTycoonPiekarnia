using MiniTycoonPiekarnia.Models.Bakery;

namespace MiniTycoonPiekarnia.Services;

public class EconomyService
{
    private readonly Func<Bakery> _getBakery;
    private readonly Action _notifyCallback;

    public EconomyService(Func<Bakery> getBakery, Action notifyCallback)
    {
        _getBakery = getBakery;
        _notifyCallback = notifyCallback;
    }

    public bool CanAfford(decimal amount) => _getBakery().Money >= amount;

    public bool SpendMoney(decimal amount)
    {
        if (!CanAfford(amount)) return false;
        _getBakery().Money -= amount;
        _notifyCallback();
        return true;
    }

    public void EarnMoney(decimal amount)
    {
        _getBakery().Money += amount;
        _notifyCallback();
    }

    public void NotifyStateChanged() => _notifyCallback();
}
