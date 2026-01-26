using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Threading;
using UnityEngine;

public class DisketContainerAnimations : MonoBehaviour
{

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
    public struct MoveToPlayerContext
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
    public struct MoveToTableContext
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
    public struct OpenLidContext
    {
        public Transform Object;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public Space Space;
        public Vector3 TargetRotation;
    }

    [Serializable]
    public struct CloseLidContext
    {
        public Transform Object;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public Space Space;
        public Vector3 TargetRotation;
    }


    #region Animation methods
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

    private UniTask AnimateMoveToPlayer(in MoveToPlayerContext context, CancellationToken token = default)
    {
        if (context.Object == null || context.TargetPosition == null)
            return UniTask.CompletedTask;

        Vector3 basePosition = context.Space == Space.Self ? context.Object.localPosition : context.Object.position;
        Vector3 targetPosition = context.Space == Space.Self ?
            context.Object.InverseTransformPoint(context.TargetPosition.position) :
            context.TargetPosition.position;

        Quaternion baseRotation = context.Space == Space.Self ? context.Object.localRotation : context.Object.rotation;

        var moveMotionBuilder = LMotion.Create(basePosition, targetPosition, context.Duration)
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

    private UniTask AnimateMoveToTable(in MoveToTableContext context, CancellationToken token = default)
    {
        if (context.Object == null || context.TargetPosition == null)
            return UniTask.CompletedTask;

        Vector3 basePosition = context.Space == Space.Self ? context.Object.localPosition : context.Object.position;
        Vector3 targetPosition = context.Space == Space.Self ?
            context.Object.InverseTransformPoint(context.TargetPosition.position) :
            context.TargetPosition.position;

        Quaternion baseRotation = context.Space == Space.Self ? context.Object.localRotation : context.Object.rotation;

        var moveMotionBuilder = LMotion.Create(basePosition, targetPosition, context.Duration)
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

    private UniTask AnimateOpenLid(in OpenLidContext context, CancellationToken token = default)
    {
        if (context.Object == null)
            return UniTask.CompletedTask;

        Quaternion baseRotation = context.Space == Space.Self ? context.Object.localRotation : context.Object.rotation;
        Quaternion targetRotation = Quaternion.Euler(context.TargetRotation);

        var rotationMotionBuilder = LMotion.Create(baseRotation, targetRotation, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        MotionHandle rotationHandle = context.Space == Space.Self ?
            rotationMotionBuilder.BindToLocalRotation(context.Object) :
            rotationMotionBuilder.BindToRotation(context.Object);

        return rotationHandle.ToUniTask(token);
    }

    private UniTask AnimateCloseLid(in CloseLidContext context, CancellationToken token = default)
    {
        if (context.Object == null)
            return UniTask.CompletedTask;

        Quaternion baseRotation = context.Space == Space.Self ? context.Object.localRotation : context.Object.rotation;
        Quaternion targetRotation = Quaternion.Euler(context.TargetRotation);

        var rotationMotionBuilder = LMotion.Create(baseRotation, targetRotation, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        MotionHandle rotationHandle = context.Space == Space.Self ?
            rotationMotionBuilder.BindToLocalRotation(context.Object) :
            rotationMotionBuilder.BindToRotation(context.Object);

        return rotationHandle.ToUniTask(token);
    }


    #endregion

    #region Flags

    [Header("Flags")]
    [field: SerializeField] public bool CanHover { get; internal set; } = true;

    [field: SerializeField] public bool IsHoverEnter { get; private set; } = false;

    #endregion

    [Header("Hover Animations")]
    [SerializeField] private HoverEnterContext _hoverEnter;
    [SerializeField] private HoverExitContext _hoverExit;

    [Header("Move Animations")]
    [SerializeField] private MoveToPlayerContext _moveToPlayer;
    [SerializeField] private MoveToTableContext _moveToTable;

    [Header("Open/Close Animations")]
    [SerializeField] private OpenLidContext _openContainerLid;
    [SerializeField] private CloseLidContext _closeContainerLid;

    private CancellationTokenSource _globalCTS;
    private CancellationTokenSource _hoverCTS;
    private CancellationTokenSource _moveCTS;
    private CancellationTokenSource _openAndCloseCTS;

    public CancellationToken GlobalToken => _globalCTS?.Token ?? default;
    public CancellationToken HoverToken => _hoverCTS?.Token ?? default;
    public CancellationToken MoveToken => _moveCTS?.Token ?? default;
    public CancellationToken OpenAndCloseToken => _openAndCloseCTS?.Token ?? default;

    void Awake()
    {
        InitializeTokens();
    }

    public async UniTask AnimateHoverEnter(CancellationToken token = default)
    {
        if (!CanHover) return;

        CancelHover();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, HoverToken);
        await AnimateHoverEnter(_hoverEnter, linkedCts.Token);

        IsHoverEnter = true;
    }

    public async UniTask AnimateHoverExit(CancellationToken token = default)
    {
        if (!CanHover) return;
        IsHoverEnter = false;

        CancelHover();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, HoverToken);
        await AnimateHoverExit(_hoverExit, linkedCts.Token);
    }

    public async UniTask AnimateMoveToPlayer(CancellationToken token = default)
    {
        CancelMove();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, MoveToken);
        await AnimateMoveToPlayer(in _moveToPlayer, linkedCts.Token);
    }

    public async UniTask AnimateMoveToTable(CancellationToken token = default)
    {
        CancelMove();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, MoveToken);
        await AnimateMoveToTable(in _moveToTable, linkedCts.Token);
    }

    public async UniTask AnimateOpenLid(CancellationToken token = default)
    {
        CancelOpenClose();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, OpenAndCloseToken);
        await AnimateOpenLid(in _openContainerLid, linkedCts.Token);
    }

    public async UniTask AnimateCloseLid(CancellationToken token = default)
    {
        CancelOpenClose();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, OpenAndCloseToken);
        await AnimateCloseLid(in _closeContainerLid, linkedCts.Token);
    }

    private void InitializeTokens()
    {
        DisposeAllTokens();

        _globalCTS = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        _hoverCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS.Token);
        _moveCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS.Token);
        _openAndCloseCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS.Token);
    }

    private void DisposeAllTokens()
    {
        _globalCTS?.Cancel();
        _hoverCTS?.Cancel();
        _moveCTS?.Cancel();
        _openAndCloseCTS?.Cancel();

        _globalCTS?.Dispose();
        _hoverCTS?.Dispose();
        _moveCTS?.Dispose();
        _openAndCloseCTS?.Dispose();

        _globalCTS = null;
        _hoverCTS = null;
        _moveCTS = null;
        _openAndCloseCTS = null;
    }

    public void ResetTokens()
    {
        DisposeAllTokens();
        InitializeTokens();
    }

    [ContextMenu("Play: Animation Hover Enter")]
    public void PlayAnimationHoverEnter() => AnimateHoverEnter().Forget();

    [ContextMenu("Play: Animation Hover Exit")]
    public void PlayAnimationHoverExit() => AnimateHoverExit().Forget();

    [ContextMenu("Play: Animation Move To Player")]
    public void PlayAnimationMoveToPlayer() => AnimateMoveToPlayer().Forget();

    [ContextMenu("Play: Animation Move To Table")]
    public void PlayAnimationMoveToTable() => AnimateMoveToTable().Forget();

    [ContextMenu("Play: Animation Open Lid")]
    public void PlayAnimationOpenLid() => AnimateOpenLid().Forget();

    [ContextMenu("Play: Animation Close Lid")]
    public void PlayAnimationCloseLid() => AnimateCloseLid().Forget();

    [ContextMenu("Cancel: Hover")]
    public void CancelHover()
    {
        _hoverCTS?.Cancel();
        _hoverCTS?.Dispose();
        _hoverCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS?.Token ?? destroyCancellationToken);
    }

    [ContextMenu("Cancel: Move")]
    public void CancelMove()
    {
        _moveCTS?.Cancel();
        _moveCTS?.Dispose();
        _moveCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS?.Token ?? destroyCancellationToken);
    }

    [ContextMenu("Cancel: Open/Close")]
    public void CancelOpenClose()
    {
        _openAndCloseCTS?.Cancel();
        _openAndCloseCTS?.Dispose();
        _openAndCloseCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS?.Token ?? destroyCancellationToken);
    }

    [ContextMenu("Cancel: All")]
    public void CancelAll() => _globalCTS?.Cancel();

    [ContextMenu("Reset: All Tokens")]
    public void ResetAllTokens() => ResetTokens();

    void OnDestroy()
    {
        DisposeAllTokens();
    }
}
