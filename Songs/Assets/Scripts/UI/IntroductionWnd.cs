using DG.Tweening;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroductionWnd : UIBase
{
    public Button next;
    public TrendsText trendsText;
    public RectTransform[] items;
    public RectTransform image;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.IntroductionWnd;
        MutexInterface = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        next.gameObject.SetActive(true);
        Vector3[] vector3s = new Vector3[3];

        for (int i = 0; i < items.Length; i++)
        {
            vector3s[i] = items[i].localPosition;
        }
        Tweener moveTw = image.DOLocalPath(vector3s,10,PathType.Linear,PathMode.Sidescroller2D).SetLookAt(0.0001f);
        moveTw.SetLoops(-1, LoopType.Yoyo);
    }

    public void OnClickHZHS()
    {
        SongsDataMng.GetInstance().SetTaskConfigData(1);
        UIMng.Instance.OpenUI(UIType.NONE);
        UIMng.Instance.ActivationUI(UIType.SelectPlotWnd);
    }

    public void OnClickHTM()
    {
        SongsDataMng.GetInstance().SetTaskConfigData(0);
        UIMng.Instance.OpenUI(UIType.NONE);
        UIMng.Instance.ActivationUI(UIType.SelectPlotWnd);
    }

    public void OnEnd()
    {
        trendsText.m_ShowSpeed = 10000;
    }
}
