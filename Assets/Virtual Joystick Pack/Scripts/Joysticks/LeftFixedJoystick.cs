using UnityEngine;
using UnityEngine.EventSystems;
using Assets;

public class LeftFixedJoystick : Joystick
{
    Vector2 joystickPosition = Vector2.zero;
    private Camera cam = new Camera();

    void Start()
    {
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickPosition;
        inputVector = direction / (background.sizeDelta.x / 2f);

        inputVector.x = (float)MathExtension.Constrain(inputVector.x, -1.0, 1.0);
        inputVector.y = (float)MathExtension.Constrain(inputVector.y, -1.0, 1.0);

        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        inputVector.x = 0;
        handle.anchoredPosition = new Vector2(0, handle.anchoredPosition.y);
    }
}