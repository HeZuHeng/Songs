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

    public void OnClose()
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