using System;
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
    public static string HZHSTaskPath = Application.streamingAssetsPath + "/HZHSTaskConfig.xml";
    public static string HTMTaskPath = Application.streamingAssetsPath + "/HTMTaskConfig.xml";
    public static string ModelPath = Application.streamingAssetsPath + "/ModelConfig.xml";
    public static string QuestionBankPath = Application.streamingAssetsPath + "/QuestionBankConfig.xml";

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
    public OnDataLoaded OnDataLoaded { get; set; }
    private TasksConfig GetHZHSTasks { get;  set; }
    private TasksConfig GetHTMTasks { get;  set; }
    private ModelConfig GetModelConfig { get; set; }
    private QuestionBankConfig GetQuestionBankConfig { get; set; }
    /// <summary>
    /// 当前场景模型数据
    /// </summary>
    public SceneData GetSceneData { get; private set; }
    /// <summary>
    /// 当前分支数据
    /// </summary>
    public TasksConfig GetTasksConfig { get; private set; }
    /// <summary>
    /// 当前场景任务数据
    /// </summary>
    public SceneTaskData GetSceneTaskData { get; private set; }

    /// <summary>
    /// 当前任务数据
    /// </summary>
    public TaskData GetTaskData { get; private set; }
    /// <summary>
    /// 阅读的诗歌文件路径
    /// </summary>
    public string GetSongFilePath { get; set; }

    public QuestionBankData GetQuestionBankData { get; private set; }

    private List<DataLoader> dataLoaders = new List<DataLoader>();
    public void Init()
    {
        Player = new MainPlayer();

        AddLoad(HZHSTaskPath, delegate (string text)
        {
            GetHZHSTasks = (TasksConfig) XmlDeserialize(text,typeof(TasksConfig));
        });
        AddLoad(HTMTaskPath, delegate (string text)
        {
            GetHTMTasks = (TasksConfig)XmlDeserialize(text, typeof(TasksConfig));
        });
        AddLoad(ModelPath, delegate (string text)
        {
            GetModelConfig = (ModelConfig)XmlDeserialize(text, typeof(ModelConfig));
            GetSceneData = GetModelConfig.datas[0];
        });
        AddLoad(QuestionBankPath, delegate (string text)
        {
            GetQuestionBankConfig = (QuestionBankConfig)XmlDeserialize(text, typeof(QuestionBankConfig));
        });
    }
    public void SetTaskConfigData(int val)
    {
        if(val == 1)
        {
            GetTasksConfig = GetHZHSTasks;
        }
        else
        {
            GetTasksConfig = GetHTMTasks;
        }
    }
    public void SetSceneTaskData(SceneTaskData val)
    {
        GetSceneTaskData = val;
        for (int i = 0; i < GetModelConfig.datas.Count; i++)
        {
            if (GetModelConfig.datas[i].name.Equals(val.sceneDataName))
            {
                GetSceneData = GetModelConfig.datas[i];
            }
        }
        if (GetSceneTaskData.datas.Count > 0)
        {
            GetTaskData = GetSceneTaskData.datas[0];
            GetTaskData.TaskState = TaskState.Start;
        }
    }

    public SceneTaskData GetSceneTaskDataFromName(string name)
    {
        SceneTaskData sceneTask = null;
        for (int i = 0; i < GetHZHSTasks.datas.Count; i++)
        {
            if (GetHZHSTasks.datas[i].name.Equals(name))
            {
                sceneTask = GetHZHSTasks.datas[i];
            }
        }
        for (int i = 0; i < GetHTMTasks.datas.Count; i++)
        {
            if (GetHTMTasks.datas[i].name.Equals(name))
            {
                sceneTask = GetHTMTasks.datas[i];
            }
        }
        return sceneTask;
    }

    public void SetNextTaskData(TaskData val)
    {
        for (int i = 0; i < GetSceneTaskData.datas.Count; i++)
        {
            if (GetSceneTaskData.datas[i].Id == val.next)
            {
                GetTaskData = GetSceneTaskData.datas[i];
                GetTaskData.TaskState = TaskState.Start;
            }
        }
    }
    public void SetNextTaskData(string val)
    {
        for (int i = 0; i < GetSceneTaskData.datas.Count; i++)
        {
            if (GetSceneTaskData.datas[i].name.Equals(val))
            {
                GetTaskData = GetSceneTaskData.datas[i];
                GetTaskData.TaskState = TaskState.Start;
            }
        }
    }

    public ModelData GetModelData(string modelName)
    {
        for (int i = 0; i < GetSceneData.datas.Count; i++)
        {
            if (GetSceneData.datas[i].name.Equals(modelName))
            {
                return GetSceneData.datas[i];
            }
        }
        return null;
    }

    public QuestionBankData SetQuestion(int questionId)
    {
        for (int i = 0; i < GetQuestionBankConfig.datas.Count; i++)
        {
            if (GetQuestionBankConfig.datas[i].Id == questionId)
            {
                GetQuestionBankData = GetQuestionBankConfig.datas[i];
                return GetQuestionBankData;
            }
        }
        return null;
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
#if SONGS_DEBUG
        if (dataLoaders.Count == 0)
        {
            if(OnDataLoaded != null)
            {
                OnDataLoaded(string.Empty);
                OnDataLoaded = null;
            }
        }
#endif
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
