using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTMStartController : ChildController
{
    public static string Name = "惠特曼介绍数据";

    SceneAssetObject sceneAsset;
    SceneAssetObject htm;
    SceneAssetObject chuang;

    public override void Init()
    {
        base.Init();

        CameraMng.GetInstance().SetGodRoamsMove();

        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        htm = SceneMng.GetInstance().GetSceneAssetObject(102);
        chuang = SceneMng.GetInstance().GetSceneAssetObject(103);

        sceneAsset.Tran.gameObject.SetActive(false);

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
        MainPlayer.songResultInfo.FillAnswer(10, string.Empty, 1, AnswerType.Operating);

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

    void TalkCamera(OnStateEndDelegate onStateEnd)
    {
        MainPlayer.songResultInfo.FillAnswer(12, string.Empty, 1, AnswerType.Operating);
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

        onStateEnd?.Invoke(GetState);
    }
}
