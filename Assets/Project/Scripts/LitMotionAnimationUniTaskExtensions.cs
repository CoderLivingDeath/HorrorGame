#region Assembly LitMotion.Animation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// location unknown
// Decompiled with ICSharpCode.Decompiler 9.1.0.7988
#endregion

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion.Animation;
using LitMotion;
using UnityEngine;

/// <summary>
/// Расширение для асинхронного запуска анимаций LitMotion с использованием UniTask
/// </summary>
public static class LitMotionAnimationUniTaskExtensions
{
    /// <summary>
    /// Запустить анимацию асинхронно с поддержкой отмены через UniTask
    /// </summary>
    public static async UniTask PlayAsync(
        this LitMotionAnimation animation,
        CancellationToken cancellationToken = default,
        bool autoPlay = true,
        PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update)
    {
        if (animation == null)
            throw new ArgumentNullException(nameof(animation));

        // Если анимация уже играет, останавливаем её
        if (animation.IsPlaying)
            animation.Stop();

        // Создаем завершаемый источник UniTask
        var completionSource = new UniTaskCompletionSource();

        // Запускаем анимацию, если нужно
        if (autoPlay)
            animation.Play();

        // Получаем токен отмены, который срабатывает при уничтожении объекта
        var destroyToken = animation.GetCancellationTokenOnDestroy();

        // Создаем связанный токен отмены
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            destroyToken
        );
        var linkedToken = linkedCts.Token;

        try
        {
            // Ожидаем завершения анимации или отмены
            await WaitForAnimationCompletion(
                animation,
                completionSource,
                linkedToken,
                playerLoopTiming
            );
        }
        finally
        {
            // Гарантируем, что задача завершена
            if (!completionSource.Task.Status.IsCompleted())
            {
                completionSource.TrySetCanceled();
            }
        }
    }

    /// <summary>
    /// Ожидание завершения анимации с проверкой состояния
    /// </summary>
    private static async UniTask WaitForAnimationCompletion(
        LitMotionAnimation animation,
        UniTaskCompletionSource completionSource,
        CancellationToken cancellationToken,
        PlayerLoopTiming playerLoopTiming)
    {
        try
        {
            // Регистрируем отмену
            cancellationToken.Register(() =>
            {
                if (animation != null && animation.gameObject != null)
                {
                    animation.Stop();
                }
                completionSource.TrySetCanceled();
            });

            // Ожидаем, пока анимация не завершится
            await UniTask.WaitUntil(
                () =>
                {
                    // Проверяем, не уничтожен ли объект
                    if (animation == null || animation.gameObject == null)
                        return true;

                    // Анимация завершена, если она не активна и не играет
                    return !animation.IsActive && !animation.IsPlaying;
                },
                playerLoopTiming,
                cancellationToken
            );

            // Успешное завершение
            completionSource.TrySetResult();
        }
        catch (OperationCanceledException)
        {
            completionSource.TrySetCanceled();
            throw;
        }
        catch (Exception ex)
        {
            completionSource.TrySetException(ex);
            throw;
        }
    }

    /// <summary>
    /// Запустить анимацию асинхронно с автоматической остановкой при уничтожении объекта
    /// </summary>
    public static UniTask PlayAsyncWithLifecycle(
        this LitMotionAnimation animation,
        bool autoPlay = true,
        PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update)
    {
        return animation.PlayAsync(
            animation.GetCancellationTokenOnDestroy(),
            autoPlay,
            playerLoopTiming
        );
    }
}

#if false // Decompilation log
'341' items in cache
------------------
Resolve: 'netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Found single assembly: 'netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.3.0f1\Editor\Data\NetStandard\ref\2.1.0\netstandard.dll'
------------------
Resolve: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.3.0f1\Editor\Data\Managed\UnityEngine\UnityEngine.CoreModule.dll'
------------------
Resolve: 'LitMotion, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'LitMotion, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\repos\HorrorGame\Library\ScriptAssemblies\LitMotion.dll'
------------------
Resolve: 'Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\repos\HorrorGame\Library\ScriptAssemblies\Unity.Collections.dll'
------------------
Resolve: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.3.0f1\Editor\Data\Managed\UnityEngine\UnityEngine.AudioModule.dll'
------------------
Resolve: 'Unity.RenderPipelines.Core.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.RenderPipelines.Core.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\repos\HorrorGame\Library\ScriptAssemblies\Unity.RenderPipelines.Core.Runtime.dll'
------------------
Resolve: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.3.0f1\Editor\Data\Managed\UnityEngine\UnityEngine.Physics2DModule.dll'
------------------
Resolve: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.3.0f1\Editor\Data\Managed\UnityEngine\UnityEngine.PhysicsModule.dll'
------------------
Resolve: 'Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\repos\HorrorGame\Library\ScriptAssemblies\Unity.TextMeshPro.dll'
------------------
Resolve: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\repos\HorrorGame\Library\ScriptAssemblies\UnityEngine.UI.dll'
------------------
Resolve: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.3.0f1\Editor\Data\Managed\UnityEngine\UnityEngine.UIModule.dll'
------------------
Resolve: 'System.Runtime.InteropServices, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'System.Runtime.InteropServices, Version=4.1.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: Version mismatch. Expected: '2.1.0.0', Got: '4.1.2.0'
Load from: 'C:\Program Files\Unity\Hub\Editor\6000.3.0f1\Editor\Data\NetStandard\compat\2.1.0\shims\netstandard\System.Runtime.InteropServices.dll'
------------------
Resolve: 'System.Runtime.CompilerServices.Unsafe, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'System.Runtime.CompilerServices.Unsafe, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: Version mismatch. Expected: '2.1.0.0', Got: '6.0.0.0'
Load from: 'D:\repos\HorrorGame\Library\PackageCache\com.unity.collections@aea9d3bd5e19\Unity.Collections.Tests\System.Runtime.CompilerServices.Unsafe\System.Runtime.CompilerServices.Unsafe.dll'
#endif
