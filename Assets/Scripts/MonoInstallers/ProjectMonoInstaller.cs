using Zenject;

public class ProjectMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var gameInput = new NewInputActions();
        gameInput.Enable();

        Container.Bind<NewInputActions>().FromInstance(gameInput).AsSingle();
    }
}