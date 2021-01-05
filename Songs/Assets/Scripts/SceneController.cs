using MREngine;
using Slate;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LaoZiCloudSDK.CameraHelper;

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
    public static TerrainController TerrainController { get; private set; }
    public bool InitScene = false;

    SceneData CurSceneData;
    GameObject playCutsceneObj;
    GameObject cameraObj;
    GameObject initTranObj;
    ChildController childController;
    List<SceneAssetObject> palyAnimators = new List<SceneAssetObject>();

    public void Init()
    {
        Close();
        InitScene = false;
        SceneData sceneData = SongsDataMng.GetInstance().GetSceneData;
        CurSceneData = sceneData;
        if (sceneData == null) return;

        SceneMng.GetInstance().OnSceneLoadProgress += OnSceneLoaded;

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

        SceneAssetObject assetObject = SceneMng.GetInstance().AddSpaceAsset(1, "nvyk", "女游客");

        ModelData modelData = null;
        for (int i = 0; i < CurSceneData.datas.Count; i++)
        {
            modelData = CurSceneData.datas[i];
            SceneMng.GetInstance().AddSpaceAsset(modelData);
        }

        if (HeavenController.Name.Equals(sceneData.name))
        {
            childController = new HeavenController();
        }

        if (ThreeSongsController.Name.Equals(sceneData.name))
        {
            childController = new ThreeSongsController();
        }

        if(ThreePictureController.Name.Equals(sceneData.name))
        {
            childController = new ThreePictureController();
        }
    }

    public void Close()
    {
        childController = null;
        UIMng.Instance.ConcealUI(UIType.LeftDialogueWnd);
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

    public void Start(TerrainController controller = null)
    {
        TerrainController = controller;
        SceneAssetObject sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        CameraMng.GetInstance().InitPlayer(sceneAsset.Tran);
        sceneAsset.Tran.gameObject.SetActive(false);

        if(childController != null) childController.Init();
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
        if(childController != null) childController.ToState(state, onStateEnd);
    }

    public void FrameUpdate()
    {
        if (!InitScene) return;
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
        if(progress >= 1)
        {
            SceneMng.GetInstance().OnSceneLoadProgress -= OnSceneLoaded;
            InitScene = true;
        }
    }
}

public class ChildController
{
    public State GetState { get; set; }

    public virtual void Init()
    {

    }
    public virtual void ToState(State state, OnStateEndDelegate onStateEnd)
    {
        GetState = state;
    }
}

public class HeavenController : ChildController
{
    public static string Name = "Men and Nature";
    SceneAssetObject sceneAsset;
    SceneAssetObject hzhs;
    SceneAssetObject tym;
    public override void Init()
    {
        base.Init();

        CameraMng.GetInstance().SetGodRoamsMove();

        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
         hzhs = SceneMng.GetInstance().GetSceneAssetObject(101);
         tym = SceneMng.GetInstance().GetSceneAssetObject(102);

    }

    public override void ToState(State state, OnStateEndDelegate onStateEnd)
    {
        base.ToState(state, onStateEnd);
        switch (state)
        {
            case State.InitMoveCamera:
                InitMoveCamera(onStateEnd);
                break;
            case State.TalkCamera:
                TalkCamera(onStateEnd);
                break;
        }
    }

    void InitMoveCamera(OnStateEndDelegate onStateEnd)
    {
        Tween tween = CameraMng.MainCamera.transform.DOMove(CameraMng.MainCamera.transform.position - CameraMng.MainCamera.transform.forward * 4,1);
        tween.onComplete = delegate ()
        {
            if (onStateEnd != null) onStateEnd(GetState);
        };
    }

    void TalkCamera(OnStateEndDelegate onStateEnd)
    {
        TymZouyi(delegate () {
            HzHsWoshou(delegate() {
                if (onStateEnd != null) onStateEnd(GetState);
            });
        });
    }

    void TymZouyi(OnStateChangeDelegate onStateChange)
    {
        sceneAsset.Tran.position = new Vector3(sceneAsset.Tran.position.x, 34.9f, sceneAsset.Tran.position.z);
        Tween tween = sceneAsset.Tran.DOMove(tym.Tran.position + tym.Tran.forward * 1, 2f);
        tween.onUpdate = delegate ()
        {
            sceneAsset.PlayAnimator("Forward", 1, 1, null);
        };
        tween.onComplete = delegate ()
        {
            sceneAsset.PlayAnimator("Forward", 0, 1, null);
            sceneAsset.PlayAnimator("zuoyi", true, 1, null);
            tym.PlayAnimator("zuoyi", true, 1, delegate(string a)
            {
                tym.PlayAnimator("zuoyi", false, 1, null);
                sceneAsset.PlayAnimator("zuoyi", false, 1, null);
                onStateChange?.Invoke();
            });
            SceneController.GetInstance().AddPlayAnimator(tym);
        };
        float y = Vector3.Angle(sceneAsset.Tran.forward, tym.Tran.forward);
        //Debug.Log(y);
        sceneAsset.Tran.DORotate(new Vector3(0, y, 0), 2f, RotateMode.LocalAxisAdd);
        //Debug.Log(Quaternion.Euler(-tym.Tran.forward).eulerAngles);
        //sceneAsset.Tran.DORotateQuaternion(Quaternion.Euler(-tym.Tran.forward), 1);

    }

    void HzHsWoshou(OnStateChangeDelegate onStateChange)
    {
        Tween tween = sceneAsset.Tran.DOMove(hzhs.Tran.position + hzhs.Tran.forward * 1, 2f);
        tween.onUpdate = delegate ()
        {
            sceneAsset.PlayAnimator("Forward", 1, 1, null);
        };
        tween.onComplete = delegate ()
        {
            sceneAsset.PlayAnimator("Forward", 0, 1, null);
            sceneAsset.PlayAnimator("woshou", true, 1, null);
            hzhs.PlayAnimator("woshou", true, 1, delegate (string a)
            {
                hzhs.PlayAnimator("woshou", false, 1, null);
                sceneAsset.PlayAnimator("woshou", false, 1, null);
                onStateChange?.Invoke();
            });
            SceneController.GetInstance().AddPlayAnimator(hzhs);
        };
        Vector3 dir = hzhs.Tran.position - sceneAsset.Tran.position; //位置差，方向     
        float v = Vector3.Dot(sceneAsset.Tran.forward, dir);
        float y = 0;
        if (v > 0)
        {
            y = Vector3.Angle(sceneAsset.Tran.forward, hzhs.Tran.forward);
        }
        else
        {
            y = Vector3.Angle(sceneAsset.Tran.forward, hzhs.Tran.forward) - 180;
        }
        //Debug.Log(y);
        sceneAsset.Tran.DORotate(new Vector3(0, y, 0), 2f, RotateMode.LocalAxisAdd);
    }
}

public class ThreeSongsController : ChildController
{
    public static string Name = "Three Books";

    SceneAssetObject sceneAsset;
    public override void Init()
    {
        base.Init();
        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        sceneAsset.Tran.gameObject.AddComponent<TriggerEvent>().enterEvent.AddListener(EnterEvent);
        InputManager.GetInstance().AddClickEventListener(OnClickEvent);
    }

    void EnterEvent(string name)
    {
        Debug.Log(name);
        string[] strs = name.Split('_');
        int id = 0;
        if(strs.Length > 1)int.TryParse(strs[1],out id);
        CameraMng.GetInstance().UserControl.State(false);
        sceneAsset.PlayAnimator("shu",true,1,delegate(string a) {
            sceneAsset.PlayAnimator("shu", false, 1, null);
            SongsDataMng.GetInstance().SetNextTaskData(id);
            UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
            UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);
            CameraMng.GetInstance().UserControl.State(true);
        });
        SceneController.GetInstance().AddPlayAnimator(sceneAsset);
        
    }

    void OnClickEvent(GameObject obj)
    {
        EnterEvent(obj.name);
    }
}
public class ThreePictureController : ChildController
{
    public static string Name = "The Image of Grass";

    SceneAssetObject sceneAsset;
    public override void Init()
    {
        base.Init();
        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        sceneAsset.Tran.gameObject.AddComponent<TriggerEvent>().enterEvent.AddListener(EnterEvent);
    }

    void EnterEvent(string name)
    {
        Debug.Log(name);
        string[] strs = name.Split('_');
        int id = 0;
        if (strs.Length > 1) int.TryParse(strs[1], out id);
        CameraMng.GetInstance().UserControl.State(false);
        sceneAsset.PlayAnimator("shu", true, 1, delegate (string a) {
            sceneAsset.PlayAnimator("shu", false, 1, null);
            SongsDataMng.GetInstance().SetNextTaskData(id);
            UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
            UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);
            CameraMng.GetInstance().UserControl.State(true);
        });
        SceneController.GetInstance().AddPlayAnimator(sceneAsset);

    }
}