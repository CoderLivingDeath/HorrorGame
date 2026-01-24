using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ComputerTerminalController : MonoBehaviour
{
    // Задачи в рамках:
    // - Обработка: Как обрабатывать клик по TerminalObject, вызывать события(OnMousePositionChanged, MouseEnter, MouseExit, MouseDown, MouseUp), управлять взаимодействием и курсором
    // TerminalObject - plane с текстурой и поддержкой raycasting в сцене, на котором рисуется курсор.

    public Camera camera; // Камера, с которой проецируется текстура
    public Canvas canvas; // Canvas для отображения UI на терминале по текстуре
    public GraphicRaycaster raycaster; // GraphicRaycaster для диспетчеризации событий
    public GameObject TerminalObject; // plane на который проецируется изображение терминала

    private Vector3 cachedPos;
    private Vector2 cachedUV;
    private GraphicRaycaster _raycaster;
    private InputSystemCanvasEventDispatcher _dispatcher;

    private void Awake()
    {
        _dispatcher = new InputSystemCanvasEventDispatcher(raycaster);
    }

    // Обработка событий мыши по плоскости и в сцене
    // Задачи обработаны в коде
    public void OnMousePositionChanged(MouseEventsEmitter.PointerContext context)
    {
        cachedPos = context.WorldPos;
        cachedUV = context.UV1;

        _dispatcher.DispatchDrag(context.ScreenPos);
    }

    public void OnMouseDrag(Vector2 delta)
    {

    }

    public void MouseEnter()
    {
        Vector2 screenPos = camera.WorldToScreenPoint(cachedPos);
        _dispatcher.DispatchEnter(screenPos);
    }

    public void MouseExit()
    {
        Vector2 screenPos = camera.WorldToScreenPoint(cachedPos);
        _dispatcher.DispatchExit(screenPos);
    }

    public void MouseDown()
    {
        Vector2 screenPos = camera.WorldToScreenPoint(cachedPos);
        _dispatcher.DispatchClick(screenPos);
    }   
    
    public void MouseUp()
    {

    }

    /// <summary>
    /// Преобразует UV-координаты (0-1) в экранные координаты камеры (пиксели).
    /// Предполагается, что UV соответствует viewport камеры (0,0 - нижний левый, 1,1 - верхний правый).
    /// </summary>
    /// <param name="uv">UV-координаты из текстуры.</param>
    /// <returns>Экранные координаты в пикселях.</returns>
    public Vector2 UVToScreenCoords(Vector2 uv)
    {
        if (camera == null)
        {
            Debug.LogWarning("Camera is null, returning zero screen coords.");
            return Vector2.zero;
        }

        float screenX = uv.x * camera.pixelWidth;
        float screenY = uv.y * camera.pixelHeight;
        return new Vector2(screenX, screenY);
    }

    private void OnDrawGizmos()
    {
        if (cachedUV != Vector2.zero && camera != null)
        {
            Vector2 screenCoords = UVToScreenCoords(cachedUV);
            Vector3 screenPoint = new Vector3(screenCoords.x, screenCoords.y, camera.nearClipPlane);
            Vector3 gizmoPos = camera.ScreenToWorldPoint(screenPoint);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(camera.transform.position, gizmoPos);
            Gizmos.DrawWireSphere(gizmoPos, 0.05f);
            Handles.Label(gizmoPos + Vector3.up * 0.1f, $"UV: {cachedUV.x:F2}, {cachedUV.y:F2}\nScreen: {screenCoords.x:F0}, {screenCoords.y:F0}");
        }
    }
}

/// <summary>
/// Диспетчер событий на Canvas с использованием Input System совместимых методов.
/// Использует GraphicRaycaster для рейкаста и логирование для диспетчеризации.
/// </summary>
public class InputSystemCanvasEventDispatcher
{
    private GraphicRaycaster _raycaster;

    public InputSystemCanvasEventDispatcher(GraphicRaycaster raycaster)
    {
        _raycaster = raycaster;
    }

    /// <summary>
    /// Диспетчеризирует событие клика на Canvas с использованием Input System совместимых интерфейсов.
    /// </summary>
    /// <param name="screenPos">Экранная позиция события.</param>
    public void DispatchClick(Vector2 screenPos)
    {
        DispatchEvent(screenPos, (target, eventData) => ExecuteEvents.Execute<IPointerClickHandler>(target, eventData, (handler, data) => handler.OnPointerClick((PointerEventData)data)));
    }

    /// <summary>
    /// Диспетчеризирует событие входа на Canvas.
    /// </summary>
    /// <param name="screenPos">Экранная позиция события.</param>
    public void DispatchEnter(Vector2 screenPos)
    {
        DispatchEvent(screenPos, (target, eventData) => ExecuteEvents.Execute<IPointerEnterHandler>(target, eventData, (handler, data) => handler.OnPointerEnter((PointerEventData)data)));
    }

    /// <summary>
    /// Диспетчеризирует событие выхода с Canvas.
    /// </summary>
    /// <param name="screenPos">Экранная позиция события.</param>
    public void DispatchExit(Vector2 screenPos)
    {
        DispatchEvent(screenPos, (target, eventData) => ExecuteEvents.Execute<IPointerExitHandler>(target, eventData, (handler, data) => handler.OnPointerExit((PointerEventData)data)));
    }

    /// <summary>
    /// Диспетчеризирует событие перетаскивания на Canvas.
    /// </summary>
    /// <param name="screenPos">Экранная позиция события.</param>
    public void DispatchDrag(Vector2 screenPos)
    {
        DispatchEvent(screenPos, (target, eventData) => ExecuteEvents.Execute<IDragHandler>(target, eventData, (handler, data) => handler.OnDrag((PointerEventData)data)));
    }

    private void DispatchEvent(Vector2 screenPos, System.Action<GameObject, PointerEventData> executeAction)
    {
        if (_raycaster == null || EventSystem.current == null) return;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPos,
            pressPosition = screenPos,
            button = PointerEventData.InputButton.Left
        };

        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(eventData, results);

        if (results.Count > 0)
        {
            eventData.pointerPress = results[0].gameObject;
            executeAction(results[0].gameObject, eventData);
            Debug.Log($"Dispatched event to {results[0].gameObject.name} at {screenPos}");
        }
    }
}