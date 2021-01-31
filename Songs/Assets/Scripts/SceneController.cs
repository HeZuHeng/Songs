using MREngine;
using Slate;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LaoZiCloudSDK.CameraHelper;
using BuildUtil;
using UnityEngine.SceneManagement;

public delegate void OnStateEndDelegate(State state);
public delegate void OnStateChangeDelegate();

public enum AnimatorType
{
    BOOl = 1,
    FLOAT,
}

public enum State
{
    InitMoveCamera,
    TalkCamera,
    TalkCameraOne,
    TalkCameraTwo,
    TalkCameraThere,
    TalkCameraFour,
    TalkCameraError,
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
    public static TerrainController TerrainController { get; set; }
    public static ChildController ChildController { get; private set; }

    public bool InitScene = false;
    public bool InitSceneObject = false;
    public float Progress { get; private set; }
    SceneData CurSceneData;
    GameObject playCutsceneObj;
    GameObject cameraObj;
    GameObject newCameraObj;
    GameObject initTranObj;
    List<SceneAssetObject> palyAnimators = new List<SceneAssetObject>();
    int totalLoadNum = 4;
    int loadNum = 0;

    public void Init()
    {
        Close();
        InitScene = false;
        InitSceneObject = false;
        loadNum = 0;
        SceneData sceneData = SongsDataMng.GetInstance().GetSceneData;
        CurSceneData = sceneData;
        if (sceneData == null) return;

        SceneMng.GetInstance().OnSceneLoadProgress += OnSceneLoaded;

        GameLoadTask loadCutscenen = GameDataManager.GetInstance().GetGameTask(CurSceneData.sceneCutscenen);
        loadCutscenen.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                loadNum++;
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
                loadNum++;
                newCameraObj = loadCamera.MainData.LoadGameObject(CurSceneData.sceneCamera);
                if (newCameraObj != null)
                {
                    newCameraObj.SetActive(false);
                    Camera camera = newCameraObj.GetComponent<Camera>();
                    CameraMng.GetInstance().InitScene(camera);
                }
            }
        };
        GameLoadTask loadPosition = GameDataManager.GetInstance().GetGameTask(CurSceneData.scenePosition);
        loadPosition.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                loadNum++;
                initTranObj = loadPosition.MainData.LoadGameObject(CurSceneData.scenePosition);
                if (initTranObj != null)
                {
                    Vector3 initPosition = initTranObj.transform.position;
                    Vector3 initRotation = initTranObj.transform.eulerAngles;
                    CameraMng.GetInstance().InitScene(initPosition, initRotation);
                }
            }
        };

        if (CurSceneData.datas.Count <= 0) loadNum++;
        ModelData modelData = null;
        for (int i = 0; i < CurSceneData.datas.Count; i++)
        {
            modelData = CurSceneData.datas[i];
            SceneMng.GetInstance().AddSpaceAsset(modelData);
        }

        //HZHS-------------------------------------
        if (HZHSStartController.Name.Equals(sceneData.name))
        {
            ChildController = new HZHSStartController();
        }
        if (HeavenController.Name.Equals(sceneData.name))
        {
            ChildController = new HeavenController();
        }

        if (ThreeSongsController.Name.Equals(sceneData.name))
        {
            ChildController = new ThreeSongsController();
        }
        if (ArtisticController.Name.Equals(sceneData.name))
        {
            ChildController = new ArtisticController();
        }

        //HTM-------------------------------------
        if (HTMStartController.Name.Equals(sceneData.name))
        {
            ChildController = new HTMStartController();
        }
        if (ThreePictureController.Name.Equals(sceneData.name))
        {
            ChildController = new ThreePictureController();
        }
        if (ImageSelfController.Name.Equals(sceneData.name))
        {
            ChildController = new ImageSelfController();
        }
        if (CoverSongController.Name.Equals(sceneData.name))
        {
            ChildController = new CoverSongController();
        }
        if (EqualityController.Name.Equals(sceneData.name))
        {
            ChildController = new EqualityController();
        }
        if (DemocracyController.Name.Equals(sceneData.name))
        {
            ChildController = new DemocracyController();
        }
        
    }

    public void Close()
    {
        Progress = 0;
        if (ChildController != null) ChildController.Close();
        ChildController = null;
        UIMng.Instance.ConcealUI(UIType.AnswerWnd);
        UIMng.Instance.ConcealUI(UIType.MemoryWnd);
        UIMng.Instance.ConcealUI(UIType.LeftDialogueWnd);
        UIMng.Instance.ConcealUI(UIType.SettingWnd);
        UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        SceneMng.GetInstance().OnSceneLoadProgress -= OnSceneLoaded;
        palyAnimators.Clear();
        CameraMng.GetInstance().ResetMove();

        if(playCutsceneObj != null) GameObject.DestroyImmediate(playCutsceneObj);
        if (initTranObj != null) GameObject.DestroyImmediate(initTranObj);
        if (CurSceneData == null) return;
        //SceneMng.GetInstance().RemoveSpaceObject(1);
        for (int i = 0; i < CurSceneData.datas.Count; i++)
        {
            SceneMng.GetInstance().RemoveSpaceObject(CurSceneData.datas[i].Id);
        }
    }

    public void Start()
    {
        TerrainController terrainController = null;
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] gameObjects = scene.GetRootGameObjects();

        GameObject root = null;
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if ("Root".Equals(gameObjects[i].name))
            {
                root = gameObjects[i];
            }
        }
        if (cameraObj != null) GameObject.DestroyImmediate(cameraObj);
        cameraObj = newCameraObj;
        if (cameraObj != null)
        {
            cameraObj.AddComponent<HighlightingEffect>();
            cameraObj.gameObject.SetActive(true);
        }
        newCameraObj = null;
        SceneAssetObject assetObject = null;
        for (int i = 0; i < CurSceneData.datas.Count; i++)
        {
            assetObject = SceneMng.GetInstance().GetSceneAssetObject(CurSceneData.datas[i].Id);
            if (assetObject != null) assetObject.Start(root.transform);
        }

        terrainController = root.GetComponent<TerrainController>();
        if (terrainController == null)
        {
            terrainController = root.AddComponent<TerrainController>();
        }
        TerrainController = terrainController;

        SceneAssetObject sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        CameraMng.GetInstance().InitPlayer(sceneAsset.Tran);
        sceneAsset.Tran.gameObject.SetActive(false);
        if (ChildController != null) ChildController.Init();
        UIMng.Instance.ConcealUI(UIType.AnswerWnd);
        UIMng.Instance.ConcealUI(UIType.SelectPlotWnd);
        UIMng.Instance.ConcealUI(UIType.LoadingWnd);
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

    public void ToState(State state, OnStateEndDelegate onStateEnd)
    {
        if(ChildController != null) ChildController.ToState(state, onStateEnd);
    }

    public void FrameUpdate()
    {
        if(InitScene && !InitSceneObject && totalLoadNum <= loadNum)
        {
            InitSceneObject = true;
            Start();
        }
        if(!InitSceneObject || !InitScene)
        {
            return;
        }
        AnimatorStateInfo stateInfo;
        for (int i = 0; i < palyAnimators.Count;)
        {
            if (palyAnimators[i].MAnimator != null)
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
            else
            {
                i++;
            }
        }
    }

    void OnSceneLoaded(float progress)
    {
        Progress = progress;
        if (progress >= 1)
        {
            SceneMng.GetInstance().OnSceneLoadProgress -= OnSceneLoaded;
            loadNum++;
        }
    }
}

public class ChildController
{
    public State GetState { get; set; }

    public virtual void Init()
    {

    }

    public virtual void Close()
    {

    }
    public virtual void ToState(State state, OnStateEndDelegate onStateEnd)
    {
        GetState = state;
    }
}
