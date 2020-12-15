using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MREngine;
using System;

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

    protected float LoadedCount { get; set; }

    Dictionary<int, IObject> spaceObjects = new Dictionary<int, IObject>();

    public SceneAssetObject AddSpaceAsset(int targetId, string assetName, string name, OnLoadProgressDelegate OnProgress = null)
    {
        if (OnProgress == null) OnProgress = OnAddProgress;

        if (spaceObjects.ContainsKey(targetId))
        {
            OnProgress(1);
            return (SceneAssetObject)spaceObjects[targetId];
        }
        SceneAssetObject obj = new SceneAssetObject(targetId, assetName);
        obj.OnProgress += OnProgress;

        spaceObjects[targetId] = obj;
        return obj;
    }

    public void RemoveSpaceObject(int targetId)
    {
        IObject obj = null;
        if (spaceObjects.TryGetValue(targetId, out obj))
        {
            spaceObjects.Remove(targetId);
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