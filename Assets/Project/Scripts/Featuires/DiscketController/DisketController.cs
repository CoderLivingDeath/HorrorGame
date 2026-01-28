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
    private DisketAnimations disketAnimations;

    [SerializeField]
    private DisketContainerController disketContainerController;

    public void OnInteract()
    {
        StartInject().Forget();
    }

    public async UniTask StartInject()
    {
        var hoverExitTask = disketAnimations.AnimateHoverExit();
        disketAnimations.CanHover = false;
        await hoverExitTask;

        this.transform.SetParent(RootTransform);
        await disketContainerController.ExitInteraction();

        await disketAnimations.AnimateMoveToComputer();
        await disketAnimations.AnimateMoveToInjector();

        this.gameObject.SetActive(false);
    }
}
