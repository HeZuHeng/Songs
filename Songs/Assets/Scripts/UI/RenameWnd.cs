using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenameWnd : UIBase
{
    public InputField input;
    public Button submit;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.RenameWnd;
        MutexInterface = true;

        submit.onClick.AddListener(OnEnd);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    void OnEnd()
    {
        SongsDataMng.GetInstance().Player.name = input.text;
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
        UIMng.Instance.OpenUI(UIType.NONE);
    }
}
