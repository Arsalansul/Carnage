using UnityEngine;
using Zenject;

public class MainSceneMonoInstaller : MonoInstaller
{
    [SerializeField] private UiManager uiManager;
    [SerializeField] private GameManager gameManager;

    public override void InstallBindings()
    {
        Container.Bind<UiManager>().FromInstance(uiManager).AsSingle();
        Container.Bind<GameManager>().FromInstance(gameManager).AsSingle();
    }
}