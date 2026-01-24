using System;
using UnityEngine;
using UnityEngine.Events;

public class MouseEventsEmitter : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onMouseDown = new();
    public UnityEvent onMouseUp = new();
    public UnityEvent<Vector2> onMouseDrag = new();
    public UnityEvent<Vector2> onMousePosition = new();
    public UnityEvent<Vector3> onMousePosition3D = new();
    public UnityEvent<PointerContext> onMousePositionContext = new();
    public UnityEvent onMouseEnter = new();
    public UnityEvent onMouseExit = new();

    public void EmittOnMouseDown()
    {
        if (this.enabled)
            onMouseDown?.Invoke();
    }

    public void EmittOnMouseUp()
    {
        if (this.enabled)
            onMouseUp?.Invoke();
    }

    public void EmittOnMouseDrag(Vector2 delta)
    {
        if (this.enabled)
            onMouseDrag?.Invoke(delta);
    }

    public void EmittOnMouseEnter()
    {
        if (this.enabled)
            onMouseEnter?.Invoke();
    }

    public void EmittOnMouseExit()
    {
        if (this.enabled)
            onMouseExit?.Invoke();
    }

    public void EmittMousePosition(PointerContext context)
    {
        if (!this.enabled) return;

        onMousePosition?.Invoke(context.ScreenPos);
        onMousePosition3D?.Invoke(context.WorldPos);
        onMousePositionContext?.Invoke(context);
    }

    public void EmittMousePosition3D(Vector3 position)
    {
        if (this.enabled)
            onMousePosition3D?.Invoke(position);
    }

    public readonly struct PointerContext
    {
        public readonly Vector2 ScreenPos;
        public readonly Vector3 WorldPos;
        public readonly RaycastHit Hit;

        public PointerContext(Vector2 screenPos, Vector3 worldPos, RaycastHit hit)
        {
            ScreenPos = screenPos;
            WorldPos = worldPos;
            Hit = hit;
        }

        public Vector2 UV1 => Hit.textureCoord;
        public Vector2 UV2 => Hit.textureCoord2;
    }
}