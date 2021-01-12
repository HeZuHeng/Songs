//using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace BuildUtil
{
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
        [XmlAttribute("Id")]
        public int Id;
        [XmlAttribute("模型名")]
        public string name;
        [XmlAttribute("模型路径")]
        public string parentPath;
        [XmlAttribute("资源名")]
        public string assetName;
        
        public Vector3 position;
        public Vector3 eulerAngle;
        public Vector3 scale;
        [XmlAttribute("图标")]
        public string icon;
        [XmlAttribute("默认显隐")]
        public bool active = true;
        public ModelData() { }

#if UNITY_EDITOR
        public ModelData(string name, string assetName)
        {
            this.name = name;
            this.assetName = assetName;
            position = Vector3.zero;
            eulerAngle = new Vector3(0, 0, 0);
        }

        public ModelData(string name, string assetName, string path)
        {
            this.name = name;
            this.assetName = assetName;
            this.parentPath = path;
            position = Vector3.zero;
            eulerAngle = new Vector3(0, 0, 0);
        }
#endif
    }
}