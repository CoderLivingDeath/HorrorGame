using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ComputerController : MonoBehaviour
{
    private CancellationTokenSource _CTS = new();

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private ComputerAnimations _computerAnimations;

    private DisketController _InjectedDisket;

    public void SetInjectedDisket(DisketController disket)
    {
        _InjectedDisket = disket;
    }

    public async UniTaskVoid OnEnterInteractWithComputer(CancellationToken token = default)
    {
        playerController.EnterComputerInteractionMode();

        await _computerAnimations.AnimateMoveCameraToComputer(token);
    }

    public async UniTaskVoid OnExitInteractWithComputer(CancellationToken token = default)
    {
        await _computerAnimations.AnimateMoveCameraToPlayer(token);

        playerController.EnterRotationMode();
    }

    public async UniTaskVoid EjectDisket()
    {
        if (_InjectedDisket == null)
        {
            Debug.LogWarning("Disk not injected");
            return;
        }

        _InjectedDisket.gameObject.SetActive(true);
        await _InjectedDisket.DisketAnimations.AnimateMoveFromInjector();
        await _InjectedDisket.DisketAnimations.AnimateMoveToContainer();
    }

    public void OnInteract()
    {
        if (playerController.CurrentState == PlayerController.State.Rotatation)
        {
            ResetCTS();
            OnEnterInteractWithComputer(_CTS.Token).Forget();
        }
        else
        {
            ResetCTS();
            OnExitInteractWithComputer(_CTS.Token).Forget();
        }
    }

    [ContextMenu("TEST")]
    public void Test()
    {
        EjectDisket().Forget();
    }

    private void ResetCTS()
    {
        _CTS?.Cancel();
        _CTS?.Dispose();
        _CTS = new();
    }
}
