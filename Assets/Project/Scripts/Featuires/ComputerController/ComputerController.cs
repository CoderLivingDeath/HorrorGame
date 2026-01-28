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

    private void ResetCTS()
    {
        _CTS?.Cancel();
        _CTS?.Dispose();
        _CTS = new();
    }
}
