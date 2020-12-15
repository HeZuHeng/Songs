//using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public enum TaskType
{
    None =0,
    Move = 1,
    Click = 2,
    OpenWnd =3,
    Talk = 4,
}

[System.Serializable]
public class TasksConfig
{
    
    [XmlElement("SceneTasks")]
    public List<SceneTaskData> datas;

    public TasksConfig() { datas = new List<SceneTaskData>(); }
}

[System.Serializable]
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

[System.Serializable]
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
    public TaskData() { }
#if SONGS_DEBUG
    public TaskData(string name, int next)
    {
        this.name = name;
        this.next = next;
    }

    public TaskData(string name, int next, string des, TaskType type) : this(name, next)
    {
        this.des = des;
        this.type = type;
    }
#endif
}