using Ui.Controllers;
using Ui.View;
using UnityEngine;
using Zenject;

public class UiInstaller : MonoInstaller
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private InGameHudController inGameHudController;
    [SerializeField] private EndGameUiController endGameUiController;
    [SerializeField] private InventorySlot inventorySlotPrefab;
    
    public override void InstallBindings()
    {
        Container.Bind<InventoryController>().FromInstance(inventoryController).AsSingle();
        Container.Bind<InGameHudController>().FromInstance(inGameHudController).AsSingle();
        Container.Bind<EndGameUiController>().FromInstance(endGameUiController).AsSingle();
        Container.BindMemoryPool<InventorySlot, InventorySlot.Pool>()
            .WithMaxSize(5)
            .FromComponentInNewPrefab(inventorySlotPrefab);
    }
}