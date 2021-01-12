using DG.Tweening;
using LaoZiCloudSDK.CameraHelper;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverSongController : ChildController
{
    public static string Name = "自我之歌数据";

    SceneAssetObject sceneAsset;
    SceneAssetObject htm;
    SceneAssetObject chuang;
    SceneAssetObject gentle;
    SceneAssetObject shower;
    SceneAssetObject baofeng;

    public override void Init()
    {
        base.Init();

        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        htm = SceneMng.GetInstance().GetSceneAssetObject(102);
        chuang = SceneMng.GetInstance().GetSceneAssetObject(103);
        gentle = SceneMng.GetInstance().GetSceneAssetObject(106);
        shower = SceneMng.GetInstance().GetSceneAssetObject(201);
        baofeng = SceneMng.GetInstance().GetSceneAssetObject(202);

        baofeng.Tran.gameObject.SetActive(false);
        if (htm != null) htm.Tran.gameObject.SetActive(false);
        sceneAsset.Tran.gameObject.AddComponent<TriggerEvent>().enterEvent.AddListener(EnterEvent);
        InputManager.GetInstance().AddClickEventListener(OnClickEvent);

        Vector3[] vector3s = new Vector3[5];
        vector3s[0] = new Vector3(228, 61, 539);
        vector3s[1] = new Vector3(400, 61, 431);
        vector3s[2] = new Vector3(608, 61, 395);
        vector3s[3] = new Vector3(784, 61, 208);
        vector3s[4] = new Vector3(940, 61, 57);
        Tweener moveTw = chuang.Tran.DOLocalPath(vector3s, 300, PathType.CatmullRom, PathMode.Ignore).SetLookAt(0.0001f);
        moveTw.SetLoops(-1, LoopType.Restart);
        Vector3 offset = chuang.Tran.position - sceneAsset.Tran.position;

        CameraMng.mainCameraParent.position = CameraMng.MainCamera.transform.position;
        CameraMng.mainCameraParent.eulerAngles = CameraMng.MainCamera.transform.eulerAngles;
        CameraMng.MainCamera.transform.localPosition = Vector3.zero;
        CameraMng.MainCamera.transform.localEulerAngles = Vector3.zero;

        Vector3 coffset = chuang.Tran.position - CameraMng.mainCameraParent.position;
        moveTw.onUpdate += delegate ()
        {
            sceneAsset.Tran.parent.position = chuang.Tran.position - offset;
            CameraMng.mainCameraParent.position = chuang.Tran.position - coffset;
        };
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
            case State.TalkCameraOne:
                TalkCameraOne(onStateEnd);
                break;
            case State.TalkCameraTwo:
                TalkCameraTwo(onStateEnd);
                break;
            case State.TalkCameraThere:
                TalkCameraThere(onStateEnd);
                break;
            case State.TalkCameraFour:
                TalkCameraFour(onStateEnd);
                break;
        }
    }

    void InitMoveCamera(OnStateEndDelegate onStateEnd)
    {
        htm.Tran.localPosition = new Vector3(3.202f,-39.734f,-1.934f);
        htm.Tran.localEulerAngles = Vector3.zero;

        htm.Tran.gameObject.SetActive(true);
        gentle.Tran.gameObject.SetActive(true);

        Vector3[] vector3s = new Vector3[2];
        vector3s[0] = new Vector3(2.3f, -39.734f, 1);
        vector3s[1] = new Vector3(2.823f, -39.734f, -1.17f);
        Tweener moveTw = gentle.Tran.DOLocalPath(vector3s, 6, PathType.CatmullRom, PathMode.Ignore).SetLookAt(0);

        //moveTw.SetLoops(-1, LoopType.Restart);
        moveTw.onUpdate += delegate ()
        {
            gentle.PlayAnimator("forward", 1, 1, null);
        };
        moveTw.onComplete += delegate ()
        {
            gentle.PlayAnimator("forward", 0, 1, null);

            onStateEnd?.Invoke(GetState);
        };
    }
    Tweener tweener;
    Tweener tweener1;
    void TalkCamera(OnStateEndDelegate onStateEnd)
    {
        tweener = CameraMng.MainCamera.transform.DOShakeRotation(20, new Vector3(0.2f, 0.2f, 0));
        tweener.SetLoops(-1,LoopType.Restart);
        tweener1 = CameraMng.MainCamera.transform.DOShakePosition(20, new Vector3(0.2f, 0.2f, 0));
        tweener1.SetLoops(-1, LoopType.Restart);

        shower.Tran.gameObject.SetActive(true);
        baofeng.Tran.gameObject.SetActive(true);

        onStateEnd?.Invoke(GetState);
    }

    void TalkCameraOne(OnStateEndDelegate onStateEnd)
    {
        //shower.Tran.gameObject.SetActive(false);
        baofeng.Tran.gameObject.SetActive(false);

        onStateEnd?.Invoke(GetState);
    }

    void TalkCameraTwo(OnStateEndDelegate onStateEnd)
    {
        if (tweener != null) tweener.CompletedLoops();
        if (tweener1 != null) tweener1.CompletedLoops();
        onStateEnd?.Invoke(GetState);
    }

    void TalkCameraThere(OnStateEndDelegate onStateEnd)
    {
        shower.Tran.gameObject.SetActive(false);
        htm.Tran.LookAt(new Vector3(CameraMng.MainCamera.transform.position.x,htm.Tran.position.y, CameraMng.MainCamera.transform.position.z));
        //baofeng.Tran.gameObject.SetActive(true);

        onStateEnd?.Invoke(GetState);
    }

    void TalkCameraFour(OnStateEndDelegate onStateEnd)
    {
        htm.Tran.LookAt(new Vector3(CameraMng.MainCamera.transform.position.x, htm.Tran.position.y, CameraMng.MainCamera.transform.position.z));
        //shower.Tran.gameObject.SetActive(false);
        //baofeng.Tran.gameObject.SetActive(true);

        onStateEnd?.Invoke(GetState);
    }

    void EnterEvent(string name)
    {
        if (!name.Contains("Book")) return;
        string[] strs = name.Split('_');
        int id = 0;
        if (strs.Length > 1) int.TryParse(strs[1], out id);
        if (id == 0) return;
        SongsDataMng.GetInstance().SetNextTaskData(id);
        UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);

    }

    void OnClickEvent(GameObject obj)
    {
        EnterEvent(obj.name);
    }
}