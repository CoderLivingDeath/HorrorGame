using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using ScriptableAnimation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using UnityEngine;

public static partial class AnimationLibrary
{
    #region reflection
    private static readonly ConcurrentDictionary<Type, Func<BaseAnimationContext, CancellationToken, UniTask>>
    _animationCache = new();

    // Универсальный кэш для Animation<T>
    private static readonly ConcurrentDictionary<Type, Delegate> _genericAnimationCache = new();

    /// <summary>
    /// Пустая анимация для каждого типа T
    /// </summary>
    private static class EmptyAnimation<T> where T : BaseAnimationContext
    {
        public static readonly Animation<T> Func = (_, _) => UniTask.CompletedTask;
    }

    private static readonly Func<BaseAnimationContext, CancellationToken, UniTask> EmptyAnimationGeneric
    = (context, token) => UniTask.CompletedTask;


    /// <summary>
    /// Создать делегат Animation<T> через Expression Trees
    /// </summary>
    private static Animation<T> CreateAnimationDelegate<T>() where T : BaseAnimationContext
    {
        var contextType = typeof(T);

        var method = typeof(AnimationLibrary)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.GetCustomAttribute<AnimationMethodAttribute>() is { } attr
                && attr.ContextType == contextType);

        if (method == null)
        {
            Debug.LogWarning($"No Animation method for {contextType.Name}");
            return EmptyAnimation<T>.Func;
        }

        var contextParam = Expression.Parameter(contextType, "context");
        var tokenParam = Expression.Parameter(typeof(CancellationToken), "token");

        var call = Expression.Call(method, contextParam, tokenParam);
        var lambda = Expression.Lambda<Animation<T>>(call, contextParam, tokenParam);

        var compiled = lambda.Compile();

        return compiled;
    }

    private static Func<BaseAnimationContext, CancellationToken, UniTask> GetCachedAnimation(Type contextType)
    {
        return _animationCache.GetOrAdd(contextType, type =>
        {
            var method = typeof(AnimationLibrary)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.GetCustomAttribute<AnimationMethodAttribute>() is { } attr
                    && attr.ContextType == type);

            if (method == null)
                return EmptyAnimationGeneric;

            var targetContextType = method.GetParameters()[0].ParameterType;

            var contextParam = Expression.Parameter(typeof(BaseAnimationContext), "context");
            var tokenParam = Expression.Parameter(typeof(CancellationToken), "token");

            var castedContext = Expression.Convert(contextParam, targetContextType);

            var call = Expression.Call(method, castedContext, tokenParam);

            var lambda = Expression.Lambda<Func<BaseAnimationContext, CancellationToken, UniTask>>(
                call, contextParam, tokenParam);

            var compiled = lambda.Compile();
            return compiled;
        });
    }

    public static async UniTask PlayAnimation(BaseAnimationContext context, CancellationToken token = default)
    {
        if (context == null) return;

        var contextType = context.GetType();
        var animationFunc = GetCachedAnimation(contextType);

        if (animationFunc == null)
        {
            Debug.LogError($"❌ No animation for {contextType.Name}");
            return;
        }

        try
        {
            await animationFunc(context, token);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Получить типобезопасный Animation<T> для конкретного типа контекста
    /// </summary>
    public static Animation<T> GetAnimation<T>() where T : BaseAnimationContext
    {
        var contextType = typeof(T);

        if (_genericAnimationCache.TryGetValue(contextType, out var cachedDelegate))
            return (Animation<T>)cachedDelegate;

        var animationDelegate = CreateAnimationDelegate<T>();
        _genericAnimationCache[contextType] = animationDelegate;
        return animationDelegate;
    }
    #endregion

    public interface IAnimationContext
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AnimationMethodAttribute : Attribute
    {
        public Type ContextType;
    }

    [Serializable]
    public abstract class BaseAnimationContext : IAnimationContext
    {
        public Transform Object;
        public float Duration;
        public float Delay;
        public Ease Ease;
        public AnimationSpace Space;

        protected BaseAnimationContext()
        {

        }

        protected BaseAnimationContext(Transform obj, float duration = 1, float delay = 0, Ease ease = Ease.Linear, AnimationSpace space = default)
        {
            Object = obj;
            Duration = duration;
            Delay = delay;
            Ease = ease;
            Space = space;
        }
    }

    [Serializable]
    public enum AnimationSpace
    {
        World,
        Local
    }

    [Serializable]
    public class MoveToTransformContext : BaseAnimationContext
    {
        public Transform Target;

        public MoveToTransformContext()
        {

        }

        public MoveToTransformContext(Transform @object, Transform target, float Duration = 1, float delay = 0, Ease ease = Ease.Linear, AnimationSpace space = AnimationSpace.World) : base(@object, Duration, delay, ease, space)
        {
            Target = target;
        }

    }

    [Serializable]
    public class MoveContext : BaseAnimationContext
    {
        public Vector3 Start;
        public Vector3 End;

        public MoveContext()
        {

        }

        public MoveContext(Transform obj, Vector3 start = default, Vector3 end = default, float duration = 1, float delay = default, Ease ease = Ease.Linear, AnimationSpace space = default)
            : base(obj, duration, delay, ease, space)
        {
            Start = start;
            End = end;
        }

        public static ScriptableAnimation.Animation<MoveContext> GetAnimationWithContext()
        {
            return Move;
        }

        public static MoveContext Create(Transform target, Vector3 targetPos, float duration = 1f, float delay = 0f, Ease ease = Ease.InOutQuad)
        {
            return new MoveContext(target, target.position, targetPos, duration, delay, ease);
        }
        public ScriptableAnimation.Animation GetAnimation()
        {
            var animation = GetAnimationWithContext();
            return async (token) => await animation.Invoke(this, token);
        }

        public async UniTask Animate(CancellationToken token = default)
        {
            await GetAnimation().Invoke(token);
        }
    }

    [Serializable]
    public class RotateContext : BaseAnimationContext
    {
        public Quaternion Start;
        public Quaternion End;

        public RotateContext()
        {

        }

        public RotateContext(Transform obj, Quaternion start = default, Quaternion end = default, float duration = 1, float delay = 0f, Ease ease = Ease.Linear, AnimationSpace space = default)
            : base(obj, duration, delay, ease, space)
        {
            Start = start;
            End = end;
        }
    }

    [Serializable]
    public class ScaleContext : BaseAnimationContext
    {
        public Vector3 Start;
        public Vector3 End;


        public ScaleContext()
        {

        }
        public ScaleContext(Transform obj, Vector3 start = default, Vector3 end = default, float duration = 1, float delay = 0f, Ease ease = Ease.Linear)
            : base(obj, duration, delay, ease, AnimationSpace.Local)
        {
            Start = start;
            End = end;
        }
    }


    [Serializable]
    public class RotateEulerContext : BaseAnimationContext
    {
        public Vector3 StartEuler;
        public Vector3 End;

        public RotateEulerContext()
        {

        }
        public RotateEulerContext(Transform obj, Vector3 startEuler = default, Vector3 endEuler = default,
            float duration = 1f, float delay = 0f, Ease ease = Ease.Linear, AnimationSpace space = AnimationSpace.World)
            : base(obj, Mathf.Max(duration, 0.016f), delay, ease, space)
        {
            StartEuler = startEuler;
            End = endEuler;
        }
    }

    [Serializable]
    public class MoveToTargetContext : BaseAnimationContext
    {
        public Vector3 End;

        public MoveToTargetContext()
        {

        }

        public MoveToTargetContext(Transform obj,
            Vector3 relativeEnd,
            float duration = 1f,
            float delay = 0f,
            Ease ease = Ease.Linear,
            AnimationSpace space = default)
            : base(obj, duration, delay, ease, space)
        {
            End = relativeEnd;
        }

        public static Animation<MoveToTargetContext> GetAnimation() => MoveToTarget;
    }

    [Serializable]
    public class RotateEulerToTargetContext : BaseAnimationContext
    {
        public Vector3 EndEuler;

        public RotateEulerToTargetContext()
        {

        }
        public RotateEulerToTargetContext(Transform obj, Vector3 endEuler = default,
            float duration = 1f, float delay = 0f, Ease ease = Ease.Linear, AnimationSpace space = AnimationSpace.World)
            : base(obj, Mathf.Max(duration, 0.016f), delay, ease, space)
        {
            EndEuler = endEuler;
        }

        public static Animation<RotateEulerToTargetContext> GetAnimation() => RotateEulerToTarget;
    }

    [Serializable]
    public class MoveToTargetDynamicContext : BaseAnimationContext
    {
        public Func<Vector3> EndSelector;

        public MoveToTargetDynamicContext()
        {

        }

        public MoveToTargetDynamicContext(
            Transform obj,
            Func<Vector3> endSelector,
            float duration = 1,
            float delay = 0,
            Ease ease = Ease.Linear,
            AnimationSpace space = AnimationSpace.World)
            : base(obj, duration, delay, ease, space)
        {
            EndSelector = endSelector ?? throw new ArgumentNullException(nameof(endSelector));
        }
    }

    [AnimationMethod(ContextType = typeof(MoveToTargetDynamicContext))]
    public static async UniTask MoveToTargetDynamic(MoveToTargetDynamicContext context, CancellationToken token = default)
    {
        // Start = текущая позиция объекта
        Vector3 start = context.Space == AnimationSpace.World
            ? context.Object.position
            : context.Object.localPosition;

        Vector3 end = context.EndSelector.Invoke();

        MotionHandle motion = context.Space == AnimationSpace.World
            ? LMotion.Create(start, end, context.Duration)
                .WithEase(context.Ease)
                .WithDelay(context.Delay)
                .BindToPosition(context.Object)
            : LMotion.Create(start, end, context.Duration)
                .WithEase(context.Ease)
                .WithDelay(context.Delay)
                .BindToLocalPosition(context.Object);

        await motion.ToUniTask(token);
    }

    [Serializable]
    public class MoveDynamicContext : BaseAnimationContext
    {
        public Func<Vector3> StartSelector;
        public Func<Vector3> EndSelector;
        public MoveDynamicContext()
        {

        }
        public MoveDynamicContext(
            Transform obj,
            Func<Vector3> startSelector,
            Func<Vector3> endSelector,
            float duration = 1,
            float delay = 0,
            Ease ease = Ease.Linear,
            AnimationSpace space = AnimationSpace.World)
            : base(obj, duration, delay, ease, space)
        {
            StartSelector = startSelector ?? throw new ArgumentNullException(nameof(startSelector));
            EndSelector = endSelector ?? throw new ArgumentNullException(nameof(endSelector));
        }
    }

    [AnimationMethod(ContextType = typeof(MoveDynamicContext))]
    public static async UniTask MoveDynamic(MoveDynamicContext context, CancellationToken token = default)
    {
        Vector3 start = context.StartSelector.Invoke();
        Vector3 end = context.EndSelector.Invoke();

        MotionHandle motion = context.Space == AnimationSpace.World
            ? LMotion.Create(start, end, context.Duration)
                .WithEase(context.Ease)
                .WithDelay(context.Delay)
                .BindToPosition(context.Object)
            : LMotion.Create(start, end, context.Duration)
                .WithEase(context.Ease)
                .WithDelay(context.Delay)
                .BindToLocalPosition(context.Object);

        await motion.ToUniTask(token);
    }

    public static Animation<MoveContext> MoveAnimation => (context, token) => Move(context, token);
    public static Animation<RotateContext> RotateAnimation => (context, token) => Rotate(context, token);
    public static Animation<ScaleContext> ScaleAnimation => (context, token) => Scale(context, token);
    public static Animation<RotateEulerContext> RotateEulerAnimation => (context, token) => RotateEuler(context, token);

    public static Animation<MoveToTargetContext> MoveToTargetAnimation => (context, token) => MoveToTarget(context, token);
    public static Animation<RotateEulerToTargetContext> RotateEulerRelativeAnimation => (context, token) => RotateEulerToTarget(context, token);


    [AnimationMethod(ContextType = typeof(MoveContext))]
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

    [AnimationMethod(ContextType = typeof(RotateContext))]
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

    [AnimationMethod(ContextType = typeof(ScaleContext))]
    public static async UniTask Scale(ScaleContext context, CancellationToken token = default)
    {
        var motion = LMotion.Create(context.Start, context.End, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay)
            .BindToLocalScale(context.Object);

        await motion.ToUniTask(token);
    }

    [AnimationMethod(ContextType = typeof(RotateEulerContext))]
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

    [AnimationMethod(ContextType = typeof(MoveToTargetContext))]
    public static async UniTask MoveToTarget(MoveToTargetContext context, CancellationToken token = default)
    {
        if (context.Object == null) return;

        Vector3 basePosition = context.Space == AnimationSpace.Local ? context.Object.localPosition : context.Object.position;
        Vector3 endPosition = context.End;

        var motion = LMotion.Create(basePosition, endPosition, context.Duration)
            .WithEase(context.Ease)
            .WithDelay(context.Delay);

        MotionHandle handle;

        if (context.Space == AnimationSpace.Local)
            handle = motion.BindToLocalPosition(context.Object);
        else
            handle = motion.BindToPosition(context.Object);

        await handle.ToUniTask(cancellationToken: token);
    }


    [Serializable]
    public class MoveFromAxisContext : BaseAnimationContext
    {
        public Vector3 Axis;
        public float Distance;

        public MoveFromAxisContext()
        {

        }

        public MoveFromAxisContext(Transform @object, Vector3 axis, float distance, float duration, float delay, Ease ease, AnimationSpace space)
            : base(@object, duration, delay, ease, space)
        {
            Axis = axis;
            Distance = distance;
        }

        private static Vector3 CalculateEnd(Transform @object, Vector3 axis, float distance, AnimationSpace space)
        {
            Vector3 basePos = space == AnimationSpace.Local ? @object.localPosition : @object.position;

            Vector3 normalizedAxis = axis.normalized;

            Vector3 endPos = basePos + normalizedAxis * distance;

            return endPos;
        }

        public MoveToTargetContext ToMoveTargetContext()
        {
            // Вычисляем конечную позицию на основе текущей позиции + смещение по оси
            Vector3 endPos = CalculateEnd(Object, Axis, Distance, Space);

            return new MoveToTargetContext(
                Object,
                endPos,
                Duration,
                Delay,
                Ease,
                Space
            );
        }
    }

    [AnimationMethod(ContextType = typeof(RotateEulerToTargetContext))]
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
