using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTMWnd : UIBase
{
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HTMWnd;
        MutexInterface = true;
    }
}