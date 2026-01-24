using System;

public class InputEventBus : IDisposable
{
    public readonly UIInputEventBus UI = new();
    public readonly GameplayInputEventBus Gameplay = new();

    public InputEventBus()
    {

    }

    public void Dispose()
    {
        UI.Dispose();
        Gameplay.Dispose();
    }
}
