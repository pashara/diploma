using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UiButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action<UiButton, PointerEventData> OnDown;
    public event Action<UiButton, PointerEventData> OnUp;


    public void OnPointerDown(PointerEventData eventData)
    {
        OnDown?.Invoke(this, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnUp?.Invoke(this, eventData);
    }



}
