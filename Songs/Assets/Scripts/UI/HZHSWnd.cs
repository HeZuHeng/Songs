using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HZHSWnd : UIBase
{
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HZHSWnd;
        MutexInterface = true;
    }

    public void OnClose()
    {
        UIMng.Instance.OpenUI(UIType.NONE);
    }
}
