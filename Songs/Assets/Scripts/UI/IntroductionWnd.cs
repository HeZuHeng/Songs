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
        UIMng.Instance.OpenUI(UIType.HZHSWnd);
    }

    public void OnClickHTM()
    {
        UIMng.Instance.OpenUI(UIType.HTMWnd);
    }
}
