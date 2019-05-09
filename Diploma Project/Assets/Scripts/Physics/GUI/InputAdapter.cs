using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAdapter : MonoBehaviour
{
    
    public static bool IsMobileDevice
    {
        get
        {
            bool result = false;

            #if UNITY_ANDROID || UNITY_IOS
                result = true;
            #endif

            return result;
        }
    }
    public static InputAdapter Instance
    {
        get;
        private set;
    }

    void Awake()
    {
        Instance = this;
    }


    public float HorizontalInput()
    {
        float result = Input.GetAxis ("Horizontal");
        if (Mathf.Approximately(result, 0f))
        {
            if (SimpleGui.Instance != null)
            {
                if (SimpleGui.Instance.IsPressed(GuiButtonTypeTEMP.Right))
                {
                    result = 1f;
                }
                else if (SimpleGui.Instance.IsPressed(GuiButtonTypeTEMP.Left))
                {
                    result = -1f;
                }
            }
        }
        return result;
    }
    public float VerticalInput()
    {
        float result = Input.GetAxis ("Vertical");
        if (Mathf.Approximately(result, 0f))
        {
            if (SimpleGui.Instance != null)
            {
                if (SimpleGui.Instance.IsPressed(GuiButtonTypeTEMP.Up))
                {
                    result = 1f;
                }
                else if (SimpleGui.Instance.IsPressed(GuiButtonTypeTEMP.Down))
                {
                    result = -1f;
                }
            }
        }
        return result;
    }

}
