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
    }
}
