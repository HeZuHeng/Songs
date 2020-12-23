using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;
using Slate;

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
        SceneData sceneData = SongsDataMng.GetInstance().GetSceneData;
        if (sceneData  != null)
        {
        
            LoadScene(sceneData);
            //LoadSceneItem(sceneData.datas);
        }
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
       
    }

    void LoadScene(SceneData sceneData)
    {
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
                    SceneController.GetInstance().Init();

                    SceneManager.LoadSceneAsync(path);
                }
            }
            else
            {
                progressBar.value = progress;
                lblStatus.text = Mathf.Round(progress * 100f) + "%";
            }
        };
    }
}

