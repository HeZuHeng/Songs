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
}

[System.Serializable]
public class TasksConfig
{
    [XmlElement("TaskDatas")]
    public List<TaskData> datas;

    public TasksConfig() { datas = new List<TaskData>(); }
}

[System.Serializable]
public class TaskData
{
    [XmlAttribute("ID")]
    public int Id;
    [XmlAttribute("任务")]
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