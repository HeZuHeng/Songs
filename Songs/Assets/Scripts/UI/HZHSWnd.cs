using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HZHSWnd : UIBase
{
    public Button next;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HZHSWnd;
        MutexInterface = true;
        next.onClick.AddListener(OnClose);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        next.gameObject.SetActive(false);
    }

    void OnClose()
    {
        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        if (taskData != null)
        {
            if(taskData.type == TaskType.OpenWnd)
            {
                UIType type = (UIType)System.Enum.Parse(typeof(UIType), taskData.val);
                if(type == Type)
                {
                    taskData.TaskState = TaskState.End;
                }
            }
        }
        UIMng.Instance.OpenUI(UIType.NONE);
    }
}
