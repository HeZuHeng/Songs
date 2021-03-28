using DG.Tweening;
using LaoZiCloudSDK.CameraHelper;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemocracyController : ChildController
{
    public static string Name = "中美民主思想比较数据";

    SceneAssetObject sceneAsset;
    SceneAssetObject htm;
    SceneAssetObject chuang;

    SceneAssetObject huachuan;
    SceneAssetObject xingshu;


    Tweener movechuang;
    Vector3[] vector3s;
    Vector3 offset;
    public override void Init()
    {
        base.Init();

        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        htm = SceneMng.GetInstance().GetSceneAssetObject(102);
        chuang = SceneMng.GetInstance().GetSceneAssetObject(103);
        huachuan = SceneMng.GetInstance().GetSceneAssetObject(203);
        xingshu = SceneMng.GetInstance().GetSceneAssetObject(204);
        if (htm != null) htm.Tran.gameObject.SetActive(true);

        vector3s = new Vector3[4];
        vector3s[0] = new Vector3(275, 31.26f, 592);
        vector3s[1] = new Vector3(405, 31.26f, 530);
        vector3s[2] = new Vector3(600, 31.26f, 408);
        vector3s[3] = new Vector3(850, 31.26f, 217);

        offset = sceneAsset.Tran.position;
        sceneAsset.Tran.parent.position = chuang.Tran.position;
        sceneAsset.Tran.localPosition = Vector3.zero;
        //movechuang = chuang.Tran.DOLocalPath(vector3s, 300, PathType.CatmullRom, PathMode.Ignore).SetLookAt(0.0001f);
        //movechuang.SetLoops(-1, LoopType.Restart);

        //Vector3 offset = chuang.Tran.position - sceneAsset.Tran.position;
        //movechuang.onUpdate += delegate ()
        //{
        //    sceneAsset.Tran.parent.position = chuang.Tran.position - offset;
        //};
    }

    public override void ToState(State state, OnStateEndDelegate onStateEnd)
    {
        base.ToState(state, onStateEnd);
        switch (state)
        {
            case State.InitMoveCamera:
                InitMoveCamera(onStateEnd);
                break;
            case State.TalkCameraError:
                TalkCameraError(onStateEnd);
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
        CameraMng.mainCameraParent.localEulerAngles = Vector3.up * 82;
        huachuan.Tran.gameObject.SetActive(false);
        xingshu.Tran.gameObject.SetActive(false);
        onStateEnd?.Invoke(GetState);
    }

    void TalkCameraError(OnStateEndDelegate onStateEnd)
    {
        Tweener tweener = CameraMng.MainCamera.transform.DOShakeRotation(0.5f, new Vector3(0.2f, 0.2f, 0));
        CameraMng.MainCamera.transform.DOShakePosition(0.5f, new Vector3(0.2f, 0.2f, 0));
        tweener.onComplete += delegate ()
        {
            onStateEnd?.Invoke(GetState);
        };
    }

    void TalkCamera(OnStateEndDelegate onStateEnd)
    {
        huachuan.Tran.gameObject.SetActive(true);
        xingshu.Tran.gameObject.SetActive(true);
        movechuang = chuang.Tran.DOMove(vector3s[0], 5);
        movechuang.onUpdate += delegate ()
        {
            sceneAsset.Tran.parent.position = chuang.Tran.position;
            //chuang.Tran.right = (chuang.Tran.position - vector3s[0]).normalized;
        };
        movechuang.onComplete += delegate ()
        {
            huachuan.Tran.gameObject.SetActive(false);
            xingshu.Tran.gameObject.SetActive(false);
            onStateEnd?.Invoke(GetState);
        };
    }

    void TalkCameraOne(OnStateEndDelegate onStateEnd)
    {
        huachuan.Tran.gameObject.SetActive(true);
        xingshu.Tran.gameObject.SetActive(true);
        movechuang = chuang.Tran.DOMove(vector3s[1], 5);
        movechuang.onUpdate += delegate ()
        {
            sceneAsset.Tran.parent.position = chuang.Tran.position;
            //chuang.Tran.right = (chuang.Tran.position - vector3s[1]).normalized;
        };
        movechuang.onComplete += delegate ()
        {
            huachuan.Tran.gameObject.SetActive(false);
            xingshu.Tran.gameObject.SetActive(false);
            onStateEnd?.Invoke(GetState);
        };
    }

    void TalkCameraTwo(OnStateEndDelegate onStateEnd)
    {
        huachuan.Tran.gameObject.SetActive(true);
        xingshu.Tran.gameObject.SetActive(true);
        movechuang = chuang.Tran.DOMove(vector3s[2], 5);
        movechuang.onUpdate += delegate ()
        {
            sceneAsset.Tran.parent.position = chuang.Tran.position;
            //chuang.Tran.right = (chuang.Tran.position - vector3s[2]).normalized;
        };
        movechuang.onComplete += delegate ()
        {
            huachuan.Tran.gameObject.SetActive(false);
            xingshu.Tran.gameObject.SetActive(false);
            onStateEnd?.Invoke(GetState);
        };
    }

    void TalkCameraThere(OnStateEndDelegate onStateEnd)
    {
        huachuan.Tran.gameObject.SetActive(true);
        xingshu.Tran.gameObject.SetActive(true);
        movechuang = chuang.Tran.DOMove(vector3s[3], 5);
        movechuang.onUpdate += delegate ()
        {
            sceneAsset.Tran.parent.position = chuang.Tran.position;
            //chuang.Tran.right = (chuang.Tran.position - vector3s[3]).normalized;
        };
        movechuang.onComplete += delegate ()
        {
            huachuan.Tran.gameObject.SetActive(false);
            xingshu.Tran.gameObject.SetActive(false);
            onStateEnd?.Invoke(GetState);
        };
    }

    void TalkCameraFour(OnStateEndDelegate onStateEnd)
    {
        onStateEnd?.Invoke(GetState);
    }

}