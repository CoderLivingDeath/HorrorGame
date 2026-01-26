using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Threading;
using UnityEngine;

public class DisketAnimations : MonoBehaviour
{
    #region Animations
    [Serializable]
    public struct HoverEnterContext
    {
        public Transform Object;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public Space Space;
        public Vector3 TargetPosition;
        public Vector3 TargetRotation;
    }

    [Serializable]
    public struct HoverExitContext
    {
        public Transform Object;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public Space Space;
        public Vector3 TargetPosition;
        public Vector3 TargetRotation;
    }

    private UniTask AnimateHoverEnter(in HoverEnterContext context, CancellationToken token = default)
    {
        if (context.Object == null || context.TargetPosition == null)
            return UniTask.CompletedTask;

        Vector3 basePosition = context.Space == Space.Self ? context.Object.localPosition : context.Object.position;

        Quaternion baseRotation = context.Space == Space.Self ? context.Object.localRotation : context.Object.rotation;

        var moveMotionBuilder = LMotion.Create(basePosition, context.TargetPosition, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        var rotationMotionBuilder = LMotion.Create(baseRotation, Quaternion.Euler(context.TargetRotation), context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        MotionHandle moveHandle;
        MotionHandle rotationHandle;

        if (context.Space == Space.Self)
        {
            moveHandle = moveMotionBuilder.BindToLocalPosition(context.Object);
            rotationHandle = rotationMotionBuilder.BindToLocalRotation(context.Object);
        }
        else
        {
            moveHandle = moveMotionBuilder.BindToPosition(context.Object);
            rotationHandle = rotationMotionBuilder.BindToRotation(context.Object);
        }

        MotionSequenceBuilder seq = LSequence.Create().Join(moveHandle).Join(rotationHandle);
        return seq.Run().ToUniTask(token);
    }

    private UniTask AnimateHoverExit(in HoverExitContext context, CancellationToken token = default)
    {
        if (context.Object == null || context.TargetPosition == null)
            return UniTask.CompletedTask;

        Vector3 basePosition = context.Space == Space.Self ? context.Object.localPosition : context.Object.position;
        Quaternion baseRotation = context.Space == Space.Self ? context.Object.localRotation : context.Object.rotation;

        var moveMotionBuilder = LMotion.Create(basePosition, context.TargetPosition, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        var rotationMotionBuilder = LMotion.Create(baseRotation, Quaternion.Euler(context.TargetRotation), context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        MotionHandle moveHandle;
        MotionHandle rotationHandle;

        if (context.Space == Space.Self)
        {
            moveHandle = moveMotionBuilder.BindToLocalPosition(context.Object);
            rotationHandle = rotationMotionBuilder.BindToLocalRotation(context.Object);
        }
        else
        {
            moveHandle = moveMotionBuilder.BindToPosition(context.Object);
            rotationHandle = rotationMotionBuilder.BindToRotation(context.Object);
        }

        MotionSequenceBuilder seq = LSequence.Create().Join(moveHandle).Join(rotationHandle);
        return seq.Run().ToUniTask(token);
    }

    #endregion Animations

    [Header("Hover Animations")]
    [SerializeField] private HoverEnterContext _hoverEnter;
    [SerializeField] private HoverExitContext _hoverExit;

    private CancellationTokenSource _globalCTS;
    private CancellationTokenSource _hoverCTS;
    private CancellationTokenSource _moveCTS;

    public CancellationToken GlobalToken => _globalCTS?.Token ?? default;
    public CancellationToken HoverToken => _hoverCTS?.Token ?? default;
    public CancellationToken MoveToken => _moveCTS?.Token ?? default;

    private void Awake()
    {
        InitializeTokens();
    }

    private void InitializeTokens()
    {
        DisposeAllTokens();

        _globalCTS = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        _hoverCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS.Token);
    }

    private void DisposeAllTokens()
    {
        _globalCTS?.Cancel();
        _hoverCTS?.Cancel();
        _moveCTS?.Cancel();
    }

    public void ResetTokens()
    {
        DisposeAllTokens();
        InitializeTokens();
    }

    public async UniTask AnimateHoverEnter(CancellationToken token = default)
    {
        CancelHover();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, HoverToken);
        await AnimateHoverEnter(_hoverEnter, linkedCts.Token);
    }

    public async UniTask AnimateHoverExit(CancellationToken token = default)
    {
        CancelHover();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, HoverToken);
        await AnimateHoverExit(_hoverExit, linkedCts.Token);
    }

    [ContextMenu("Play: Animation Hover Enter")]
    public void PlayAnimateHoverEnter() => AnimateHoverEnter().Forget();

    [ContextMenu("Play: Animation Hover Exit")]
    public void PlayAnimateHoverExit() => AnimateHoverExit().Forget();

    [ContextMenu("Cancel: Hover")]
    public void CancelHover()
    {
        _hoverCTS?.Cancel();
        _hoverCTS?.Dispose();
        _hoverCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS?.Token ?? destroyCancellationToken);
    }
}
