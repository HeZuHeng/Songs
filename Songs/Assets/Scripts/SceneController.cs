using MREngine;
using Slate;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimatorType
{
    BOOl = 1,
    FLOAT,
}

public class SceneController 
{

    private static SceneController _instance = null;
    public static SceneController GetInstance()
    {
        if (null == _instance)
        {
            _instance = new SceneController();
        }
        return _instance;
    }

    public bool InitScene = false;

    SceneData CurSceneData;
    GameObject playCutsceneObj;
    GameObject cameraObj;
    GameObject initTranObj;

    List<SceneAssetObject> palyAnimators = new List<SceneAssetObject>();

    public void Init()
    {
        Close();
        InitScene = false;
        SceneMng.GetInstance().OnSceneLoadProgress += OnSceneLoaded;
        SceneData sceneData = SongsDataMng.GetInstance().GetSceneData;
        CurSceneData = sceneData;
        if (sceneData == null) return;

        GameLoadTask loadCutscenen = GameDataManager.GetInstance().GetGameTask(CurSceneData.sceneCutscenen);
        loadCutscenen.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                playCutsceneObj = loadCutscenen.MainData.LoadGameObject(CurSceneData.sceneCutscenen);
                if (playCutsceneObj != null)
                {
                    Cutscene playCutscene = playCutsceneObj.GetComponent<Cutscene>();
                    CameraMng.GetInstance().InitScene(playCutscene);
                }
            }
        };

        GameLoadTask loadCamera = GameDataManager.GetInstance().GetGameTask(CurSceneData.sceneCamera);
        loadCamera.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                cameraObj = loadCamera.MainData.LoadGameObject(CurSceneData.sceneCamera);
                if (cameraObj != null)
                {
                    cameraObj.AddComponent<HighlightingEffect>();
                    Camera camera = cameraObj.GetComponent<Camera>();
                    CameraMng.GetInstance().InitScene(camera);
                }
            }
        };
        GameLoadTask loadPosition = GameDataManager.GetInstance().GetGameTask(CurSceneData.scenePosition);
        loadPosition.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                initTranObj = loadPosition.MainData.LoadGameObject(CurSceneData.scenePosition);
                if (initTranObj != null)
                {
                    Vector3 initPosition = initTranObj.transform.position;
                    Vector3 initRotation = initTranObj.transform.eulerAngles;
                    CameraMng.GetInstance().InitScene(initPosition, initRotation);
                }
            }
        };

        ModelData modelData = null;
        for (int i = 0; i < CurSceneData.datas.Count; i++)
        {
            modelData = CurSceneData.datas[i];
            SceneMng.GetInstance().AddSpaceAsset(modelData);
        }

        SceneAssetObject assetObject = SceneMng.GetInstance().AddSpaceAsset(1, "nvyk", "女游客");
    }

    public void Close()
    {
        UIMng.Instance.ConcealUI(UIType.SettingWnd);
        UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);

        SceneMng.GetInstance().OnSceneLoadProgress -= OnSceneLoaded;
        palyAnimators.Clear();
        CameraMng.GetInstance().ResetMove();
        if(playCutsceneObj != null) GameObject.DestroyImmediate(playCutsceneObj);
        if (cameraObj != null) GameObject.DestroyImmediate(cameraObj);
        if (initTranObj != null) GameObject.DestroyImmediate(initTranObj);
        if (CurSceneData == null) return;
        for (int i = 0; i < CurSceneData.datas.Count; i++)
        {
            SceneMng.GetInstance().RemoveSpaceObject(CurSceneData.datas[i].Id);
        }
    }

    public void Start()
    {
        SceneAssetObject sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        CameraMng.GetInstance().InitPlayer(sceneAsset.Tran);
        sceneAsset.Tran.gameObject.SetActive(false);


        UIMng.Instance.OpenUI(UIType.NONE);
        UIMng.Instance.ActivationUI(UIType.SettingWnd);
    }

    public void AddPlayAnimator(SceneAssetObject sceneAsset)
    {
        bool exit = false;
        for (int i = 0; i < palyAnimators.Count; i++)
        {
            if(palyAnimators[i].TargetId == sceneAsset.TargetId)
            {
                exit = true;
            }
        }
        if (!exit)
        {
            palyAnimators.Add(sceneAsset);
        }
    }

    public void FrameUpdate()
    {
        if (!InitScene) return;
        AnimatorStateInfo stateInfo;
        for (int i = 0; i < palyAnimators.Count;)
        {
            stateInfo = palyAnimators[i].MAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime >= 1.0f && stateInfo.IsName(palyAnimators[i].AnimationName))
            {
                SceneAssetObject sceneAsset = palyAnimators[i];
                palyAnimators.RemoveAt(i);
                if (sceneAsset.OnAnimatorEnd != null) sceneAsset.OnAnimatorEnd(sceneAsset.AnimationName);
            }
            else
            {
                i++;
            }
        }
    }

    void OnSceneLoaded(float progress)
    {
        if(progress >= 1)
        {
            SceneMng.GetInstance().OnSceneLoadProgress -= OnSceneLoaded;
            InitScene = true;
            Start();
        }
    }
}