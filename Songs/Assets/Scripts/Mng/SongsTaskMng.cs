using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongsTaskMng
{
    static SongsTaskMng instance;
    public static SongsTaskMng GetInstance()
    {
        if (null == instance)
        {
            instance = new SongsTaskMng();
        }
        return instance;
    }

    private Dictionary<int,TaskData> taskDic = new Dictionary<int, TaskData>();

    public void Init(List<TaskData> taskDatas)
    {
        for (int i = 0; i < taskDatas.Count; i++)
        {
            taskDic[taskDatas[i].Id] = taskDatas[i];
        }

        RunTask(taskDatas[0].Id);
    }

    public bool RunTask(int Id)
    {
        TaskData task = null;
        taskDic.TryGetValue(Id, out task);
        if (task == null) return false;
        Move(task);
        return false;
    }

    private void Move(TaskData task)
    {

    }
}
