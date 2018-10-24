using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider2DRight : MonoBehaviour {
    public GUISkin SliderSkin;
    public GUIStyle BackStyle;
    public GUIStyle ThumbStyle;


    public static Vector2 value;
    public Vector2 maxvalue;
    public Vector2 minvalue;

    static Vector2 offset;
    static int slider2Dhash = "Slider2D".GetHashCode();



    // Use this for initialization
    void Start()
    {
        value    = new Vector2(  0.0f,   0.0f);
        maxvalue = new Vector2( 90.0f,  90.0f);
        minvalue = new Vector2(-90.0f, -90.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // OnGUI
    void OnGUI()
    {
        GUI.skin = SliderSkin;

        value = SliderTWOD(new Rect(940, 310, 200, 200),
                  value,
                  maxvalue,
                  minvalue,
                  "box",
                  GUI.skin.horizontalSliderThumb);

    }

    static void swap(ref float f1, ref float f2) { float tmp = f1; f1 = f2; f2 = tmp; }

    static Vector2 SliderTWOD(Rect rect, Vector2 value, Vector2 maxvalue, Vector2 minvalue, GUIStyle backStyle, GUIStyle thumbStyle)
    {
        if (backStyle == null)
            return value;
        if (thumbStyle == null)
            return value;

        int id = GUIUtility.GetControlID(slider2Dhash, FocusType.Passive);

        // test max and min
        if (maxvalue.x < minvalue.x) // swap
            swap(ref maxvalue.x, ref minvalue.x);
        if (maxvalue.y < minvalue.y)
            swap(ref maxvalue.y, ref minvalue.y);



        // value to px ratio vector
        Vector2 val2pxRatio = new Vector2(
            (rect.width - (backStyle.padding.right + backStyle.padding.left)) / (maxvalue.x - minvalue.x),
            (rect.height - (backStyle.padding.top + backStyle.padding.bottom)) / (maxvalue.y - minvalue.y));

        // thumb
        float thumbHeight = thumbStyle.fixedHeight == 0 ? thumbStyle.padding.vertical : thumbStyle.fixedHeight;
        float thumbWidth = thumbStyle.fixedWidth == 0 ? thumbStyle.padding.horizontal : thumbStyle.fixedWidth;

        Rect thumbRect = new Rect(
                    rect.x + (value.x * val2pxRatio.x) - (thumbWidth / 2) + backStyle.padding.left - (minvalue.x * val2pxRatio.x),
                    rect.y + (value.y * val2pxRatio.y) - (thumbHeight / 2) + backStyle.padding.top - (minvalue.y * val2pxRatio.y),
                    thumbWidth, thumbHeight);

        Event e = Event.current;

        switch (e.GetTypeForControl(id))
        {
            case EventType.MouseDown:
                {
                    if (rect.Contains(e.mousePosition)) // inside this control
                    {

                        GUIUtility.hotControl = id;
                        // did we hit the thumb?
                        if (thumbRect.Contains(e.mousePosition))
                        {
                            // record offset
                            offset = new Vector2(
                                                e.mousePosition.x - thumbRect.x,
                                                e.mousePosition.y - thumbRect.y
                                              );

                        }
                        else // or just outside
                        {
                            offset = new Vector2(7, 7);
                            value.x = (((e.mousePosition.x - rect.x) - offset.x + (thumbWidth / 2) - backStyle.padding.left) / val2pxRatio.x) + (minvalue.x);
                            value.y = (((e.mousePosition.y - rect.y) - offset.y + (thumbHeight / 2) - backStyle.padding.top) / val2pxRatio.y) + (minvalue.y);

                        }
                        Event.current.Use();
                    }
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == id)
                    GUIUtility.hotControl = 0;
                break;
            case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != id)
                        break;
                    // move thumb to mouse position
                    value.x = (((e.mousePosition.x - rect.x + (thumbWidth / 2)) - offset.x - backStyle.padding.left) / val2pxRatio.x) + (minvalue.x);
                    value.y = (((e.mousePosition.y - rect.y + (thumbHeight / 2)) - offset.y - backStyle.padding.top) / val2pxRatio.y) + (minvalue.y);

                    // clamp
                    value.x = Mathf.Clamp(value.x, minvalue.x, maxvalue.x);
                    value.y = Mathf.Clamp(value.y, minvalue.y, maxvalue.y);
                }
                break;
            case EventType.Repaint:
                {
                    // background              
                    backStyle.Draw(rect, GUIContent.none, id);
                    // thumb
                    thumbStyle.Draw(thumbRect, GUIContent.none, id);
                }
                break;
        }
        return value;
    }
}
