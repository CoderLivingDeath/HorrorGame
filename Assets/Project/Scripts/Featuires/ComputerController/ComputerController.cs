using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using ScriptableAnimation;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerController : MonoBehaviour
{
    private CancellationTokenSource _CTS = new();

    [Header("CameraTarget")]

    public CameraTarget CameraTarget;

    private PlayerController playerController;

    private void PrepareBeforeAnimationMoveCameraToComputer(PlayerController playerController)
    {
        var playerControllerCameraTarget = playerController.CameraTarget;

        CameraTarget.TrackingTarget.position = playerControllerCameraTarget.TrackingTarget.position;
        CameraTarget.TrackingTarget.rotation = playerControllerCameraTarget.TrackingTarget.rotation;

        CameraTarget.LookAtTarget.position = playerControllerCameraTarget.LookAtTarget.position;

        playerController.SetCameraTarget(CameraTarget);
    }

    private void PrepareBeforeAnimationMoveCameraToPlayer(PlayerController playerController)
    {
    }


    public async UniTaskVoid OnEnterInteractWithComputer(CancellationToken token = default)
    {
        playerController.EnterComputerInteractionMode();

        playerController.IsAnimating = true;


        playerController.IsAnimating = false;

        // TODO: Добавить на Esc выход из интеракции
        // TODO: добавить отключение взаимодействия с другими объектами вне интерацкции с компьютером
    }

    public async UniTaskVoid OnExitInteractWithComputer(CancellationToken token = default)
    {
        PrepareBeforeAnimationMoveCameraToPlayer(playerController);


        playerController.IsAnimating = true;


        playerController.IsAnimating = false;

        playerController.SetCameraTarget(playerController.CameraTarget);
        playerController.EnterRotationMode();

        // TODO: Добавить на Esc выход из интеракции
        // TODO: добавить отключение взаимодействия с другими объектами вне интерацкции с компьютером
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

[Serializable]
public struct CameraAnimationContext
{
    public CameraTarget CameraTarget;
    public Vector3 TargetPosition;
    public Quaternion TargetRotation;
    public Ease Ease;
    public float Duration;

    public CameraAnimationContext(CameraTarget cameraTarget, Vector3 targetPosition = default, Quaternion targetRotation = default,
        Ease ease = default, float duration = default)
    {
        CameraTarget = cameraTarget;
        TargetPosition = targetPosition;
        TargetRotation = targetRotation;
        Ease = ease;
        Duration = duration;
    }
}
