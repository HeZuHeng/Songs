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

    Camera camera = null;
    Vector3 initPosition = Vector3.zero;
    Vector3 initRotation = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.LoadingWnd;
        MutexInterface = true;
        progressBar.value = 0;
    }

    protected override void OnEnable()
    {
        if (SongsDataMng.GetInstance().GetSceneData != null) LoadScene(SongsDataMng.GetInstance().GetSceneData);
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
       
        UIMng.Instance.OpenUI(UIType.NONE);
        UIMng.Instance.ActivationUI(UIType.SettingWnd);
    }

    void LoadScene(SceneData sceneData)
    {
        UIMng.Instance.ConcealUI(UIType.SettingWnd);
        UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);

        CameraMng.GetInstance().UserControl.gameObject.SetActive(false);

        GameLoadTask loadCutscenen = GameDataManager.GetInstance().GetGameTask(sceneData.sceneCutscenen);
        loadCutscenen.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                GameObject obj = loadCutscenen.MainData.LoadGameObject(sceneData.sceneCutscenen);
                if (obj != null)
                {
                    Cutscene playCutscene = obj.GetComponent<Cutscene>();
                    CameraMng.GetInstance().InitScene(playCutscene);
                }
            }
        };

        GameLoadTask loadCamera = GameDataManager.GetInstance().GetGameTask(sceneData.sceneCamera);
        loadCamera.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                GameObject obj = loadCamera.MainData.LoadGameObject(sceneData.sceneCamera);
                if(obj != null)
                {
                    camera = obj.GetComponent<Camera>();
                }
                CameraMng.GetInstance().InitScene(camera);
            }
        };
        GameLoadTask loadPosition = GameDataManager.GetInstance().GetGameTask(sceneData.scenePosition);
        loadPosition.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                GameObject obj = loadPosition.MainData.LoadGameObject(sceneData.scenePosition);
                if (obj != null)
                {
                    initPosition = obj.transform.position;
                    initRotation = obj.transform.eulerAngles;
                }
                CameraMng.GetInstance().InitScene(initPosition, initRotation);
            }
        };
        GameLoadTask loadTask = GameDataManager.GetInstance().GetGameTask(sceneData.sceneName);
        loadTask.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                string path = loadTask.MainData.GetSceneName(sceneData.sceneName);
                Debug.Log(path);
                SceneManager.sceneLoaded += OnSceneLoaded;
                if (!string.IsNullOrEmpty(path))SceneManager.LoadSceneAsync(path);
            }
            else
            {
                progressBar.value = progress;
                lblStatus.text = Mathf.Round(progress * 100f) + "%";
            }
        };
    }
}

