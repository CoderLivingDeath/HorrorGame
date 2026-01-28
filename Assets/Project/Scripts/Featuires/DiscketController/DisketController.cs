using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using ScriptableAnimation;
using UnityEngine;

public class DisketController : MonoBehaviour
{
    [SerializeField]
    private Transform RootTransform;

    [SerializeField]
    private DisketAnimations _disketAnimations;

    [SerializeField]
    private DisketContainerController disketContainerController;

    [SerializeField]
    private ComputerController _computerController;

    public DisketAnimations DisketAnimations => _disketAnimations;

    public void OnInteract()
    {
        StartInject().Forget();
    }

    public async UniTask StartInject()
    {
        var hoverExitTask = _disketAnimations.AnimateHoverExit();
        _disketAnimations.CanHover = false;
        await hoverExitTask;

        this.transform.SetParent(RootTransform);
        await disketContainerController.ExitInteraction();

        await _disketAnimations.AnimateMoveToComputer();
        await _disketAnimations.AnimateMoveToInjector();

        this.gameObject.SetActive(false);

        _computerController.SetInjectedDisket(this);
    }
}
