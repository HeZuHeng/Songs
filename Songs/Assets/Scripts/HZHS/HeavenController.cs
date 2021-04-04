using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        sceneAsset.Tran.parent.localEulerAngles = Vector3.zero;
        sceneAsset.Tran.localEulerAngles = new Vector3(0, -65, 0);

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
        Tween tween = CameraMng.MainCamera.transform.DOMove(CameraMng.MainCamera.transform.position - CameraMng.MainCamera.transform.forward * 4, 1);
        tween.onComplete = delegate ()
        {
            if (onStateEnd != null) onStateEnd(GetState);
        };
    }

    void TalkCamera(OnStateEndDelegate onStateEnd)
    {
        MainPlayer.songResultInfo.FillAnswer(9, string.Empty, 1, AnswerType.Operating);

        TymZouyi(delegate () {
            HzHsWoshou(delegate () {
                if (onStateEnd != null) onStateEnd(GetState);
            });
        });
    }

    void TymZouyi(OnStateChangeDelegate onStateChange)
    {
        sceneAsset.Tran.position = new Vector3(sceneAsset.Tran.position.x, 35.93194f, sceneAsset.Tran.position.z);
        Tween tween = sceneAsset.Tran.DOMove(tym.Tran.position + tym.Tran.forward * 1, 2f);
        tween.onUpdate = delegate ()
        {
            sceneAsset.PlayAnimator("Forward", 1, 1, null);
        };
        tween.onComplete = delegate ()
        {
            sceneAsset.PlayAnimator("Forward", 0, 1, null);
            sceneAsset.PlayAnimator("zuoyi", true, 1, null);
            tym.PlayAnimator("zuoyi", true, 1, delegate (string a)
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
        Tween tween = sceneAsset.Tran.DOMove(hzhs.Tran.position + hzhs.Tran.forward * 0.85f, 2f);
        tween.onUpdate = delegate ()
        {
            sceneAsset.PlayAnimator("Forward", 1, 1, null);
        };
        tween.onComplete = delegate ()
        {
            sceneAsset.PlayAnimator("Forward", 0, 1, null);
            hzhs.PlayAnimator("woshou", true, 1, null);
            sceneAsset.PlayAnimator("woshou", true, 1, delegate (string a)
            {
                hzhs.PlayAnimator("woshou", false, 1, null);
                sceneAsset.PlayAnimator("woshou", false, 1, null);
                onStateChange?.Invoke();
            });
            SceneController.GetInstance().AddPlayAnimator(sceneAsset);
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

