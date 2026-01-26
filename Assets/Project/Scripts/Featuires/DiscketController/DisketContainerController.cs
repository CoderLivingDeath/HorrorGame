using Cysharp.Threading.Tasks;
using LitMotion;
using Reflex.Attributes;
using System.Threading;
using UnityEngine;

public class DisketContainerController : MonoBehaviour
{

    #region CTS

    private CancellationTokenSource InteractionCTS;

    private CancellationTokenSource CreateInteractionCTS() => CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken); 
    #endregion

    #region Flags

    private bool _isAnimationg = false;

    #endregion

    [Inject]
    private InputEventBus _InputEventBus;

    [SerializeField]
    private DisketContainerAnimations _disketContainerAnimations;

    [SerializeField]
    private Collider _collider;


    private void Start()
    {
        InteractionCTS = CreateInteractionCTS();
    }

    private bool CanInteract()
    {
        return !_isAnimationg;
    }

    public void OnInteract()
    {
        if (!CanInteract()) return;

        EnterInteraction(destroyCancellationToken).Forget();
    }

    public async UniTask EnterInteraction(CancellationToken token = default)
    {

        _InputEventBus.UI.OnCancel += _gameplayInputEventBus_OnCancel;

        if (!_disketContainerAnimations.IsHoverEnter)
        {
            var hoverEnterTask = _disketContainerAnimations.AnimateHoverEnter(token);

            _disketContainerAnimations.CanHover = false;

            await hoverEnterTask;
        }
        else
        {
            _disketContainerAnimations.CanHover = false;
        }


        _isAnimationg = true;

        var openlidTask = _disketContainerAnimations.AnimateOpenLid(token);
        var moveToPlayerTask = _disketContainerAnimations.AnimateMoveToPlayer(token).ContinueWith(() => _collider.enabled = false);

        var animations = UniTask.WhenAll(openlidTask, moveToPlayerTask);

        await UniTask.WhenAny(animations, InteractionCTS.Token.ToUniTask().Item1);

        _isAnimationg = false;

        _collider.enabled = false;

    }

    private void _gameplayInputEventBus_OnCancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ResetInteractionCTS();
        ExitInteraction().Forget();
        _InputEventBus.UI.OnCancel -= _gameplayInputEventBus_OnCancel;
    }

    public async UniTask ExitInteraction(CancellationToken token = default)
    {
        _collider.enabled = true;
        _isAnimationg = true;

        var closelidTask = _disketContainerAnimations.AnimateCloseLid(token);
        var moveToTableTask = _disketContainerAnimations.AnimateMoveToTable(token);


        await UniTask.WhenAll(closelidTask, moveToTableTask);

        _isAnimationg = false;

        _disketContainerAnimations.CanHover = true;

        _disketContainerAnimations.AnimateHoverExit().Forget();
    }

    public void ResetInteractionCTS()
    {
        InteractionCTS?.Cancel();
        InteractionCTS?.Dispose();

        InteractionCTS = CreateInteractionCTS();
    }
}


