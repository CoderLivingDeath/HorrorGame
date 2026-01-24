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

    [Header("Camera Target Enter Animation")]
    public CameraTarget CameraTargetAnimationEnterInteraction;
    public Ease EnterEase;
    public float EnterDuration = 1f;

    [Header("Camera Target Exit Animation")]
    public CameraTarget CameraTargetAnimationExitInteraction;
    public Ease ExitEase;
    public float ExitDuration = 1f;

    public Animation<CameraAnimationContext> cameraAnimation = CameraAnimation;
    [SerializeField]
    private PlayerController playerController;

    private static async UniTask CameraAnimation(CameraAnimationContext context, CancellationToken token = default)
    {
        await UniTask.WhenAll(
            AnimationLibrary.Move(new AnimationLibrary.MoveContext(
                obj: context.CameraTarget.TrackingTarget,
                start: context.CameraTarget.TrackingTarget.position,
                end: context.TargetPosition,
                duration: context.Duration,
                ease: context.Ease,
                space: AnimationLibrary.AnimationSpace.World
            ), token),
            AnimationLibrary.Rotate(new AnimationLibrary.RotateContext(
                context.CameraTarget.TrackingTarget,
                context.CameraTarget.TrackingTarget.localRotation,
                Quaternion.Inverse(context.CameraTarget.TrackingTarget.rotation) * context.TargetRotation,
                context.Duration,
                0f,
                context.Ease,
                space: AnimationLibrary.AnimationSpace.World
            ), token)
        );
    }

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
        CameraTarget.TrackingTarget.position = CameraTargetAnimationEnterInteraction.TrackingTarget.position;

        CameraTarget.TrackingTarget.position = CameraTargetAnimationEnterInteraction.TrackingTarget.position;
        CameraTarget.TrackingTarget.rotation = CameraTargetAnimationEnterInteraction.TrackingTarget.rotation;

        CameraTarget.LookAtTarget.position = CameraTargetAnimationEnterInteraction.LookAtTarget.position;

        playerController.SetCameraTarget(CameraTarget);
    }

    private UniTask GetAnimationMoveCameraToComputerTask(PlayerController playerController, CancellationToken token = default)
    {
        Vector3 targetPosition = CameraTargetAnimationEnterInteraction.TrackingTarget.position;
        CameraAnimationContext animationContext = new(
            cameraTarget: CameraTarget,
            targetPosition: targetPosition,
            targetRotation: CameraTargetAnimationEnterInteraction.TrackingTarget.rotation,
            ease: EnterEase,
            duration: EnterDuration);

        return cameraAnimation.OnBefore(() => PrepareBeforeAnimationMoveCameraToComputer(playerController))
            .Invoke(animationContext, token);
    }
    private UniTask GetAnimationMoveCameraToPlayerTask(PlayerController playerController, CancellationToken token = default)
    {
        Vector3 targetPosition = CameraTargetAnimationExitInteraction.TrackingTarget.position;
        CameraAnimationContext animationContext = new(
            cameraTarget: CameraTarget,
            targetPosition: targetPosition,
            targetRotation: CameraTargetAnimationEnterInteraction.TrackingTarget.rotation,
            ease: ExitEase,
            duration: ExitDuration);

        return cameraAnimation.OnBefore(() => PrepareBeforeAnimationMoveCameraToPlayer(playerController))
            .Invoke(animationContext, token);
    }

    public async UniTaskVoid OnEnterInteractWithComputer(CancellationToken token = default)
    {
        playerController.EnterComputerInteractionMode();
        UniTask animationTask = GetAnimationMoveCameraToComputerTask(playerController, token);

        playerController.IsAnimating = true;

        await animationTask;

        playerController.IsAnimating = false;

        // TODO: Добавить на Esc выход из интеракции
        // TODO: добавить отключение взаимодействия с другими объектами вне интерацкции с компьютером
    }

    public async UniTaskVoid OnExitInteractWithComputer(CancellationToken token = default)
    {
        PrepareBeforeAnimationMoveCameraToPlayer(playerController);

        UniTask animationTask = GetAnimationMoveCameraToPlayerTask(playerController, token);

        playerController.IsAnimating = true;

        await animationTask;

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
