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
    public static TerrainController TerrainController { get; set; }
    public bool InitScene = false;
    public bool InitSceneObject = false;

    SceneData CurSceneData;
    GameObject playCutsceneObj;
    GameObject cameraObj;
    GameObject newCameraObj;
    GameObject initTranObj;
    ChildController childController;
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

        if (HZHSStartController.Name.Equals(sceneData.name))
        {
            childController = new HZHSStartController();
        }

        if (ArtisticController.Name.Equals(sceneData.name))
        {
            childController = new ArtisticController();
        }
    }

    public void Close()
    {
        childController = null;
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
            if (assetObject != null) assetObject.Start();
        }

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
        if(progress >= 1)
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
    public virtual void ToState(State state, OnStateEndDelegate onStateEnd)
    {
        GetState = state;
    }
}

public class HeavenController : ChildController
{
    public static string Name = "中英诗歌精神比较";
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
    public static string Name = "华兹华斯书房三本诗歌";

    SceneAssetObject sceneAsset;
    SceneAssetObject hzhs;

    SceneAssetObject bool1;
    SceneAssetObject bool2;
    SceneAssetObject bool3;
    public override void Init()
    {
        base.Init();
        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        hzhs = SceneMng.GetInstance().GetSceneAssetObject(101);
        bool1 = SceneMng.GetInstance().GetSceneAssetObject(1002);
        bool2 = SceneMng.GetInstance().GetSceneAssetObject(1003);
        bool3 = SceneMng.GetInstance().GetSceneAssetObject(1004);
        if(hzhs != null) hzhs.Tran.gameObject.SetActive(false);
        sceneAsset.Tran.gameObject.AddComponent<TriggerEvent>().enterEvent.AddListener(EnterEvent);
        InputManager.GetInstance().AddClickEventListener(OnClickEvent);
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
        Transform newModelParent = SceneController.TerrainController.transform.Find("ALL_Model/NewModelParent");
        if (newModelParent != null)
        {
            newModelParent.gameObject.SetActive(false);
        }
        bool1.Tran.gameObject.SetActive(false);
        bool2.Tran.gameObject.SetActive(false);
        bool3.Tran.gameObject.SetActive(false);
        hzhs.Tran.gameObject.SetActive(true);
        float tw = 0;
        Tween t = DOTween.To(() => tw, x => tw = x, 20, 2);
        t.onUpdate += delegate ()
        {
            CameraMng.GetInstance().Twist.twistAngle = tw;
        };
        t.onComplete += delegate ()
        {
            CameraMng.GetInstance().Twist.twistAngle = 0;
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
            sceneAsset.Tran.localEulerAngles = new Vector3(0, 180 + y, 0);
            CameraMng.mainCameraParent.localEulerAngles = new Vector3(0, 180 + y, 0);
            onStateEnd?.Invoke(GetState);
        };
    }

    void TalkCamera(OnStateEndDelegate onStateEnd)
    {
        Transform newModelParent = SceneController.TerrainController.transform.Find("ALL_Model/NewModelParent");
        if (newModelParent != null)
        {
            newModelParent.gameObject.SetActive(true);
        }
        onStateEnd?.Invoke(GetState);
    }


    void EnterEvent(string name)
    {
        if (!name.Contains("Book")) return;
        string[] strs = name.Split('_');
        int id = 0;
        if(strs.Length > 1)int.TryParse(strs[1],out id);
        if (id == 0) return;
        //CameraMng.GetInstance().UserControl.State(false);
        //sceneAsset.PlayAnimator("shu",true,1,delegate(string a) {
        //    sceneAsset.PlayAnimator("shu", false, 1, null);
        //    SongsDataMng.GetInstance().SetNextTaskData(id);
        //    UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        //    UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);
        //    CameraMng.GetInstance().UserControl.State(true);
        //});
        //SceneController.GetInstance().AddPlayAnimator(sceneAsset);
        SongsDataMng.GetInstance().SetNextTaskData(id);
        UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);

    }

    void OnClickEvent(GameObject obj)
    {
        EnterEvent(obj.name);
    }
}
public class ThreePictureController : ChildController
{
    public static string Name = "草的意象数据";

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
        if (!name.Contains("Picture")) return;
        string[] strs = name.Split('_');
        int id = 0;
        if (strs.Length > 1) int.TryParse(strs[1], out id);
        if (id == 0) return;
        //CameraMng.GetInstance().UserControl.State(false);
        //sceneAsset.PlayAnimator("shu", true, 1, delegate (string a) {
        //    sceneAsset.PlayAnimator("shu", false, 1, null);
        //    SongsDataMng.GetInstance().SetNextTaskData(id);
        //    UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        //    UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);
        //    CameraMng.GetInstance().UserControl.State(true);
        //});
        //SceneController.GetInstance().AddPlayAnimator(sceneAsset);
        SongsDataMng.GetInstance().SetNextTaskData(id);
        UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);
    }

    void OnClickEvent(GameObject obj)
    {
        EnterEvent(obj.name);
    }
}