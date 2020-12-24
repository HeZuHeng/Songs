using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MREngine
{
    public class GameDataManager
    {      

        private static GameDataManager instance = null;
        public static GameDataManager GetInstance()
        {
            if(null == instance)
            {
                AssetBundle.UnloadAllAssetBundles(true);
                instance = new GameDataManager();
            }
            return instance;
        }

        private Dictionary<string, AbstractData> dataDict = new Dictionary<string, AbstractData>();
        private List<GameLoadTask> taskList = new List<GameLoadTask>();
        private GameLoadTask currentTask = null;

        private AssetBundleManifest manifest = null;
        SimpleInitSignal simpleInit;
        private Transform poolTransform = null;

        UnityWebRequest request = null;
        public void Startup(Transform poolTransform, SimpleInitSignal simpleInit)
        {
            this.simpleInit = simpleInit;
            string manifstPath = FileUtil.GetManifestPath();
            request = UnityWebRequestAssetBundle.GetAssetBundle(manifstPath);
            request.SendWebRequest();
        }

        public Transform GetObjectPoolTransform()
        {
            return poolTransform;
        }
        public AbstractData GetGameData(string url)
        {
            AbstractData data = null;
            //在任务队列里
            if (GetTaskInLoadingTask((url)) != null)
            {
                dataDict.TryGetValue(url, out data);
                data.AddRefCount();
                return data;
            }

            GameLoadTask task = new GameLoadTask();

            data = CreateGameData(url);
            data.AddRefCount();

            string[] dps = manifest.GetAllDependencies(url);
            if(null != dps && dps.Length > 0)
            {
                for(int i = 0; i < dps.Length; i++)
                {
                    AbstractData dpData = CreateGameData(dps[i]);
                    dpData.AddRefCount();
                    task.AddDpData(dpData);
                }              
            }

            task.MainData = data;
            taskList.Add(task);

            return data;
        }

        public GameLoadTask GetGameTask(string url)
        {
            url = url.ToLower();
            AbstractData data = null;
            GameLoadTask task = GetTaskInLoadingTask(url);
            //在任务队列里
            string[] dps = null;

            if (null != task)
            {
                dataDict.TryGetValue(url, out data);
                data.AddRefCount();

                dps = manifest.GetAllDependencies(url);
                if (null != dps && dps.Length > 0)
                {
                    for (int i = 0; i < dps.Length; i++)
                    {
                        AbstractData dpData = CreateGameData(dps[i]);
                        dpData.AddRefCount();
                        task.AddDpData(dpData);
                    }
                }
                return task;
            }

            task = new GameLoadTask();

            data = CreateGameData(url);
            data.AddRefCount();

            dps = manifest.GetAllDependencies(url);
            if (null != dps && dps.Length > 0)
            {
                for (int i = 0; i < dps.Length; i++)
                {
                    AbstractData dpData = CreateGameData(dps[i]);
                    dpData.AddRefCount();
                    task.AddDpData(dpData);
                }
            }

            task.MainData = data;
            taskList.Add(task);

            return task;
        }
        
        private AbstractData CreateGameData(string url)
        {

            AbstractData data = null;
            if (!dataDict.TryGetValue(url, out data))
            {
                data = new AbstractData(url);
                dataDict.Add(url, data);
            }
            
            return data;               
        }

        private GameLoadTask GetTaskInLoadingTask(string url)
        {           
            for(int i = 0; i < taskList.Count; i++)
            {
                if(taskList[i].MainData.URL == url)
                {
                    return taskList[i];
                }
            }

            return null;
        }

        public void ReleaseGameData(string url)
        {
            AbstractData data = GetGameData(url);
            //if (dataDict.TryGetValue(url, out data))
            //{

            //}
            ReleaseGameData(data);
        }

        public void ReleaseGameData(AbstractData data)
        {
            string[] dps = manifest.GetAllDependencies(data.URL);
            string url;

            if(null != dps && dps.Length > 0)
            {
                for(int i = 0; i < dps.Length; i++)
                {
                    AbstractData tmpData = null;                    
                    if (dataDict.TryGetValue(dps[i], out tmpData))
                    {
                        tmpData.RemoveRefCount();
                        if(tmpData.RefCount <= 0)
                        {
                            url = tmpData.URL;
                            tmpData.Reset();
                            dataDict.Remove(url);
                        }

                    }
                }               
            }

            data.RemoveRefCount();
            if(data.RefCount <=0)
            {
                url = data.URL;
                data.Reset();
                dataDict.Remove(url);
            }
        }
        
        public void FrameUpdate()
        {
            if(request != null && request.isDone)
            {
                DownloadHandlerAssetBundle handlerAssetBundle = request.downloadHandler as DownloadHandlerAssetBundle;
                AssetBundle assetBundle = handlerAssetBundle.assetBundle;
                manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                assetBundle.Unload(false);
                request = null;
                simpleInit?.Invoke();
            }
            if(null == currentTask && taskList.Count < 1)
            {
                return;
            }

            if(null == currentTask)
            {
                currentTask = taskList[0];
                currentTask.Start();               
            }
            else
            {
                if(currentTask.Update())
                {
                    currentTask.Reset();
                    currentTask = null;
                    taskList.RemoveAt(0);
                }
            }
        }
        
        public void Terminate()
        {
            if(null != dataDict)
            {
                var enumerator = dataDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.Reset();                  
                }
                dataDict.Clear();
                dataDict = null;
            }        

            if(null != taskList)
            {
                for(int i = 0; i < taskList.Count;  i++)
                {
                    taskList[i].Reset();
                }
                taskList.Clear();
                taskList = null;
            }
          
            if(null != currentTask)
            {
                currentTask.Reset();
                currentTask = null;
            }           
            manifest = null;
        }  
    }
}
