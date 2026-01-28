using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Threading;
using UnityEngine;

public class ComputerAnimations : MonoBehaviour
{
    #region Animations

    [Serializable]
    public struct MoveCameraToComputer
    {
        public Transform Object;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public Space Space;
        public Transform TargetPosition;
    }

    [Serializable]
    public struct MoveCameraToPlayer
    {
        public Transform Object;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public Space Space;
        public Transform TargetPosition;
    }

    private UniTask AnimateMoveCameraToComputer(in MoveCameraToComputer context, CancellationToken token = default)
    {
        Vector3 basePosition = context.Space == Space.Self
     ? context.Object.localPosition : context.Object.position;

        Vector3 targetPosition = context.TargetPosition.position;

        var builder = LMotion.Create(basePosition, targetPosition, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        var handle = context.Space == Space.Self
            ? builder.BindToLocalPosition(context.Object) : builder.BindToPosition(context.Object);

        return handle.ToUniTask(token);
    }

    private UniTask AnimateMoveCameraToPlayer(in MoveCameraToPlayer context, CancellationToken token = default)
    {
        Vector3 basePosition = context.Space == Space.Self
     ? context.Object.localPosition : context.Object.position;

        Vector3 targetPosition = context.TargetPosition.position;

        var builder = LMotion.Create(basePosition, targetPosition, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        var handle = context.Space == Space.Self
            ? builder.BindToLocalPosition(context.Object) : builder.BindToPosition(context.Object);

        return handle.ToUniTask(token);
    }

    #endregion

    #region Contexts

    [SerializeField] private MoveCameraToComputer _moveCameraToComputer;
    [SerializeField] private MoveCameraToPlayer _moveCameraToPlayer;

    #endregion

    #region CTS

    private CancellationTokenSource _GlobalCTS;
    private CancellationTokenSource _MoveCameraCTS;

    private CancellationTokenSource CreateGlobalCTS() => CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
    private CancellationTokenSource CreateMoveCameraCTS() => CancellationTokenSource.CreateLinkedTokenSource(_GlobalCTS.Token);
    #endregion

    private void Awake()
    {
        _GlobalCTS = CreateGlobalCTS();
        _MoveCameraCTS = CreateMoveCameraCTS();
    }

    public async UniTask AnimateMoveCameraToComputer(CancellationToken token = default)
    {
        CancleMoveCamera();

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(token, _MoveCameraCTS.Token);
        await AnimateMoveCameraToComputer(_moveCameraToComputer, linkedCTS.Token);
    }

    public async UniTask AnimateMoveCameraToPlayer(CancellationToken token = default)
    {
        CancleMoveCamera();

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(token, _MoveCameraCTS.Token);
        await AnimateMoveCameraToPlayer(_moveCameraToPlayer, linkedCTS.Token);
    }

    [ContextMenu("Play: Animation Move Camera To Computer")]
    public void PlayAnimationMoveCameraToComputer()
    {
        AnimateMoveCameraToComputer().Forget();
    }

    [ContextMenu("Play: Animation Move Camera To Player")]
    public void PlayAnimationMoveCameraToPlayer()
    {
        AnimateMoveCameraToPlayer().Forget();
    }

    [ContextMenu("Cancel: Move Camera")]
    public void CancleMoveCamera()
    {
        _MoveCameraCTS?.Cancel();
        _MoveCameraCTS?.Dispose();
        _MoveCameraCTS = CreateMoveCameraCTS();
    }

}
