using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LookSongWnd : UIBase
{
    public DOTweenAnimation tweenAnimation;

    public Image image1;
    public Image img;
    public Image image2;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.LookSongWnd;
        MutexInterface = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        image1.gameObject.SetActive(false);
        image2.gameObject.SetActive(false);
        img.gameObject.SetActive(false);
        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        if(taskData.Id == 202)
        {
            image1.gameObject.SetActive(true);
        }
        if (taskData.Id == 203)
        {
            image2.gameObject.SetActive(true);
        }
        tweenAnimation.transform.localScale = Vector3.one * 0.1f;
        tweenAnimation.DORestart();
    }

    public void OnEnd()
    {
        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        if (taskData.Id == 202)
        {
            //img.gameObject.SetActive(true);
        }
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
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
