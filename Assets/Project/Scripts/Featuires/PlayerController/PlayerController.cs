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

    private int _currentAngle = 0;

    [SerializeField]
    private float[] _rotationAngles =
    {
        0f, 90f, 180f, 270f
    };

    [Inject] private InputEventBus _inputEventBus;
    private CancellationTokenSource _rotationCts = new();
    private Animation<AnimationLibrary.MoveContext> _rotateAnimation = AnimationLibrary.Move;

    public bool IsAnimating = false;

    public CinemachineCamera cinemachineCamera;

    public CameraTarget CameraTarget;

    public Ease RotationEase = Ease.Linear;
    public float RotationDuration = 1f;

    public State CurrentState = State.Rotatation;

    void OnEnable()
    {
        _inputEventBus.Gameplay.OnLeft += OnLeft;
        _inputEventBus.Gameplay.OnRight += OnRight;
        //_inputEventBus.Gameplay.OnInteract += OnInteract;
        _inputEventBus.Gameplay.MousePosition += OnMousePositionChanged;
        _inputEventBus.Gameplay.MouseL += OnMouseL;

        _rotationCts = new();
    }

    void OnDisable()
    {
        _inputEventBus.Gameplay.OnLeft -= OnLeft;
        _inputEventBus.Gameplay.OnRight -= OnRight;
        //_inputEventBus.Gameplay.OnInteract -= OnInteract;
        _inputEventBus.Gameplay.MousePosition -= OnMousePositionChanged;

        _rotationCts?.Cancel();
        _rotationCts?.Dispose();
    }

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

    #region Interaction

    //private void OnInteract(InputAction.CallbackContext callbackContext)
    //{
    //    // Raycast по позиции мыши
    //    Vector2 mousePosition = Mouse.current.position.ReadValue();

    //    Ray ray = Camera.main.ScreenPointToRay(mousePosition);

    //    if (Physics.Raycast(ray, out RaycastHit hit))
    //    {
    //        Debug.Log($"Клик по: {hit.collider.name}");
    //        if (hit.collider.TryGetComponent<InteractableBehaviour>(out var interactable))
    //        {
    //            interactable.Interact(this.gameObject);
    //        }
    //    }
    //}

    #endregion

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
        int newIndex = (_currentAngle - 1 + _rotationAngles.Length) % _rotationAngles.Length;
        float targetAngle = _rotationAngles[newIndex];

        _currentAngle = newIndex;
        RotateAsync(targetAngle).Forget();
    }

    private void OnRight(InputAction.CallbackContext callbackContext)
    {
        int newIndex = (_currentAngle + 1) % _rotationAngles.Length;
        float targetAngle = _rotationAngles[newIndex];

        _currentAngle = newIndex;
        RotateAsync(targetAngle).Forget();
    }

    public async UniTaskVoid RotateAsync(float targetAngleDegrees)
    {
        _rotationCts?.Cancel();
        _rotationCts?.Dispose();
        _rotationCts = new CancellationTokenSource();

        // Целевая ротация из массива углов
        float normalizedTargetAngle = targetAngleDegrees % 360f;

        var startRotation = CameraTarget.TrackingTarget.rotation;
        var targetRotation = Quaternion.Euler(CameraTarget.TrackingTarget.eulerAngles.x, normalizedTargetAngle, CameraTarget.TrackingTarget.eulerAngles.z);

        // Создаем контекст для вращения
        var context = new AnimationLibrary.RotateContext(
            obj: CameraTarget.TrackingTarget,
            start: startRotation,
            end: targetRotation,
            duration: RotationDuration,
            delay: 0f,
            ease: RotationEase
        );

        await AnimationLibrary.Rotate(context, _rotationCts.Token);
    }
}
