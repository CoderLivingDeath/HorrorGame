using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using Reflex.Attributes;
using ScriptableAnimation;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        Rotatation, ComputerInteraction
    }

    [SerializeField]
    private PlayerAnimations PlayerAnimations;

    [Inject] private InputEventBus _inputEventBus;

    public CinemachineCamera cinemachineCamera;

    public CameraTarget CameraTarget;

    public State CurrentState = State.Rotatation;


    #region Mouse tracking

    private MouseEventsEmitter currentHoveredEmitter;
    private bool isMousePressed = false;
    private Vector2 lastMousePosition;

    private void OnMousePositionChanged(InputAction.CallbackContext callbackContext)
    {
        Vector2 mousePosition = callbackContext.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<MouseEventsEmitter>(out var emitter))
            {
                // Если сменился объект
                if (emitter != currentHoveredEmitter)
                {
                    // Выход с предыдущего
                    ExitPreviousEmitter();
                    // Вход на новый
                    emitter.EmittOnMouseEnter();  // ← Явно вызываем Enter
                    currentHoveredEmitter = emitter;
                }
                // Всегда отправляем позицию текущему объекту
                var context = new MouseEventsEmitter.PointerContext(mousePosition, hit.point, hit);
                currentHoveredEmitter.EmittMousePosition(context); // ← Контекст
            }
            else
            {
                // Объект без компонента - выход
                ExitPreviousEmitter();
            }
        }
        else
        {
            // Мышь вне объектов - выход
            ExitPreviousEmitter();
        }

        if (isMousePressed && currentHoveredEmitter != null)
        {
            Vector2 delta = mousePosition - lastMousePosition;
            currentHoveredEmitter.EmittOnMouseDrag(delta);
        }
        lastMousePosition = mousePosition;
    }

    private void ExitPreviousEmitter()
    {
        if (currentHoveredEmitter != null)
        {
            currentHoveredEmitter.EmittOnMouseExit();
            currentHoveredEmitter = null;
        }
    }

    private void OnMouseL(InputAction.CallbackContext callbackContext)
    {
        var value = callbackContext.ReadValueAsButton();
        if (value)
        {
            if (currentHoveredEmitter != null)
            {
                currentHoveredEmitter.EmittOnMouseDown();
            }
            isMousePressed = true;
        }
        else
        {
            if (currentHoveredEmitter != null)
            {
                currentHoveredEmitter.EmittOnMouseUp();
            }
            isMousePressed = false;
        }
    }
    #endregion

    void OnEnable()
    {
        _inputEventBus.Gameplay.OnLeft += OnLeft;
        _inputEventBus.Gameplay.OnRight += OnRight;
        _inputEventBus.Gameplay.MousePosition += OnMousePositionChanged;
        _inputEventBus.Gameplay.MouseL += OnMouseL;
    }

    void OnDisable()
    {
        _inputEventBus.Gameplay.OnLeft -= OnLeft;
        _inputEventBus.Gameplay.OnRight -= OnRight;
        _inputEventBus.Gameplay.MousePosition -= OnMousePositionChanged;
        _inputEventBus.Gameplay.MouseL -= OnMouseL;
    }

    public void SetCameraTarget(CameraTarget cameraTarget)
    {
        cinemachineCamera.Target = cameraTarget;
    }

    public void EnterRotationMode()
    {
        if (CurrentState == State.Rotatation) return;

        _inputEventBus.Gameplay.OnLeft += OnLeft;
        _inputEventBus.Gameplay.OnRight += OnRight;

        CurrentState = State.Rotatation;
    }

    public void EnterComputerInteractionMode()
    {
        if (CurrentState == State.ComputerInteraction) return;

        _inputEventBus.Gameplay.OnLeft -= OnLeft;
        _inputEventBus.Gameplay.OnRight -= OnRight;

        CurrentState = State.ComputerInteraction;
    }
    private void OnLeft(InputAction.CallbackContext callbackContext)
    {
        PlayerAnimations.PlayAnimationRotationLeft();
    }

    private void OnRight(InputAction.CallbackContext callbackContext)
    {
        PlayerAnimations.PlayAnimationRotationRight();
    }
}
