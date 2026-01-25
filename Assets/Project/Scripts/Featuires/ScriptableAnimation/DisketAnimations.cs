using ScriptableAnimation;
using System;
using System.Threading;
using UnityEngine;
using static AnimationLibrary;

public class DisketAnimations : MonoBehaviour
{
    [Header("Hover Animations")]
    [SerializeField] private MoveToTargetContext _hoverEnter;
    [SerializeField] private MoveToTargetContext _hoverExit;

    [Header("Move Animations")]
    [SerializeField] private MoveToTargetContext _moveToPlayer;
    [SerializeField] private MoveToTargetContext _moveToTable;

    [Header("Open/Close Animations")]
    [SerializeField] private RotateEulerToTargetContext _openContainerLid;
    [SerializeField] private RotateEulerToTargetContext _closeContainerLid;

    // Токены отмены
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

    private CancellationTokenSource CreateGlobalCTS() =>
        CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

    private CancellationTokenSource CreateMoveCTS() =>
        CancellationTokenSource.CreateLinkedTokenSource(GlobalToken);

    private CancellationTokenSource CreateHoverCTS() =>
        CancellationTokenSource.CreateLinkedTokenSource(GlobalToken);

    private CancellationTokenSource CreateOpenCloseCTS() =>
        CancellationTokenSource.CreateLinkedTokenSource(GlobalToken);

    private void InitializeTokens()
    {
        DisposeAllTokens();
        _globalCTS = CreateGlobalCTS();
        _hoverCTS = CreateHoverCTS();
        _moveCTS = CreateMoveCTS();
        _openAndCloseCTS = CreateOpenCloseCTS();
    }

    private void DisposeAllTokens()
    {
        _globalCTS?.Dispose();
        _hoverCTS?.Dispose();
        _moveCTS?.Dispose();
        _openAndCloseCTS?.Dispose();
    }

    public void ResetTokens() => InitializeTokens();

    public void CancelHover()
    {
        _hoverCTS?.Cancel();
        _hoverCTS?.Dispose();
        _hoverCTS = CreateHoverCTS();
    }

    public void CancelMove()
    {
        _moveCTS?.Cancel();
        _moveCTS?.Dispose();
        _moveCTS = CreateMoveCTS();
    }

    public void CancelOpenClose()
    {
        _openAndCloseCTS?.Cancel();
        _openAndCloseCTS?.Dispose();
        _openAndCloseCTS = CreateOpenCloseCTS();
    }

    // Глобальная отмена (каскад через все linked токены)
    public void CancelAll() => _globalCTS?.Cancel();

    void OnDestroy()
    {
        DisposeAllTokens();
    }
}
