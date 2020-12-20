//using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class ModelConfig
{
    [XmlElement("SceneDatas")]
    public List<SceneData> datas;

    public ModelConfig() { datas = new List<SceneData>(); }
}

[System.Serializable]
public class SceneData
{
    [XmlAttribute("场景")]
    public string name;
    [XmlAttribute("场景资源名")]
    public string sceneName;
    [XmlAttribute("摄像机")]
    public string sceneCamera;
    [XmlAttribute("位置")]
    public string scenePosition;
    [XmlAttribute("镜头")]
    public string sceneCutscenen;
    [XmlElement("ModelDatas")]
    public List<ModelData> datas;

    public SceneData() { datas = new List<ModelData>(); }
}

[System.Serializable]
public class ModelData
{
    [XmlAttribute("模型名")]
    public string name;
    [XmlAttribute("资源名")]
    public string assetName;
    //[XmlAttribute("父节点")]
    //public string parent;

    [XmlAttribute("图标")]
    public string icon;
    public ModelData() { }
#if SONGS_DEBUG
    public ModelData(string name, string assetName)
    {
        this.name = name;
        this.assetName = assetName;
    }
#endif
}