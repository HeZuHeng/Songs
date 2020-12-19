using MREngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAssetObject : IAssetObject
{
    public OnLoadProgressDelegate OnProgress;
    public bool IsDone { get; protected set; }
    public float Progress { get; protected set; }

    public Collider Col { get; private set; }
    public string Des { get; private set; }
    public bool Show { get; private set; }

    protected GameLoadTask LoadTask { get; set; }

    public SceneAssetObject(int targetId, string url)
    {
        TargetId = targetId;
        Position = Vector3.zero;
        Euler = Vector3.zero;
        Scale = Vector3.one;
        URL = url;
        IsDone = false;
        LoadTask = GameDataManager.GetInstance().GetGameTask(URL);
        if (LoadTask.Progress >= 1)
        {
            InitGameObject();
        }
        else
        {
            LoadTask.OnTaskProgress += OnLoadProgress;
        }
    }

    private void OnLoadProgress(float _progress_)
    {
        if (_progress_ >= 1f)
        {
            if (LoadTask != null) LoadTask.OnTaskProgress -= OnLoadProgress;
            InitGameObject();
            Progress = 1f;
        }
        else
        {
            Progress = _progress_;
        }
        if (null != OnProgress)
        {
            OnProgress.Invoke(Progress);
        }
    }

    private void InitGameObject()
    {
        if (LoadTask != null && !IsDone)
        {
            GameObject gameObject = LoadTask.MainData.LoadGameObject(URL);
#if UNITY_EDITOR
            Transform[] games = gameObject.GetComponentsInChildren<Transform>();
            Renderer[] renderers = null;
            for (int i = 0; i < games.Length; i++)
            {
                renderers = games[i].GetComponentsInChildren<Renderer>();
                for (int j = 0; j < renderers.Length; j++)
                {
                    renderers[j].sharedMaterial.shader = Shader.Find(renderers[j].sharedMaterial.shader.name);
                }
            }
#endif
            InitAssetObject(gameObject.transform);
        }
        LoadTask = null;
        IsDone = true;
    }

    public override void InitAssetObject(Transform asset)
    {
        base.InitAssetObject(asset);
        if (Tran == null) return;
        Col = Tran.GetComponentInChildren<Collider>();
    }

    public override void Destroy()
    {
        OnProgress = null;
        if (LoadTask != null)
        {
            LoadTask.OnTaskProgress -= OnLoadProgress;
            LoadTask = null;
        }
        Col = null;
        IsDone = true;
        base.Destroy();
    }
}
