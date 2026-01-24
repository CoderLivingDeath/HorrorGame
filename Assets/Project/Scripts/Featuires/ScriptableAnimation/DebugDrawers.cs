#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static AnimationLibrary;
using System;
using System.Collections;

public static class AnimationDebugDrawers
{
    public static void DrawRotateToTarget(in RotateEulerToTargetContext context)
    {
        if (context.Object == null) return;

        float angle = context.EndEuler.magnitude;
        Vector3 axis = GetRotationAxis(context.EndEuler);

        DrawArcWithHandles(context.Object.position, context.Object.forward, axis, context.EndEuler, angle);
    }

    private static void DrawArcWithHandles(Vector3 center, Vector3 forward, Vector3 axis, Vector3 endEuler, float angle, float radius = 1f)
    {
        Handles.color = new Color(0f, 1f, 0f, 0.1f);

        if (angle >= 360f)
            DrawFullCircles(center, forward, axis, angle, radius);
        else
            Handles.DrawSolidArc(center, axis, forward, angle, radius);

        DrawAxisVisualization(center, axis, radius);
        DrawAngleLabel(center, forward, endEuler, angle, radius);
    }

    private static void DrawFullCircles(Vector3 center, Vector3 forward, Vector3 axis, float angle, float radius)
    {
        int fullCircles = Mathf.FloorToInt(angle / 360f);
        Vector3 currentForward = forward;

        for (int i = 0; i < fullCircles; i++)
        {
            Vector3 offset = axis * (i * 0.01f);
            Handles.DrawSolidArc(center + offset, axis, currentForward, 360f, radius);
        }

        float remainingAngle = angle - (fullCircles * 360f);
        if (remainingAngle > 0.1f)
        {
            Handles.DrawSolidArc(center, axis, forward, remainingAngle, radius);
        }
    }

    private static void DrawAxisVisualization(Vector3 center, Vector3 axis, float radius)
    {
        Handles.DrawLine(center, center + axis);
    }

    private static void DrawAngleLabel(Vector3 center, Vector3 forward, Vector3 endEuler, float angle, float radius)
    {
        Quaternion halfRotation = Quaternion.Euler(endEuler.x * 0.5f, endEuler.y * 0.5f, endEuler.z * 0.5f);
        Vector3 labelPosition = center + (halfRotation * forward * radius);
        Handles.Label(labelPosition, $"{angle:F1}°");
    }

    private static Vector3 GetRotationAxis(Vector3 euler)
    {
        float absX = Mathf.Abs(euler.x);
        float absY = Mathf.Abs(euler.y);
        float absZ = Mathf.Abs(euler.z);

        if (absY >= absX && absY >= absZ) return Vector3.up;
        if (absZ >= absX && absZ >= absY) return Vector3.forward;
        return Vector3.right;
    }
}
#endif
