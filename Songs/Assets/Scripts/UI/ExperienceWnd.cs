using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class ExperienceWnd : UIBase
{
    public TrendsText trendsText;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.ExperienceWnd;
        MutexInterface = true;

        trendsText.m_CallBack.AddListener(OnClose);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        trendsText.Play();
    }

    public void OnClose()
    {
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
