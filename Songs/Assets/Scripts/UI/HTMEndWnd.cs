using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HTMEndWnd : UIBase
{
    public Button next;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HTMEndWnd;
        MutexInterface = true;
        next.onClick.AddListener(OnClose);
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