using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HTMMapPathWnd : UIBase
{
    public Button Button_next;

    public Button[] maps;

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

        for (int i = 0; i < maps.Length; i++)
        {
            maps[i].onClick.AddListener(OnEnd);
        }
    }



    protected override void OnEnable()
    {
        base.OnEnable();
        int index = 0;
        if(SceneController.ChildController.GetType() == typeof(HTMStartController))
        {
            index = 0;
        }

        if (SceneController.ChildController.GetType() == typeof(ImageSelfController))
        {
            index = 1;
        }

        if (SceneController.ChildController.GetType() == typeof(CoverSongController))
        {
            index = 2;
        }

        if (SceneController.ChildController.GetType() == typeof(EqualityController))
        {
            index = 3;
        }

        if (SceneController.ChildController.GetType() == typeof(DemocracyController))
        {
            index = 4;
        }

        for (int i = 0; i < maps.Length; i++)
        {
            maps[i].transform.parent.gameObject.SetActive(index == i);
        }
    }

    public void OnClickNext()
    {
        Button_next.transform.parent.gameObject.SetActive(false);

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
