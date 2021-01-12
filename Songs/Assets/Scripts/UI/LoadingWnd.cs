using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;
using Slate;
using BuildUtil;

public class LoadingWnd : UIBase
{
	public Slider progressBar;
	public Text lblStatus;
    public float Progress { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.LoadingWnd;
        MutexInterface = false;
        progressBar.value = 0;
    }

    protected override void OnEnable()
    {
        Progress = 0;
        progressBar.value = 0;
        lblStatus.text = Mathf.Round(0 * 100f) + "%";
        SceneData sceneData = SongsDataMng.GetInstance().GetSceneData;
        if (sceneData  != null)
        {
        
            LoadScene(sceneData);
            //LoadSceneItem(sceneData.datas);
        }
        transform.SetAsLastSibling();
	}

    private void Update()
    {
       progressBar.value = (Progress + SceneController.GetInstance().Progress)/2f;
       lblStatus.text = Mathf.Round(progressBar.value * 100f) + "%";
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
        
        SceneController.GetInstance().InitScene = true;
    }

    void LoadScene(SceneData sceneData)
    {
        SceneController.GetInstance().Init();
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name.Equals(sceneData.sceneName))
        {
            SceneController.GetInstance().InitScene = true;
            OnSceneLoaded(scene, LoadSceneMode.Single);
            return;
        }
        GameLoadTask loadTask = GameDataManager.GetInstance().GetGameTask(sceneData.sceneName);
        loadTask.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                string path = loadTask.MainData.GetSceneName(sceneData.sceneName);
                Debug.Log(path);
                SceneManager.sceneLoaded += OnSceneLoaded;
                if (!string.IsNullOrEmpty(path))
                {
                    SceneManager.LoadSceneAsync(path);
                }
            }
            else
            {
                Progress = progress;
                
            }
        };
    }
}

