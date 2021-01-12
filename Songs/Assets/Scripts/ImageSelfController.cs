using DG.Tweening;
using LaoZiCloudSDK.CameraHelper;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSelfController : ChildController
{
    public static string Name = "寻找草的意象数据";

    SceneAssetObject sceneAsset;
    SceneAssetObject htm;
    SceneAssetObject chuang;

    public override void Init()
    {
        base.Init();
        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        htm = SceneMng.GetInstance().GetSceneAssetObject(102);
        chuang = SceneMng.GetInstance().GetSceneAssetObject(103);
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
        moveTw.onUpdate += delegate ()
        {
            sceneAsset.Tran.parent.position = chuang.Tran.position - offset;
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
        }
    }

    void InitMoveCamera(OnStateEndDelegate onStateEnd)
    {
        htm.Tran.gameObject.SetActive(true);

        Vector3[] vector3s = new Vector3[5];
        vector3s[0] = new Vector3(6, -39.734f, 5);
        vector3s[1] = new Vector3(4, -39.734f, 4);
        vector3s[2] = new Vector3(2.3f, -39.734f, 2.2f);
        vector3s[3] = new Vector3(2.3f, -39.734f, 1);
        vector3s[4] = new Vector3(4f, -39.734f, -1.5f);
        Tweener moveTw = htm.Tran.DOLocalPath(vector3s, 40, PathType.CatmullRom, PathMode.Ignore).SetLookAt(1);
        //moveTw.SetLoops(-1, LoopType.Restart);
        moveTw.onUpdate += delegate ()
        {
            CameraMng.MainCamera.transform.position = htm.Tran.position - htm.Tran.forward * 0.65f + htm.Tran.up * 1.65f;
            htm.PlayAnimator("forward", 1, 1, null);
        };
        moveTw.onComplete += delegate ()
        {
            htm.PlayAnimator("forward", 0, 1, null);
            
            CameraMng.GetInstance().SetPersonMove();
            sceneAsset.Tran.position = htm.Tran.position - htm.Tran.forward * 2f;
            sceneAsset.Tran.LookAt(htm.Tran);
            CameraMng.MainCamera.transform.localEulerAngles = Vector3.zero;

            Vector3[] sikao = new Vector3[2];
            sikao[0] = new Vector3(4f, -39.734f, -1.5f);
            sikao[1] = new Vector3(4f, -39.734f, -0.5f);
            Tweener sikaoTw = htm.Tran.DOLocalPath(sikao, 3, PathType.Linear, PathMode.Ignore).SetLookAt(0.01f);
            sikaoTw.SetLoops(-1,LoopType.Yoyo);

            onStateEnd?.Invoke(GetState);
        };

        Tween tween = htm.Tran.DOLocalRotate(135 * Vector3.up, 30);
        tween.onUpdate += delegate ()
        {
            CameraMng.MainCamera.transform.eulerAngles = htm.Tran.eulerAngles + Vector3.right * 5;
        };
    }

    void TalkCamera(OnStateEndDelegate onStateEnd)
    {
        Transform newModelParent = SceneController.TerrainController.transform.Find("ALL_Model/NewModelParent");
        if (newModelParent != null)
        {
            newModelParent.gameObject.SetActive(true);
        }
        float tw = 0;
        Tween t = DOTween.To(() => tw, x => tw = x, 20, 2);
        t.onUpdate += delegate ()
        {
            CameraMng.GetInstance().Twist.twistAngle = tw;
        };
        t.onComplete += delegate ()
        {
            CameraMng.GetInstance().Twist.twistAngle = 0;
            htm.Tran.gameObject.SetActive(false);
            onStateEnd?.Invoke(GetState);
        };
    }


    void EnterEvent(string name)
    {
        if (!name.Contains("Book")) return;
        string[] strs = name.Split('_');
        int id = 0;
        if (strs.Length > 1) int.TryParse(strs[1], out id);
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

