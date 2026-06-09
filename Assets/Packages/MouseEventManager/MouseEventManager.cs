using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MouseEventManager : MonoBehaviour
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle,
        Back,
        Forward,
    }

    public enum MouseInputType
    {
        WasPressedThisFrame,
        WasReleasedThisFrame,
        IsPressed,
    }

    [System.Serializable]
    public class MouseEvent
    {
        public MouseButton       button;
        public MouseInputType    inputType;
        public UnityEvent<Mouse> handler;
    }

    [field: SerializeField] public List<MouseEvent> MouseEvents { get; private set; }

    private void Update()
    {
        var mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        foreach (var mouseEvent in from mouseEvent in MouseEvents
                 let button = mouseEvent.button switch
                 {
                     MouseButton.Left    => mouse.leftButton,
                     MouseButton.Right   => mouse.rightButton,
                     MouseButton.Middle  => mouse.middleButton,
                     MouseButton.Back    => mouse.backButton,
                     MouseButton.Forward => mouse.forwardButton,
                     _                   => null
                 }
                 let satisfied = button != null && mouseEvent.inputType switch
                 {
                     MouseInputType.WasPressedThisFrame  => button.wasPressedThisFrame,
                     MouseInputType.WasReleasedThisFrame => button.wasReleasedThisFrame,
                     MouseInputType.IsPressed            => button.isPressed,
                     _                                   => false
                 }
                 where satisfied select mouseEvent)
        {
            mouseEvent.handler.Invoke(mouse);
        }
    }

    public bool TryGetValue(MouseButton button, out MouseEvent mouseEvent)
    {
        mouseEvent = null;

        foreach (var e in MouseEvents.Where(e => e.button == button))
        {
            mouseEvent = e;
            return true;
        }

        return false;
    }
}