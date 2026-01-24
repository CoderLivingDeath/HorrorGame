using UnityEngine;

public readonly struct InteractContext
{
    public readonly GameObject Sender;

    public InteractContext(GameObject sender)
    {
        Sender = sender;
    }
}
