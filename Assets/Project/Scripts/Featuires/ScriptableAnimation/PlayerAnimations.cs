using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Threading;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    #region Animations

    [Serializable]
    public struct RotationContextDTO
    {
        public Transform Object;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public Space Space;

        public RotationContext ToRotationContext(Vector3 rotation)
        {
            return new()
            {
                Object = Object,
                Duration = Duration,
                Delay = Delay,
                Space = Space,
                Ease = Ease,
                TargetRotation = rotation
            };
        }
    }


    [Serializable]
    public struct RotationContext
    {
        public Transform Object;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public Space Space;
        public Vector3 TargetRotation;
    }

    private UniTask AnimateRotation(in RotationContext context, CancellationToken token = default)
    {
        Quaternion baseRotation = context.Space == Space.Self 
            ? context.Object.localRotation : context.Object.rotation;

        Quaternion TargetRotation = Quaternion.Euler(context.TargetRotation);

        var builder = LMotion.Create(baseRotation, TargetRotation, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        MotionHandle handle = context.Space == Space.Self
            ? builder.BindToLocalRotation(context.Object) : builder.BindToRotation(context.Object);

        return handle.ToUniTask(token);
    }

    #endregion

    #region Properies

    [field: SerializeField] public int[] RotationAngles;
    [field: SerializeField] public int CurrentAngleId;

    #endregion

    #region CTS

    private CancellationTokenSource _globalCTS;
    private CancellationTokenSource _RotationCTS;

    private CancellationTokenSource CreateGlobalCTS() => CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
    private CancellationTokenSource CreateRotationCTS() => CancellationTokenSource.CreateLinkedTokenSource(_globalCTS.Token);

    #endregion

    #region Contexts

    [Header("Animations")]

    [SerializeField] private RotationContextDTO _Rotation;

    #endregion

    private void Awake()
    {
        _globalCTS = CreateGlobalCTS();
        _RotationCTS = CreateRotationCTS();
    }


    private Vector3 GetLeftTargetRotation()
    {
        int nextId = (CurrentAngleId - 1 + RotationAngles.Length) % RotationAngles.Length;
        return new Vector3(0, RotationAngles[nextId], 0);
    }

    private Vector3 GetRightTargetRotation()
    {
        int nextId = (CurrentAngleId + 1) % RotationAngles.Length;
        return new Vector3(0, RotationAngles[nextId], 0);
    }

    public async UniTask AnimateRotationLeft(CancellationToken token = default)
    {
        CancelRotationCTS();

        Vector3 targetRotation = GetLeftTargetRotation();

        // Change CurrentAndleId
        CurrentAngleId = (CurrentAngleId - 1 + RotationAngles.Length) % RotationAngles.Length;

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, _RotationCTS.Token);
        await AnimateRotation(_Rotation.ToRotationContext(targetRotation), linkedCts.Token);
    }

    public async UniTask AnimateRotationRight(CancellationToken token = default)
    {
        CancelRotationCTS();

        Vector3 targetRotation = GetRightTargetRotation();

        // Change CurrentAndleId
        CurrentAngleId = (CurrentAngleId + 1) % RotationAngles.Length;

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, _RotationCTS.Token);
        await AnimateRotation(_Rotation.ToRotationContext(targetRotation), linkedCts.Token);
    }


    [ContextMenu("Player: Rotation Left")]
    public void PlayAnimationRotationLeft()
    {
        AnimateRotationLeft().Forget();
    }

    [ContextMenu("Player: Rotation Right")]
    public void PlayAnimationRotationRight()
    {
        AnimateRotationRight().Forget();
    }

    [ContextMenu("Cancel: Rotation")]
    public void CancelRotationCTS()
    {
        _RotationCTS?.Cancel();
        _RotationCTS?.Dispose();
        _RotationCTS = CreateRotationCTS();
    }
}
