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
        if (hzhs != null) hzhs.Tran.gameObject.SetActive(false);
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

    List<int> lookSongNum = new List<int>();
    void EnterEvent(string name)
    {
        if (!name.Contains("book")) return;
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
        if (!lookSongNum.Contains(id)) lookSongNum.Add(id);
        MainPlayer.songResultInfo.FillAnswer(2, string.Empty, lookSongNum.Count, AnswerType.Operating);
        UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);

    }

    void OnClickEvent(GameObject obj)
    {
        EnterEvent(obj.name);
    }
}