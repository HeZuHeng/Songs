using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntroductionItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public int TaskConfigId = -1;
    public Transform tip;
    public RectTransform item;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tip.gameObject.SetActive(true);
        item.sizeDelta = new Vector2(140,140);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tip.gameObject.SetActive(false);
        item.sizeDelta = new Vector2(100, 100);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (TaskConfigId < 0) return;
        SongsDataMng.GetInstance().SetTaskConfigData(TaskConfigId);
        UIMng.Instance.OpenUI(UIType.NONE);
        UIMng.Instance.ActivationUI(UIType.SelectPlotWnd);
    }
}
