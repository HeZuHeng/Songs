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
    [XmlAttribute("场景名")]
    public string name;
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

    public ModelData() { }
#if SONGS_DEBUG
    public ModelData(string name, string assetName)
    {
        this.name = name;
        this.assetName = assetName;
    }
#endif
}