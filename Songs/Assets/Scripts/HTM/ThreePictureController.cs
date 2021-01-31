using DG.Tweening;
using LaoZiCloudSDK.CameraHelper;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreePictureController : ChildController
{
    public static string Name = "草的意象数据";

    SceneAssetObject sceneAsset;
    SceneAssetObject htm;
    SceneAssetObject chuang;

    public override void Init()
    {
        base.Init();
        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        htm = SceneMng.GetInstance().GetSceneAssetObject(102);
        chuang = SceneMng.GetInstance().GetSceneAssetObject(103);

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
        }
    }
    void InitMoveCamera(OnStateEndDelegate onStateEnd)
    {
        Tweener moveTw = CameraMng.MainCamera.transform.DOMove(htm.Tran.position + htm.Tran.forward * 1.5f + htm.Tran.up * 2f, 10);

        moveTw.onUpdate += delegate ()
        {
            CameraMng.MainCamera.transform.LookAt(htm.Tran.position + htm.Tran.up * 1.65f);
        };
        moveTw.onComplete += delegate ()
        {
            onStateEnd?.Invoke(GetState);
        };
    }
    List<int> lookSongNum = new List<int>();
    void EnterEvent(string name)
    {
        Debug.Log(name);
        if (!name.Contains("picture")) return;
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

        if (!lookSongNum.Contains(id)) lookSongNum.Add(id);
        MainPlayer.songResultInfo.FillAnswer(13, string.Empty, lookSongNum.Count, AnswerType.Operating);

        UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);
    }

    void OnClickEvent(GameObject obj)
    {
        EnterEvent(obj.name);
    }
}