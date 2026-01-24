using UnityEngine.EventSystems;

public interface ITerminalPointerHandler : IEventSystemHandler
{
    void OnTerminalClick(PointerEventData eventData);
    void OnTerminalEnter(PointerEventData eventData);
    void OnTerminalExit(PointerEventData eventData);
    void OnTerminalDrag(PointerEventData eventData);
}