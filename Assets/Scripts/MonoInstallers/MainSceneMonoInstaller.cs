using Core;
using Ui;
using UnityEngine;
using Zenject;

public class MainSceneMonoInstaller : MonoInstaller
{
    [SerializeField] private UiManager uiManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private HybridHandler hybridHandler;

    public override void InstallBindings()
    {
        Container.Bind<UiManager>().FromInstance(uiManager).AsSingle();
        Container.Bind<GameManager>().FromInstance(gameManager).AsSingle();
        poolManager.InitializePools();
        Container.Bind<PoolManager>().FromInstance(poolManager).AsSingle();
        Container.Bind<HybridHandler>().FromInstance(hybridHandler).AsSingle();
        Container.Bind<IInventory>().To<Inventory>().AsSingle();
    }
}