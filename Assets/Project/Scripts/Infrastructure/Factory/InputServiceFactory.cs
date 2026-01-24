using Reflex.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputServiceFactory : IFactory<InputService>
{
    private const string GAMEPLAY_MAP = "Gameplay";

    private const string GAMEPLAY_LEFT_ACTION = "Left";
    private const string GAMEPLAY_RIGHT_ACTION = "Right";
    private const string GAMEPLAY_INTERACT_ACTION = "Interact";
    private const string GAMEPLAY_MOUSE_POSITION_ACTION = "MousePosition";
    private const string GAMEPLAY_MOUSE_L_ACTION = "MouseL";
    private readonly Container container;

    [Obsolete]
    public InputServiceFactory(Container container)
    {
        this.container = container;
    }

    public InputServiceFactory()
    {
    }

    [Obsolete]
    public InputService Create()
    {
        if (container == null) throw new NullReferenceException(nameof(container));

        InputActionAsset inputAsset = container.Resolve<InputSystem_Actions>().asset;
        InputEventBus inputEventBus = container.Resolve<InputEventBus>();

        InputService service = new(inputAsset);

        SubscribeInputs(service, inputEventBus);

        return service;
    }

    public InputService Create(InputActionAsset inputAsset, InputEventBus inputEventBus)
    {
        InputService service = new(inputAsset);

        SubscribeInputs(service, inputEventBus);

        return service;
    }

    private void SubscribeInputs(InputService service, InputEventBus inputEventBus)
    {
        service.Subscribe(new(GAMEPLAY_MAP, GAMEPLAY_LEFT_ACTION), inputEventBus.Gameplay.RiseLeft, InputActionType.Performed);

        service.Subscribe(new(GAMEPLAY_MAP, GAMEPLAY_RIGHT_ACTION), inputEventBus.Gameplay.RiseRight, InputActionType.Performed);

        service.Subscribe(new(GAMEPLAY_MAP, GAMEPLAY_MOUSE_POSITION_ACTION), inputEventBus.Gameplay.RiseMosePosition, InputActionType.Performed);

        service.Subscribe(new(GAMEPLAY_MAP, GAMEPLAY_INTERACT_ACTION), inputEventBus.Gameplay.RiseInteract, InputActionType.Performed);

        service.Subscribe(new(GAMEPLAY_MAP, GAMEPLAY_MOUSE_L_ACTION), inputEventBus.Gameplay.RiseMoseL, InputActionType.Performed | InputActionType.Canceled);
    }
}
