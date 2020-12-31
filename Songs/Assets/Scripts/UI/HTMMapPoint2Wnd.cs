using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HTMMapPoint2Wnd : UIBase
{
    public Button Button_ok;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HTMMapPoint2Wnd;
        MutexInterface = true;
        Initialized();
    }

    private void Initialized()
    {
        if(Button_ok != null) Button_ok.onClick.AddListener(OnClick);
    }



    protected override void OnEnable()
    {
        base.OnEnable();
    }






    public void OnClick()
    {
        //to do:切换界面
        Debug.Log("结束地图");
        OnEnd();
    }

    public void OnEnd()
    {
        UIMng.Instance.OpenUI(UIType.NONE);

        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        if (taskData != null)
        {
            if (taskData.type == TaskType.OpenWnd)
            {
                UIType type = (UIType)System.Enum.Parse(typeof(UIType), taskData.val);
                if (type == Type)
                {
                    taskData.TaskState = TaskState.End;
                }
            }
        }
    }

   
}
