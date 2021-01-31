using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatRenameWnd : UIBase
{
    public InputField input;
    public Button submit;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HTMBoatNameWnd;
        MutexInterface = true;

        submit.onClick.AddListener(OnEnd);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    void OnEnd()
    {
        UIMng.Instance.OpenUI(UIType.NONE);
        SongsDataMng.GetInstance().Player.name = input.text;
        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        if (taskData != null)
        {
            if (taskData.type == TaskType.OpenWnd)
            {
                UIType type = (UIType)System.Enum.Parse(typeof(UIType), taskData.val);
                if (type == Type)
                {
                    MainPlayer.songResultInfo.FillAnswer(11, string.Empty, 1, AnswerType.Operating);
                    taskData.TaskState = TaskState.End;
                }
            }
        }
       
    }
}
