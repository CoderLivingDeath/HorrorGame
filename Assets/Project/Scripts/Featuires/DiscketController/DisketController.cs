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

    private CancellationTokenSource _CTS = new();

    public void HoverEnter()
    {
        ResetCTS();
    }

    public void HoverExit()
    {
        ResetCTS();
    }

    private void ResetCTS()
    {
        _CTS?.Cancel();
        _CTS?.Dispose();
        _CTS = new();
    }
}
