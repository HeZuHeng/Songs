using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HTMMapPathWnd : UIBase
{
    public Button Button_next;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HTMMapPathWnd;
        MutexInterface = true;
        Initialized();
    }

    private void Initialized()
    {
        if(Button_next != null) Button_next.onClick.AddListener(OnClickNext);
    }



    protected override void OnEnable()
    {
        base.OnEnable();
    }






    public void OnClickNext()
    {
        //to do:切换界面
        Debug.Log("切换界面");
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
