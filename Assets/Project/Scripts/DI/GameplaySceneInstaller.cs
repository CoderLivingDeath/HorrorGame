using Assets.Project.UI.Scripts.ViewModels;
using Reflex.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class GameplaySceneInstaller : MonoBehaviour, IInstaller
{
    [SerializeField]
    private ComputerManager computerManager;
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(computerManager);
        
        containerBuilder.AddTransient<PlayerService>(c => new PlayerService());
        containerBuilder.AddTransient<MacWelcomeViewModel>(c => new MacWelcomeViewModel(c.Resolve<ComputerManager>(), new()));
        containerBuilder.AddTransient<PrintViewModel>(c => new PrintViewModel(c.Resolve<PlayerService>()));
        containerBuilder.AddTransient<PrintSecondViewModel>(c => new PrintSecondViewModel(c.Resolve<PlayerService>()));
    }
}
