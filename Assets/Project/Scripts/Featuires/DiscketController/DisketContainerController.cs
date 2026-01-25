using Cysharp.Threading.Tasks;
using LitMotion;
using Reflex.Attributes;
using ScriptableAnimation;
using System;
using System.Threading;
using UnityEngine;
using static AnimationLibrary;

public class DisketContainerController : MonoBehaviour
{
    private Animation<MoveAndRotateParallelContext> _discketContainerAnimation;

    private Animation<MoveToTargetContext> _hover = MoveToTarget;

    #region CTS

    private CancellationTokenSource _globalCTS = new();

    private CancellationTokenSource _interactionCTS = new();

    private CancellationTokenSource _hover_CTS = new();

    #endregion

    #region Flags

    private bool _isAnimationg = false;

    private bool _canHover = true;

    #endregion

    [Inject]
    private InputEventBus _InputEventBus;

    [SerializeField]
    private MoveAndRotateParallelContext EnterAnimationContext;

    [SerializeField]
    private MoveAndRotateParallelContext ExitAnimationContext;

    [SerializeField]
    private MoveToTargetContext HoverEnter;

    [SerializeField]
    private MoveToTargetContext HoverExit;

    [SerializeField]
    private RotateEulerToTargetContext LidOpenAnimation;

    [SerializeField]
    private RotateEulerToTargetContext LidCloseAnimation;

    [SerializeField]
    private Collider Collider;

    void Awake()
    {
        _discketContainerAnimation = GetAnim();
    }

    private Animation<MoveAndRotateParallelContext> GetAnim()
    {
        var anim = AnimationTools.Sequence<MoveAndRotateParallelContext>(
        parallel: true,
        (ctx, t) => MoveToTarget(ctx.Move, t),
        (ctx, t) => RotateEulerToTarget(ctx.Rotate, t));

        return anim;
    }

    private bool CanInteract()
    {
        return !_isAnimationg;
    }

    public void OnInteract()
    {
        if (!CanInteract()) return;

        ResetHoverCTS();

        ResetCTS();
        EnterInteraction(_interactionCTS.Token).Forget();
    }

    public void OnHoverEnter()
    {
        if (!_canHover) return;

        ResetHoverCTS();
        _hover.Invoke(HoverEnter, _hover_CTS.Token);
    }

    public void OnHoverExit()
    {
        if (!_canHover) return;

        ResetHoverCTS();
        _hover.Invoke(HoverExit, _hover_CTS.Token);
    }

    public async UniTask EnterInteraction(CancellationToken token = default)
    {
        _isAnimationg = true;
        _canHover = false;

        await _discketContainerAnimation.Invoke(EnterAnimationContext, token);
        Collider.enabled = false;
        _InputEventBus.UI.OnCancel += _gameplayInputEventBus_OnCancel;

        _isAnimationg = false;
    }

    private void _gameplayInputEventBus_OnCancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ResetHoverCTS();

        ResetCTS();
        ExitInteraction(_interactionCTS.Token).Forget();
    }

    public async UniTask ExitInteraction(CancellationToken token = default)
    {

        _isAnimationg = true;

        await _discketContainerAnimation.Invoke(ExitAnimationContext, token);

        Collider.enabled = true;

        _isAnimationg = false;
        _canHover = true;
    }

    [ContextMenu("Reset Global CTS")]
    private void ResetGlobalCTS()
    {
        _globalCTS?.Cancel();
        _globalCTS?.Dispose();
        _globalCTS = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
    }

    private void ResetHoverCTS()
    {
        _hover_CTS?.Cancel();
        _hover_CTS?.Dispose();
        _hover_CTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS.Token);
    }

    private void ResetCTS()
    {
        _interactionCTS?.Cancel();
        _interactionCTS?.Dispose();
        _interactionCTS = CancellationTokenSource.CreateLinkedTokenSource(_globalCTS.Token);
    }

    void OnDestroy()
    {
        _interactionCTS?.Cancel();
        _interactionCTS?.Dispose();
        _interactionCTS = null;

        _hover_CTS?.Cancel();
        _hover_CTS?.Dispose();
        _hover_CTS = null;
    }
}

[Serializable]
public struct MoveAndRotateParallelContext : IAnimationContext
{
    public AnimationLibrary.MoveToTargetContext Move; // struct MoveContext : IAnimationContext
    public AnimationLibrary.RotateEulerToTargetContext Rotate; // struct RotateEulerContext : IAnimationContext

    public MoveAndRotateParallelContext(AnimationLibrary.MoveToTargetContext move, AnimationLibrary.RotateEulerToTargetContext rotate)
    {
        Move = move;
        Rotate = rotate;
    }
}

