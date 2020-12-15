using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public delegate void OnDataLoaded(string text);

public class DataLoader
{
    public UnityWebRequest request;
    public OnDataLoaded onDataLoaded;
}

public class SongsDataMng 
{
    public static string TaskPath = Application.streamingAssetsPath + "/TaskConfig.xml";
    public static string ModelPath = Application.streamingAssetsPath + "/ModelConfig.xml";

    static SongsDataMng instance;
    public static SongsDataMng GetInstance()
    {
        if (null == instance)
        {
            instance = new SongsDataMng();
        }
        return instance;
    }
    public MainPlayer Player { get; private set; }
    public TasksConfig GetTasksConfig { get; private set; }
    public ModelConfig GetModelConfig { get; private set; }
    public SceneData GetSceneData { get; private set; }

    private List<DataLoader> dataLoaders = new List<DataLoader>();

    public void Init()
    {
        Player = new MainPlayer();

        AddLoad(TaskPath, delegate (string text)
        {
            GetTasksConfig = (TasksConfig) XmlDeserialize(text,typeof(TasksConfig));
        });
        AddLoad(ModelPath, delegate (string text)
        {
            GetModelConfig = (ModelConfig)XmlDeserialize(text, typeof(ModelConfig));
            GetSceneData = GetModelConfig.datas[0];
        });
    }

    int index = 0;
    public void NextSceneData()
    {
        index++;
        if(index >= GetModelConfig.datas.Count)
        {
            index = 0;
        }
        GetSceneData = GetModelConfig.datas[index];
    }

    public void LoadUpdate()
    {
        for (int i = 0; i < dataLoaders.Count; )
        {
            if (dataLoaders[i].request.isDone)
            {
                if (dataLoaders[i].request.isHttpError)
                {
                    dataLoaders[i]?.onDataLoaded(string.Empty);
                }
                {
                    dataLoaders[i]?.onDataLoaded(dataLoaders[i].request.downloadHandler.text);
                }
                dataLoaders.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    public DataLoader AddLoad(string url, OnDataLoaded onDataLoaded)
    {
        DataLoader loader = new DataLoader();
        loader.request = UnityWebRequest.Get(url);
        loader.onDataLoaded = onDataLoaded;
        loader.request.SendWebRequest();
        dataLoaders.Add(loader);
        return loader;
    }

    public static object XmlDeserialize(string text,Type type)
    {
        StringReader reader = new StringReader(text);
        reader.ReadLine();
        XmlSerializer serializer = new XmlSerializer(type);
        object obj = serializer.Deserialize(reader);

        reader.Close();
        return obj;
    }
}
