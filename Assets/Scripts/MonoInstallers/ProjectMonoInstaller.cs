using Zenject;

public class ProjectMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var gameInput = new InputSystem_Actions();
        gameInput.Enable();

        Container.Bind<InputSystem_Actions>().FromInstance(gameInput).AsSingle();
    }
}