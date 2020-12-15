using MREngine;
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

	AsyncOperation operation;
    string path;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.LoadingWnd;
        MutexInterface = true;
        progressBar.value = 0;
    }

    protected override void OnEnable()
    {
        if(SongsDataMng.GetInstance().GetSceneData != null) LoadScene(SongsDataMng.GetInstance().GetSceneData.sceneName);
	}

    private void Update()
    {
		if (operation == null) return;
        if (operation.isDone)
        {
            progressBar.value = 1;

            operation = null;
            Scene scene = SceneManager.GetSceneByPath(path);
            lblStatus.text = scene.name;
            SceneManager.SetActiveScene(scene);
            GameObject[] games = scene.GetRootGameObjects();
            Renderer[] renderers = null;
            Camera  camera = null;

            for (int i = 0; i < games.Length; i++)
            {
#if UNITY_EDITOR
                renderers = games[i].GetComponentsInChildren<Renderer>();
                for (int j = 0; j < renderers.Length; j++)
                {
                    renderers[j].sharedMaterial.shader = Shader.Find(renderers[j].sharedMaterial.shader.name);
                }
#endif
                if(camera == null) camera = games[i].GetComponent<Camera>();
            }
#if UNITY_EDITOR
            RenderSettings.skybox.shader = Shader.Find(RenderSettings.skybox.shader.name);
#endif
            CameraMng.GetInstance().InitScene(camera);
            UIMng.Instance.OpenUI(UIType.NONE);
        }
        else
        {
            progressBar.value = operation.progress;
        }
    }

    void UnLoadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.IsValid())
        {
            SceneManager.UnloadSceneAsync(scene);
        }
    }

    void LoadScene(string url)
    {
        CameraMng.GetInstance().UserControl.gameObject.SetActive(false);
        GameLoadTask loadTask = GameDataManager.GetInstance().GetGameTask(url);
        loadTask.OnTaskProgress += delegate (float proggpess)
        {
            if(proggpess >= 1)
            {
                path = loadTask.MainData.GetSceneName(url);
                Debug.Log(path);
                if(!string.IsNullOrEmpty(path))operation = SceneManager.LoadSceneAsync(path);
            }
        };
    }
}

