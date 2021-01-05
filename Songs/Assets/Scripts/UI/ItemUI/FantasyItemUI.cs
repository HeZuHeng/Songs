using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FantasyItemUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    public System.Action<Image> OnPointUp;

    public Image image;
    public Text tip;
    public Image itemTip;

    public void OnDrag(PointerEventData eventData)
    {
        itemTip.transform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //image.gameObject.SetActive(false);
        itemTip.transform.position = image.transform.position;
        itemTip.transform.rotation = image.transform.rotation;
        itemTip.transform.localScale = Vector3.one;
        itemTip.sprite = image.sprite;
        itemTip.gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tip.gameObject.SetActive( true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tip.gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        itemTip.gameObject.SetActive(false);
        if (OnPointUp != null)
        {
            OnPointUp(image);
        }
    }

}
