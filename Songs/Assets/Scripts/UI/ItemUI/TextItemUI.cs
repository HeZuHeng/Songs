using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextItemUI : MonoBehaviour,IPointerEnterHandler
{
    Text text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }
    
    void Awake()
    {
        if (text == null) text = GetComponent<Text>();
    }
}
