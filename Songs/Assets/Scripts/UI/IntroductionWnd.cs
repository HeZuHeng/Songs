using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroductionWnd : UIBase
{
    public Button next;
    public TrendsText trendsText;
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
