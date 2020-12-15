using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class SelectPlotWnd : UIBase
{
    public UI_Control_ScrollFlow scrollFlow;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.SelectPlotWnd;
    }

    protected override void Start()
    {
        base.Start();
        scrollFlow.Refresh();
    }
}
