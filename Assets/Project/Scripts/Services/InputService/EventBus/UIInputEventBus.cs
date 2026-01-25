using System;
using UnityEngine.InputSystem;

public class UIInputEventBus : IDisposable
{
    public event Action<InputAction.CallbackContext> OnSubmit;
    public event Action<InputAction.CallbackContext> OnCancel;

    private void Rise<T>(Action<T> @event, T value)
    {
        ThrowIfDisposed();
        @event?.Invoke(value);
    }

    public void RiseSubmit(InputAction.CallbackContext context) => Rise(OnSubmit, context);
    public void RiseCancel(InputAction.CallbackContext context) => Rise(OnCancel, context);

    private bool disposed;
    public bool Disposed => disposed;

    private void ThrowIfDisposed()
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(UIInputEventBus));
    }

    public void Dispose()
    {
        OnSubmit = null;
        disposed = true;
    }
}
