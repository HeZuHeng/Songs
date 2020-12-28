using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HZHSEndWnd : UIBase
{

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HZHSEndWnd;
        MutexInterface = true;

    }

    public void OnEnd()
    {
        SongsDataMng.GetInstance().SetTaskConfigData(0);
        UIMng.Instance.ActivationUI(UIType.SelectPlotWnd);
    }
}
