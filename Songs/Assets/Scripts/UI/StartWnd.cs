using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Songs;

public class StartWnd :  UIBase
{
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.StartWnd;
        MutexInterface = true;
    }

    public void OnBtnClick()
    {
        UIMng.Instance.OpenUI(UIType.IntroductionWnd);
    }
}
