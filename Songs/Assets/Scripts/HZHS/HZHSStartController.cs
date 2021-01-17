using DG.Tweening;
using LaoZiCloudSDK.CameraHelper;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HZHSStartController : ChildController
{
    public static string Name = "华兹华斯书房预览";

    SceneAssetObject sceneAsset;
    public override void Init()
    {
        base.Init();
        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
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
        CameraMng.MainCamera.transform.position = CameraMng.MainCamera.transform.position - CameraMng.MainCamera.transform.forward * 0.5f;
        Tween tween = CameraMng.MainCamera.transform.DOMove(CameraMng.MainCamera.transform.position + CameraMng.MainCamera.transform.forward * 1, 4);
        tween.onComplete = delegate ()
        {
            if (onStateEnd != null) onStateEnd(GetState);
        };
    }

    void TalkCamera(OnStateEndDelegate onStateEnd)
    {
        Transform transform = SceneController.TerrainController.transform.Find("ALL_Model/门/Door_Open");
        if(transform != null)
        {
            transform.DOLocalRotate(new Vector3(0,90,0),2);
        }
        Tween tween = CameraMng.MainCamera.transform.DOMove(CameraMng.MainCamera.transform.position + CameraMng.MainCamera.transform.forward * 6, 10);
        tween.onComplete = delegate ()
        {
            transform.localEulerAngles = Vector3.zero;
            if (onStateEnd != null) onStateEnd(GetState);
        };
    }
}
