using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum GuiButtonTypeTEMP
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4,
    Reset = 5,
}
public class SimpleGui : MonoBehaviour
{
    public static event Action<GuiButtonTypeTEMP> OnUp;
    public static event Action<GuiButtonTypeTEMP> OnDown;
    [SerializeField] UiButton upButton;
    [SerializeField] UiButton downButton;
    [SerializeField] UiButton leftButton;
    [SerializeField] UiButton rightButton;
    [SerializeField] UiButton resetButton;
    Dictionary<GuiButtonTypeTEMP, bool> isPressed = new Dictionary<GuiButtonTypeTEMP, bool>();


    public static SimpleGui Instance
    {
        get;
        private set;
    }
  
    void OnEnable()
    {
        Instance = this;
        upButton.OnUp += Button_OnUp;
        downButton.OnUp += Button_OnUp;
        leftButton.OnUp += Button_OnUp;
        rightButton.OnUp += Button_OnUp;
        resetButton.OnUp += Button_OnUp;

        upButton.OnDown += Button_OnDown;
        downButton.OnDown += Button_OnDown;
        leftButton.OnDown += Button_OnDown;
        rightButton.OnDown += Button_OnDown;
        resetButton.OnDown += Button_OnDown;

    }


    void Button_OnDown(UiButton button, PointerEventData data)
    {
        GuiButtonTypeTEMP key = GuiButtonTypeTEMP.None;
        if (button == upButton)
        {
            key = GuiButtonTypeTEMP.Up;
        }
        else if (button == downButton)
        {
            key = (GuiButtonTypeTEMP.Down);   
        }
        else if (button == leftButton)
        {
            key = (GuiButtonTypeTEMP.Left);   
        }
        else if (button == rightButton)
        {
            key = (GuiButtonTypeTEMP.Right);   
        }
        else if (button == resetButton)
        {
            key = (GuiButtonTypeTEMP.Reset);
        }

        if (key != GuiButtonTypeTEMP.None)
        {
            isPressed.Remove(key);
            isPressed.Add(key, true);
        }
        OnDown?.Invoke(key);
    }
    void Button_OnUp(UiButton button, PointerEventData data)
    {
        GuiButtonTypeTEMP key = GuiButtonTypeTEMP.None;
        if (button == upButton)
        {
            key = GuiButtonTypeTEMP.Up;
        }
        else if (button == downButton)
        {
            key = (GuiButtonTypeTEMP.Down);   
        }
        else if (button == leftButton)
        {
            key = (GuiButtonTypeTEMP.Left);   
        }
        else if (button == rightButton)
        {
            key = (GuiButtonTypeTEMP.Right);   
        }
        else if (button == resetButton)
        {
            key = (GuiButtonTypeTEMP.Reset);
        }

        if (key != GuiButtonTypeTEMP.None)
        {
            isPressed.Remove(key);
            isPressed.Add(key, false);
        }

        OnUp?.Invoke(key);
    }


    public bool IsPressed(GuiButtonTypeTEMP key)
    {
        bool result = false;
        bool isFinded = isPressed.TryGetValue(key, out result);

        return isFinded && result;
    }
}
