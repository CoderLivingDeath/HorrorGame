using Assets.Project.UI.Scripts.ViewModels;
using Reflex.Core;
using UnityEngine;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        ImmediateInstall(containerBuilder);
    }

    private void ImmediateInstall(ContainerBuilder containerBuilder)
    {
        var inputSystemActions = new InputSystem_Actions();
        var inputEventBus = new InputEventBus();

        InputServiceFactory factory = new();

        InputService InputService = factory.Create(inputSystemActions.asset, inputEventBus);

        containerBuilder.AddSingleton(inputSystemActions);
        containerBuilder.AddSingleton(inputEventBus);

        containerBuilder.AddSingleton(InputService);
    }
}