using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using ScriptableAnimation;
using UnityEngine;
using static AnimationLibrary;

public class DisketController : MonoBehaviour
{
    private Animation<MoveToTargetContext> _hoverAnimation = MoveToTarget;

    [SerializeField]
    private MoveToTargetContext _enterHoverAnimationContext;

    [SerializeField]
    private MoveToTargetContext _exitHoverAnimationContext;

    private CancellationTokenSource _CTS = new();

    public void HoverEnter()
    {
        ResetCTS();
        _hoverAnimation.Invoke(_enterHoverAnimationContext, _CTS.Token);
    }

    public void HoverExit()
    {
        ResetCTS();
        _hoverAnimation.Invoke(_exitHoverAnimationContext, _CTS.Token);
    }

    private void ResetCTS()
    {
        _CTS?.Cancel();
        _CTS?.Dispose();
        _CTS = new();
    }
}
