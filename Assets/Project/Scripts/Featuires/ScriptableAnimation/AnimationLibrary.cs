using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using LitMotion;
using LitMotion.Extensions;
using ScriptableAnimation;

public static partial class AnimationLibrary
{

    public interface IAnimationContext
    {

    }

    [Serializable]
    public enum AnimationSpace
    {
        World,
        Local
    }

    [Serializable]
    public struct MoveContext : IAnimationContext
    {
        public Transform Object;
        public Vector3 Start;
        public Vector3 End;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public AnimationSpace Space;

        public MoveContext(Transform obj, Vector3 start = default, Vector3 end = default, float duration = 1, float delay = default, Ease ease = Ease.Linear, AnimationSpace space = default)
        {

            Object = obj;
            Start = start;
            End = end;
            Duration = duration;
            Delay = delay;
            Ease = ease;
            Space = space;
        }
    }

    [Serializable]
    public struct RotateContext : IAnimationContext
    {
        public Transform Object;
        public Quaternion Start;
        public Quaternion End;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public AnimationSpace Space;

        public RotateContext(Transform obj, Quaternion start = default, Quaternion end = default, float duration = 1, float delay = 0f, Ease ease = Ease.Linear, AnimationSpace space = default)
        {
            Object = obj;
            Start = start;
            End = end;
            Duration = duration;
            Delay = delay;
            Ease = ease;
            Space = space;
        }
    }

    public struct ScaleContext : IAnimationContext
    {
        public Transform Object;
        public Vector3 Start;
        public Vector3 End;
        public float Duration;
        public float Delay;
        public Ease Ease;

        public ScaleContext(Transform obj, Vector3 start = default, Vector3 end = default, float duration = 1, float delay = 0f, Ease ease = Ease.Linear)
        {
            Object = obj;
            Start = start;
            End = end;
            Duration = duration;
            Delay = delay;
            Ease = ease;
        }
    }


    [Serializable]
    public struct RotateEulerContext : IAnimationContext
    {

        public Transform Object;
        public Vector3 StartEuler;
        public Vector3 End;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public AnimationSpace Space;

        public RotateEulerContext(Transform obj, Vector3 startEuler = default, Vector3 endEuler = default,
            float duration = 1f, float delay = 0f, Ease ease = Ease.Linear, AnimationSpace space = AnimationSpace.World)
        {
            Object = obj;
            StartEuler = startEuler;
            End = endEuler;
            Duration = Mathf.Max(duration, 0.016f);
            Delay = delay;
            Ease = ease;
            Space = space;
        }
    }

    [Serializable]
    public struct MoveToTargetContext : IAnimationContext
    {
        public Transform Object;
        public Vector3 End;
        public float Duration;
        public float Delay;
        public Ease Ease;

        public AnimationSpace Space;

        public MoveToTargetContext(Transform obj,
            Vector3 relativeEnd,
            float duration = 1f,
            float delay = 0f,
            Ease ease = Ease.Linear,
            AnimationSpace space = default)
        {
            Object = obj;
            End = relativeEnd;
            Duration = duration;
            Delay = delay;
            Ease = ease;
            Space = space;
        }
    }

    [Serializable]
    public struct RotateEulerToTargetContext : IAnimationContext
    {
        public Transform Object;
        public Vector3 EndEuler;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public AnimationSpace Space;

        public RotateEulerToTargetContext(Transform obj, Vector3 endEuler = default,
            float duration = 1f, float delay = 0f, Ease ease = Ease.Linear, AnimationSpace space = AnimationSpace.World)
        {
            Object = obj;
            EndEuler = endEuler;
            Duration = Mathf.Max(duration, 0.016f);
            Delay = delay;
            Ease = ease;
            Space = space;
        }
    }


    public static Animation<MoveContext> MoveAnimation => (context, token) => Move(context, token);
    public static Animation<RotateContext> RotateAnimation => (context, token) => Rotate(context, token);
    public static Animation<ScaleContext> ScaleAnimation => (context, token) => Scale(context, token);
    public static Animation<RotateEulerContext> RotateEulerAnimation => (context, token) => RotateEuler(context, token);

    public static Animation<MoveToTargetContext> MoveToTargetAnimation => (context, token) => MoveToTarget(context, token);
    public static Animation<RotateEulerToTargetContext> RotateEulerRelativeAnimation => (context, token) => RotateEulerToTarget(context, token);

    public static async UniTask Move(MoveContext context, CancellationToken token = default)
    {
        MotionHandle motion;

        if (context.Space == AnimationSpace.World)
        {
            motion = LMotion.Create(context.Start, context.End, context.Duration)
                .WithEase(context.Ease)
                .WithDelay(context.Delay)
                .BindToPosition(context.Object);
        }
        else
        {
            motion = LMotion.Create(context.Start, context.End, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay)
            .BindToLocalPosition(context.Object);
        }

        await motion.ToUniTask(token);
    }

    public static async UniTask Rotate(RotateContext context, CancellationToken token = default)
    {
        MotionHandle motion;
        if (context.Space == AnimationSpace.World)
        {
            motion = LMotion.Create(context.Start, context.End, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay)
            .BindToRotation(context.Object);
        }
        else
        {
            motion = LMotion.Create(context.Start, context.End, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay)
            .BindToLocalRotation(context.Object);
        }

        await motion.ToUniTask(token);
    }

    public static async UniTask Scale(ScaleContext context, CancellationToken token = default)
    {
        var motion = LMotion.Create(context.Start, context.End, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay)
            .BindToLocalScale(context.Object);

        await motion.ToUniTask(token);
    }



    public static async UniTask RotateEuler(RotateEulerContext context, CancellationToken token = default)
    {
        if (context.Object == null) return;

        // Конвертируем Эйлер углы в Quaternion
        Quaternion startRot = Quaternion.Euler(context.StartEuler);
        Quaternion endRot = Quaternion.Euler(context.End);

        if (context.Space == AnimationSpace.World)
        {
            var motion = LMotion.Create(startRot, endRot, context.Duration)
                .WithEase(context.Ease)
                .WithDelay(context.Delay)
                .BindToRotation(context.Object);
            await motion.ToUniTask(token);
        }
        else
        {
            var motion = LMotion.Create(startRot, endRot, context.Duration)
                .WithEase(context.Ease)
                .WithDelay(context.Delay)
                .BindToLocalRotation(context.Object);
            await motion.ToUniTask(token);
        }
    }

    public static async UniTask MoveToTarget(MoveToTargetContext context, CancellationToken token = default)
    {
        if (context.Object == null) return;

        Vector3 basePosition = context.Space == AnimationSpace.Local
            ? context.Object.localPosition
            : context.Object.position;

        Vector3 startPosition = basePosition;
        Vector3 endPosition = context.End;

        var motion = LMotion.Create(startPosition, endPosition, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        MotionHandle handle;

        if (context.Space == AnimationSpace.Local)
            handle = motion.BindToLocalPosition(context.Object);
        else
            handle = motion.BindToPosition(context.Object);

        await handle.ToUniTask(cancellationToken: token);
    }

    public static async UniTask RotateEulerToTarget(RotateEulerToTargetContext context, CancellationToken token = default)
    {
        if (context.Object == null) return;

        Vector3 currentEuler = context.Space == AnimationSpace.Local 
            ? context.Object.localEulerAngles 
            : context.Object.eulerAngles;

        Vector3 delta = new Vector3(
            CalculateDeltaAngle(currentEuler.x, context.EndEuler.x),
            CalculateDeltaAngle(currentEuler.y, context.EndEuler.y),
            CalculateDeltaAngle(currentEuler.z, context.EndEuler.z)
        );

        Vector3 startEuler = currentEuler;
        Vector3 finalTarget = currentEuler + delta;

        if (context.Space == AnimationSpace.Local)
        {
            var motion = LMotion.Create(startEuler, finalTarget, context.Duration)
                .WithEase(context.Ease)
                .WithDelay(context.Delay)
                .BindToLocalEulerAngles(context.Object);
            await motion.ToUniTask(token);
        }
        else
        {
            var motion = LMotion.Create(startEuler, finalTarget, context.Duration)
                .WithEase(context.Ease)
                .WithDelay(context.Delay)
                .BindToEulerAngles(context.Object);
            await motion.ToUniTask(token);
        }
    }

    private static float CalculateDeltaAngle(float current, float target)
    {
        if (Mathf.Abs(target) >= 360f)
            return target - current;
        
        return Mathf.DeltaAngle(current, target);
    }
}
