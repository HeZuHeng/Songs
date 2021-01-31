using DG.Tweening;
using LaoZiCloudSDK.CameraHelper;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtisticController : ChildController
{
    public static string Name = "华兹华斯书房意境";

    SceneAssetObject sceneAsset;
    SceneAssetObject hzhs;

    public override void Init()
    {
        base.Init();
        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        hzhs = SceneMng.GetInstance().GetSceneAssetObject(101);
        if(hzhs != null) hzhs.Tran.gameObject.SetActive(false);
        //sceneAsset.Tran.gameObject.AddComponent<TriggerEvent>().enterEvent.AddListener(EnterEvent);
        InputManager.GetInstance().AddClickEventListener(OnClickEvent);
    }

    public override void Close()
    {
        base.Close();
        InputManager.GetInstance().RemoveClickEventListener(OnClickEvent);
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
        UIMng.Instance.ActivationUI(UIType.MemoryWnd);
        Transform newModelParent = SceneController.TerrainController.transform.Find("ALL_Model/NewModelParent");
        if (newModelParent != null)
        {
            newModelParent.gameObject.SetActive(false);
        }
        
        hzhs.Tran.gameObject.SetActive(true);
        hzhs.PlayAnimator("sitloop", true, 1, null);

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
        sceneAsset.Tran.localEulerAngles = new Vector3(0, 180+y, 0);
        CameraMng.mainCameraParent.localEulerAngles = new Vector3(0, 180 + y, 0);
        onStateEnd?.Invoke(GetState);
        
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
            hzhs.Tran.gameObject.SetActive(false);
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

