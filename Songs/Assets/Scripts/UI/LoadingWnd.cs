﻿using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class LoadingWnd : UIBase
{
	public Slider progressBar;
	public Text lblStatus;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.LoadingWnd;
        MutexInterface = true;
        progressBar.value = 0;
    }

    protected override void OnEnable()
    {
        if (SongsDataMng.GetInstance().GetSceneData != null) LoadScene(SongsDataMng.GetInstance().GetSceneData.sceneName);
	}

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        progressBar.value = 1;
        lblStatus.text = "100%";

        SceneManager.SetActiveScene(scene);
        GameObject[] games = scene.GetRootGameObjects();
        Renderer[] renderers = null;
        Camera camera = null;
        string str = "InitPosition";
        Vector3 initPosition = Vector3.zero;
        Vector3 initRotation = Vector3.zero;
        for (int i = 0; i < games.Length; i++)
        {
#if UNITY_EDITOR
            renderers = games[i].GetComponentsInChildren<Renderer>();
            for (int j = 0; j < renderers.Length; j++)
            {
                renderers[j].sharedMaterial.shader = Shader.Find(renderers[j].sharedMaterial.shader.name);
            }
#endif
            if (camera == null) camera = games[i].GetComponent<Camera>();
            if (games[i].name.Equals(str))
            {
                initPosition = games[i].transform.position;
                initRotation = games[i].transform.eulerAngles;
            }
        }
#if UNITY_EDITOR
        RenderSettings.skybox.shader = Shader.Find(RenderSettings.skybox.shader.name);
#endif
        CameraMng.GetInstance().InitScene(camera, initPosition, initRotation);
        UIMng.Instance.OpenUI(UIType.NONE);
        UIMng.Instance.ActivationUI(UIType.SettingWnd);
    }

    void LoadScene(string url)
    {
        UIMng.Instance.ConcealUI(UIType.SettingWnd);
        UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);

        CameraMng.GetInstance().UserControl.gameObject.SetActive(false);
        GameLoadTask loadTask = GameDataManager.GetInstance().GetGameTask(url);
        loadTask.OnTaskProgress += delegate (float proggpess)
        {
            if (proggpess >= 1)
            {
                string path = loadTask.MainData.GetSceneName(url);
                Debug.Log(path);
                SceneManager.sceneLoaded += OnSceneLoaded;
                if (!string.IsNullOrEmpty(path))SceneManager.LoadSceneAsync(path);
            }
            else
            {
                progressBar.value = proggpess;
                lblStatus.text = Mathf.Round(proggpess * 100f) + "%";
            }
        };
    }
}

