using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractionEventEmiter : MonoBehaviour, IInteractHandler
{
    public event Action<InteractContext> OnInteract;
    
    [InspectorName("OnInteract")]
    public UnityEvent<InteractContext> OnInteractUnityEvent;

    public void HandleInteract(in InteractContext context)
    {
        OnInteract?.Invoke(context);
        OnInteractUnityEvent?.Invoke(context);
    }
}