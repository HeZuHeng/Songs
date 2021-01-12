using BuildUtil;
using MREngine;
using SpaceSimulation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnAnimatorEnd(string aName);

public class SceneAssetObject : IAssetObject
{
    public ModelData Data { get; private set; }
    public Animator MAnimator { get; private set; }
    public string AnimationName { get; private set; }
    public OnAnimatorEnd OnAnimatorEnd { get; private set; }
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

    public SceneAssetObject(ModelData data)
    {
        Data = data;
        Name = data.name;
        TargetId = Data.Id;
        Position = Data.position;
        Euler = Data.eulerAngle;
        Scale = data.scale;
        URL = Data.assetName;
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
            InitAssetObject(gameObject.transform);
        }
        LoadTask = null;
        IsDone = true;
    }

    public void Start(Transform parent)
    {
        if (parent != null)
        {
            if (string.IsNullOrEmpty(Data.parentPath) || parent.Find(Data.parentPath) == null)
            {
                Tran.SetParent(parent);
            }
            else
            {
                Tran.SetParent(parent.Find(Data.parentPath));
            }
        }
        Tran.name = Name;
        Tran.position = Position;
        Tran.eulerAngles = Euler;
        Tran.localScale = Scale == Vector3.zero ? Vector3.one : Scale;
        if (Tran != null) Tran.gameObject.SetActive(Data.active);
    }

    public override void InitAssetObject(Transform asset)
    {
        asset.SetParent(GameCenter.AssetsParent);

        base.InitAssetObject(asset);
        if (Tran == null) return;
        MAnimator = Tran.GetComponentInChildren<Animator>();
        Col = Tran.GetComponentInChildren<Collider>();
        if(MAnimator != null) MAnimator.enabled = MAnimator.parameterCount != 0;
        Tran.gameObject.SetActive(false);
    }

    public bool PlayAnimator(string aName, float val,float speed, OnAnimatorEnd onAnimatorEnd)
    {
        if(MAnimator != null)
        {
            AnimationName = aName.ToLower();
            MAnimator.SetFloat(AnimationName, val);
            MAnimator.speed = speed;
            OnAnimatorEnd = onAnimatorEnd;
            return MAnimator.GetCurrentAnimatorStateInfo(0).loop;
        }
        return false;
    }

    public bool PlayAnimator(string aName, bool val, float speed, OnAnimatorEnd onAnimatorEnd)
    {
        if (MAnimator != null)
        {
            MAnimator.enabled = true;
            AnimationName = aName.ToLower();
            MAnimator.SetBool(AnimationName, val);
            MAnimator.speed = speed;
            OnAnimatorEnd = onAnimatorEnd;
            return MAnimator.GetCurrentAnimatorStateInfo(0).loop;
        }
        return false;
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
