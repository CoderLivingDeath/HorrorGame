using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayInputEventBus : IDisposable
{
    public event Action<InputAction.CallbackContext> OnLeft;
    public event Action<InputAction.CallbackContext> OnRight;
    public event Action<InputAction.CallbackContext> OnInteract;
    public event Action<InputAction.CallbackContext> MousePosition;
    public event Action<InputAction.CallbackContext> MouseL;

    private void Rise<T>(Action<T> @event, T value)
    {
        ThrowIfDisposed();
        @event?.Invoke(value);
    }

    public void RiseLeft(InputAction.CallbackContext callback) => Rise(OnLeft, callback);
    public void RiseRight(InputAction.CallbackContext callback) => Rise(OnRight, callback);
    public void RiseInteract(InputAction.CallbackContext callback) => Rise(OnInteract, callback);
    public void RiseMosePosition(InputAction.CallbackContext callback) => Rise(MousePosition, callback);
    public void RiseMoseL(InputAction.CallbackContext callback) => Rise(MouseL, callback);

    private bool disposed;
    public bool Disposed => disposed;

    private void ThrowIfDisposed()
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(GameplayInputEventBus));
    }

    public void Dispose()
    {
        OnLeft = null;
        OnRight = null;
        OnInteract = null;
        MousePosition = null;
        disposed = true;
    }
}
