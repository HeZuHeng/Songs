using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionWnd : UIBase
{
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.IntroductionWnd;
        MutexInterface = true;
    }


    public void OnClickHZHS()
    {
        SongsDataMng.GetInstance().SetTaskConfigData(1);
        UIMng.Instance.OpenUI(UIType.SelectPlotWnd);
    }

    public void OnClickHTM()
    {
        SongsDataMng.GetInstance().SetTaskConfigData(0);
        UIMng.Instance.OpenUI(UIType.SelectPlotWnd);
    }
}
