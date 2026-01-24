using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using ScriptableAnimation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static AnimationLibrary;

public class DisketContainerController : MonoBehaviour
{
    private Animation<MoveAndRotateParallelContext> _discketContainerAnimation;

    private Animation<MoveToTargetContext> _hover = MoveToTarget;

    private Animation<MoveAndRotateParallelContext> GetAnim()
    {
        var anim = AnimationTools.Sequence<MoveAndRotateParallelContext>(
        parallel: true,
        (ctx, t) => AnimationLibrary.MoveToTarget(ctx.Move, t),
        (ctx, t) => AnimationLibrary.RotateEulerToTarget(ctx.Rotate, t));

        return anim;
    }

    void Awake()
    {
        _discketContainerAnimation = GetAnim();
    }

    private CancellationTokenSource _CTS = new();

    private CancellationTokenSource _Hover_CTS = new();

    [SerializeField]
    private MoveAndRotateParallelContext EnterAnimationContext;

    [SerializeField]
    private MoveAndRotateParallelContext ExitAnimationContext;

    [SerializeField]
    private MoveToTargetContext HoverEnter;

    [SerializeField]
    private MoveToTargetContext HoverExit;

    public int count = 0;
    public void OnInteract()
    {
        if (count % 2 != 0)
        {

            ResetHoverCTS();

            ResetCTS();
            EnterInteraction(_CTS.Token);
        }
        else
        {

            ResetHoverCTS();

            ResetCTS();
            ExitInteraction(_CTS.Token);
        }

        count++;
    }


    public void OnHoverEnter()
    {
        if (count % 2 == 0) return;
        ResetHoverCTS();
        _hover.Invoke(HoverEnter, _Hover_CTS.Token);
    }

    public void OnHoverExit()
    {
        if (count % 2 == 0) return;
        ResetHoverCTS();
        _hover.Invoke(HoverExit, _Hover_CTS.Token);
    }

    public void EnterInteraction(CancellationToken token = default)
    {
        _discketContainerAnimation.Invoke(EnterAnimationContext, token);
    }

    public void ExitInteraction(CancellationToken token = default)
    {
        _discketContainerAnimation.Invoke(ExitAnimationContext, token);
    }

    private void ResetHoverCTS()
    {
        _Hover_CTS?.Cancel();
        _Hover_CTS?.Dispose();
        _Hover_CTS = new();
    }

    private void ResetCTS()
    {
        _CTS?.Cancel();
        _CTS?.Dispose();
        _CTS = new();
    }

    void OnDestroy()
    {
        _CTS?.Cancel();
        _CTS?.Dispose();
        _CTS = null;
    }

    private void OnDrawGizmos()
    {

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

