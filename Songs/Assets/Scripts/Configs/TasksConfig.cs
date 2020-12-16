//using System;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public enum TaskType
{
    None =0,
    /// <summary>
    /// 模型移动
    /// </summary>
    Move = 1,
    /// <summary>
    /// 点击模型
    /// </summary>
    Click = 2,
    /// <summary>
    /// 打开UI界面
    /// </summary>
    OpenWnd =3,
    /// <summary>
    /// 人物对话
    /// </summary>
    Talk = 4,
    /// <summary>
    /// 场景进入动画
    /// </summary>
    DOTween = 5,
    /// <summary>
    /// 上帝视角漫游，摄像机不动
    /// </summary>
    GodRoams,
    /// <summary>
    /// 第三人称漫游，摄像机跟随人物移动
    /// </summary>
    ThirdPerson,
    /// <summary>
    /// 第一人称漫游，摄像机移动
    /// </summary>
    FirstPerson
}

public enum TaskState
{
    None = 0,
    Start,
    Run,
    End,
}

[Serializable]
public class TasksConfig
{
    
    [XmlElement("SceneTasks")]
    public List<SceneTaskData> datas;

    public TasksConfig() { datas = new List<SceneTaskData>(); }
}

[Serializable]
public class SceneTaskData
{
    [XmlAttribute("场景任务名")]
    public string name;
    [XmlAttribute("图标")]
    public string icon;
    [XmlAttribute("场景数据")]
    public string sceneDataName;
    [XmlElement("TaskDatas")]
    public List<TaskData> datas;

    public SceneTaskData() { datas = new List<TaskData>(); }
}

[Serializable]
public class TaskData
{
    [XmlAttribute("ID")]
    public int Id;
    [XmlAttribute("任务名")]
    public string name;
    [XmlAttribute("下一个")]
    public int next;
    [XmlAttribute("类型")]
    public TaskType type;
    [XmlAttribute("值")]
    public string val;
    [XmlAttribute("描述")]
    public string des;

    [NonSerialized]
    public TaskStateEvent onStateChange = new TaskStateEvent();

    [NonSerialized]
    TaskState taskState;
    public TaskState TaskState
    {
        get { return taskState; }
        set
        {
            if (taskState != value)
            {
                taskState = value;
                if(onStateChange != null) onStateChange.Invoke(taskState);
            }
        }
    }
    public TaskData()
    {
    }

}

public class TaskStateEvent : UnityEvent<TaskState>
{
    public TaskStateEvent() { }
}