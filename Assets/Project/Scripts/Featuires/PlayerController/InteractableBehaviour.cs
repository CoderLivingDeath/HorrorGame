using UnityEngine;

public class InteractableBehaviour : MonoBehaviour
{
    private IInteractHandler[] interactHandlers = null;

    private IInteractHandler[] GetInteractHandlers()
    {
        var components = GetComponents<IInteractHandler>();

        return components;
    }

    void Start()
    {
        interactHandlers = GetInteractHandlers();
    }

    public void Interact(GameObject sender)
    {
        InteractContext context = new(sender);
        foreach(var handler in interactHandlers)
        {
            handler.HandleInteract(in context);
        }
    }
}
