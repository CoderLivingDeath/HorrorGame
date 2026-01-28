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

    [Serializable]
    public struct InjectDisketContext
    {
        [Serializable]
        public struct MoveToComputerContext
        {
            public Transform Object;
            public float Duration;
            public float Delay;
            public Ease Ease;
            public Space Space;
            public Transform TargetPosition;
            public Vector3 TargetRotation;
        }

        [Serializable]
        public struct MoveToInjectorContext
        {
            public Transform Object;
            public float Duration;
            public float Delay;
            public Ease Ease;
            public Space Space;
            public Transform TargetPosition;
        }

        public MoveToComputerContext MoveToComputer;
        public MoveToInjectorContext MoveToInjector;
    }

    public UniTask AnimateMoveToComputer(in InjectDisketContext.MoveToComputerContext context, CancellationToken token = default)
    {
        if (context.Object == null || context.TargetPosition == null)
            return UniTask.CompletedTask;

        Vector3 basePosition = context.Space == Space.Self ? context.Object.localPosition : context.Object.position;

        var moveMotionBuilder = LMotion.Create(basePosition, context.TargetPosition.position, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        Quaternion baseRotation = context.Space == Space.Self
            ? context.Object.localRotation : context.Object.rotation;

        Quaternion targetRotation = Quaternion.Euler(context.TargetRotation);

        var rotationBuilder = LMotion.Create(baseRotation, targetRotation, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        MotionHandle moveHandle;
        MotionHandle rotationHandle;

        if (context.Space == Space.Self)
        {
            moveHandle = moveMotionBuilder.BindToLocalPosition(context.Object);
            rotationHandle = rotationBuilder.BindToLocalRotation(context.Object);
        }
        else
        {
            moveHandle = moveMotionBuilder.BindToPosition(context.Object);
            rotationHandle = rotationBuilder.BindToRotation(context.Object);
        }

        return UniTask.WhenAll(moveHandle.ToUniTask(token), rotationHandle.ToUniTask(token));
    }

    public UniTask AnimateMoveToInjector(in InjectDisketContext.MoveToInjectorContext context, CancellationToken token = default)
    {
        if (context.Object == null || context.TargetPosition == null)
            return UniTask.CompletedTask;

        Vector3 basePosition = context.Space == Space.Self ? context.Object.localPosition : context.Object.position;

        var moveMotionBuilder = LMotion.Create(basePosition, context.TargetPosition.position, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        MotionHandle moveHandle;

        if (context.Space == Space.Self)
        {
            moveHandle = moveMotionBuilder.BindToLocalPosition(context.Object);
        }
        else
        {
            moveHandle = moveMotionBuilder.BindToPosition(context.Object);
        }

        return moveHandle.ToUniTask(token);
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

    #region Properties

    [field: SerializeField] public bool CanHover { get; set; } = true;

    #endregion

    [Header("Hover Animations")]
    [SerializeField] private HoverEnterContext _hoverEnter;
    [SerializeField] private HoverExitContext _hoverExit;

    [Header("Inject Animations")]
    [SerializeField] private InjectDisketContext _injectDisket;

    private CancellationTokenSource _globalCTS;
    private CancellationTokenSource _hoverCTS;
    private CancellationTokenSource _InjectCTS;

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
        _InjectCTS?.Cancel();
    }

    public void ResetTokens()
    {
        DisposeAllTokens();
        InitializeTokens();
    }

    public async UniTask AnimateHoverEnter(CancellationToken token = default)
    {
        if (!CanHover) return;

        CancelHover();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, _hoverCTS.Token);
        await AnimateHoverEnter(_hoverEnter, linkedCts.Token);
    }

    public async UniTask AnimateHoverExit(CancellationToken token = default)
    {
        if (!CanHover) return;

        CancelHover();

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(token, _hoverCTS.Token);
        await AnimateHoverExit(_hoverExit, linkedCTS.Token);
    }

    public async UniTask AnimateMoveToComputer(CancellationToken token = default)
    {
        CancelInject();

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(token, _InjectCTS.Token);
        await AnimateMoveToComputer(_injectDisket.MoveToComputer, linkedCTS.Token);
    }

    public async UniTask AnimateMoveToInjector(CancellationToken token = default)
    {
        CancelInject();

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(token, _InjectCTS.Token);
        await AnimateMoveToInjector(_injectDisket.MoveToInjector, linkedCTS.Token);
    }

    [ContextMenu("Play: Animation Hover Enter")]
    public void PlayAnimateHoverEnter() => AnimateHoverEnter().Forget();

    [ContextMenu("Play: Animation Hover Exit")]
    public void PlayAnimateHoverExit() => AnimateHoverExit().Forget();

    [ContextMenu("Play: Animation Move To Computer")]
    public void PlayAnimationMoveToComputer() => AnimateMoveToComputer().Forget();

    [ContextMenu("Play: Animation Move To Injectr")]
    public void PlayAnimationMoveToInjector() => AnimateMoveToInjector().Forget();

    [ContextMenu("Cancel: Hover")]
    public void CancelHover()
    {
        _hoverCTS?.Cancel();
        _hoverCTS?.Dispose();
        _hoverCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS?.Token ?? destroyCancellationToken);
    }

    [ContextMenu("Cancel: Inject")]
    public void CancelInject()
    {
        _InjectCTS?.Cancel();
        _InjectCTS?.Dispose();
        _InjectCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS?.Token ?? destroyCancellationToken);
    }
}
