using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MREngine;
using System;
using BuildUtil;

public class SceneMng
{
    private static SceneMng _instance = null;
    public static SceneMng GetInstance()
    {
        if (null == _instance)
        {
            _instance = new SceneMng();
        }

        return _instance;
    }

    static int IndexID = 1;

    public int GetCreateTargetID()
    {
        if (IndexID >= int.MaxValue)
        {
            IndexID = 1;
        }
        IndexID++;
        while (spaceObjects.ContainsKey(IndexID))
        {
            IndexID++;
        }
        return IndexID;
    }

    public SimpleProgressSignal OnSceneLoadProgress;
    public float Progress { get { return LoadedCount / spaceObjects.Count; } }

    public float LoadedCount { get; set; }

    Dictionary<int, IObject> spaceObjects = new Dictionary<int, IObject>();

    public SceneAssetObject AddSpaceAsset(int targetId,string assetName, string name, OnLoadProgressDelegate OnProgress = null)
    {
        if (OnProgress == null) OnProgress = OnAddProgress;
        if (spaceObjects.ContainsKey(targetId))
        {
            OnProgress(1);
            return (SceneAssetObject)spaceObjects[targetId];
        }
        if(targetId == 0) targetId = GetCreateTargetID();
        SceneAssetObject obj = new SceneAssetObject(targetId, assetName);
        obj.OnProgress += OnProgress;

        spaceObjects[targetId] = obj;
        return obj;
    }

    public SceneAssetObject AddSpaceAsset(ModelData modelData, OnLoadProgressDelegate OnProgress = null)
    {
        if (OnProgress == null) OnProgress = OnAddProgress;
        if (spaceObjects.ContainsKey(modelData.Id))
        {
            OnProgress(1);
            return (SceneAssetObject)spaceObjects[modelData.Id];
        }
        if (modelData.Id == 0) modelData.Id = GetCreateTargetID();
        SceneAssetObject obj = new SceneAssetObject(modelData);
        obj.OnProgress += OnProgress;

        spaceObjects[modelData.Id] = obj;
        return obj;
    }

    public SceneAssetObject GetSceneAssetObject(int targetId)
    {
        if (spaceObjects.ContainsKey(targetId))
        {
            return (SceneAssetObject)spaceObjects[targetId];
        }
        return null;
    }

    public void RemoveSpaceObject(int targetId)
    {
        IObject obj = null;
        if (spaceObjects.TryGetValue(targetId, out obj))
        {
            spaceObjects.Remove(targetId);
            LoadedCount--;
            if (obj.GetType() == typeof(SceneAssetObject))
            {
                IAssetObject asset = (IAssetObject)obj;
                if (asset != null)
                {
                    asset.Destroy();
                    //GameDataManager.GetInstance().ReleaseGameData(asset.URL);
                }
            }
        }
    }

    private void OnAddProgress(float _progress)
    {
        if (_progress >= 1)
        {
            LoadedCount++;
        }
        if (OnSceneLoadProgress != null)
        {
            OnSceneLoadProgress.Invoke(Progress);
        }
    }
}