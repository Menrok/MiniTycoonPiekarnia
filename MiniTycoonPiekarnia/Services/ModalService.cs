using MiniTycoonPiekarnia.Models;

namespace MiniTycoonPiekarnia.Services;

public class ModalService
{
    public bool IsProductionModalOpen { get; private set; }
    public bool IsWarehouseModalOpen { get; private set; }
    public bool IsShopModalOpen { get; private set; }
    public bool IsUpgradesModalOpen { get; private set; }
    public bool IsReportsModalOpen { get; private set; }
    public bool IsBuildModalOpen { get; private set; }
    public bool IsCustomerModalOpen { get; private set; }
    public Tile? SelectedTileForBuild { get; private set; }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();

    private void CloseAllModals()
    {
        IsProductionModalOpen = false;
        IsWarehouseModalOpen = false;
        IsShopModalOpen = false;
        IsUpgradesModalOpen = false;
        IsReportsModalOpen = false;
        IsBuildModalOpen = false;
        IsCustomerModalOpen = false;
        SelectedTileForBuild = null;
    }

    public void OpenProductionModal()
    {
        CloseAllModals();
        IsProductionModalOpen = true;
        NotifyStateChanged();
    }

    public void OpenWarehouseModal()
    {
        CloseAllModals();
        IsWarehouseModalOpen = true;
        NotifyStateChanged();
    }

    public void OpenShopModal()
    {
        CloseAllModals();
        IsShopModalOpen = true;
        NotifyStateChanged();
    }

    public void OpenUpgradesModal()
    {
        CloseAllModals();
        IsUpgradesModalOpen = true;
        NotifyStateChanged();
    }

    public void OpenReportsModal()
    {
        CloseAllModals();
        IsReportsModalOpen = true;
        NotifyStateChanged();
    }

    public void OpenCustomerModal()
    {
        CloseAllModals();
        IsCustomerModalOpen = true;
        NotifyStateChanged();
    }
    public void OpenBuildModal(Tile tile)
    {
        CloseAllModals();
        IsBuildModalOpen = true;
        SelectedTileForBuild = tile;
        NotifyStateChanged();
    }

    public void CloseModal()
    {
        CloseAllModals();
        NotifyStateChanged();
    }
}