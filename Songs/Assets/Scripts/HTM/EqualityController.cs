using DG.Tweening;
using LaoZiCloudSDK.CameraHelper;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqualityController : ChildController
{
    public static string Name = "体现诗人平等数据";

    SceneAssetObject sceneAsset;
    //SceneAssetObject htm;
    SceneAssetObject chuang;

    SceneAssetObject zhengzhijia;
    SceneAssetObject junren;

    SceneAssetObject nm;

    SceneAssetObject cs;
    SceneAssetObject np;

    SceneAssetObject nf;
    SceneAssetObject nh;

    Tweener movechuang;
    public override void Init()
    {
        base.Init();

        sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
        //htm = SceneMng.GetInstance().GetSceneAssetObject(102);
        chuang = SceneMng.GetInstance().GetSceneAssetObject(103);

        zhengzhijia = SceneMng.GetInstance().GetSceneAssetObject(106);
        junren = SceneMng.GetInstance().GetSceneAssetObject(104);

        nm = SceneMng.GetInstance().GetSceneAssetObject(107);
        nm.Tran.position = new Vector3(108.648f, 22.53f, 665.031f);

        cs = SceneMng.GetInstance().GetSceneAssetObject(108);
        np = SceneMng.GetInstance().GetSceneAssetObject(109);

        nf = SceneMng.GetInstance().GetSceneAssetObject(110);
        nh = SceneMng.GetInstance().GetSceneAssetObject(111);

        //if (htm != null) htm.Tran.gameObject.SetActive(false);
        MainPlayer.songResultInfo.FillAnswer(18, string.Empty, 1, AnswerType.Operating);
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
        //zhengzhijia.Tran.gameObject.SetActive(true);

        //Vector3[] vector3s = new Vector3[2];
        //vector3s[0] = new Vector3(2.3f, -39.734f, 1);
        //vector3s[1] = new Vector3(2.823f, -39.734f, -1.17f);
        //Tweener moveTw = zhengzhijia.Tran.DOLocalPath(vector3s, 6, PathType.CatmullRom, PathMode.Ignore).SetLookAt(0);

        ////moveTw.SetLoops(-1, LoopType.Restart);
        //moveTw.onUpdate += delegate ()
        //{
        //    zhengzhijia.PlayAnimator("forward", 1, 1, null);
        //};
        //moveTw.onComplete += delegate ()
        //{
        //    zhengzhijia.PlayAnimator("forward", 0, 1, null);

        //    onStateEnd?.Invoke(GetState);
        //};
        onStateEnd?.Invoke(GetState);
    }

    void TalkCamera(OnStateEndDelegate onStateEnd)
    {
        //sceneAsset.Tran.parent.position = chuang.Tran.position;
        //sceneAsset.Tran.localPosition = Vector3.zero;
        //Vector3 offset = chuang.Tran.position - sceneAsset.Tran.parent.position;
        //sceneAsset.Tran.parent.position = chuang.Tran.position - offset;

        //Vector3[] vector3s = new Vector3[5];
        //vector3s[0] = new Vector3(228, 61, 539);
        //vector3s[1] = new Vector3(400, 61, 431);
        //vector3s[2] = new Vector3(608, 61, 395);
        //vector3s[3] = new Vector3(784, 61, 208);
        //vector3s[4] = new Vector3(940, 61, 57);
        //movechuang = chuang.Tran.DOLocalPath(vector3s, 300, PathType.CatmullRom, PathMode.Ignore).SetLookAt(0.0001f);
        //movechuang.SetLoops(-1, LoopType.Restart);

        //movechuang.onUpdate += delegate ()
        //{
        //    sceneAsset.Tran.parent.position = chuang.Tran.position - offset;
        //};

        //sceneAsset.Tran.gameObject.AddComponent<TriggerEvent>().enterEvent.AddListener(EnterEvent);
        InputManager.GetInstance().RemoveClickEventListener(OnClickEvent);
        InputManager.GetInstance().AddClickEventListener(OnClickEvent);

        onStateEnd?.Invoke(GetState);
    }

    void TalkCameraOne(OnStateEndDelegate onStateEnd)
    {
        onStateEnd?.Invoke(GetState);
    }

    void TalkCameraTwo(OnStateEndDelegate onStateEnd)
    {
        onStateEnd?.Invoke(GetState);
    }

    void TalkCameraThere(OnStateEndDelegate onStateEnd)
    {
        onStateEnd?.Invoke(GetState);
    }

    void TalkCameraFour(OnStateEndDelegate onStateEnd)
    {
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

    List<int> lookSongNum = new List<int>();
    void OnClickEvent(GameObject obj)
    {
        int id = 0;
        if(obj.name.Equals(zhengzhijia.Tran.gameObject.name) || obj.name.Equals(junren.Tran.gameObject.name))
        {
            id = 5;
            obj = zhengzhijia.Tran.gameObject;
        }
        if (obj.name.Equals(nm.Tran.gameObject.name))
        {
            id = 7;
            obj = nm.Tran.gameObject;
        }
        if (obj.name.Equals(cs.Tran.gameObject.name) || obj.name.Equals(np.Tran.gameObject.name))
        {
            id = 8;
            obj = cs.Tran.gameObject;
        }
        if (obj.name.Equals(nf.Tran.gameObject.name) || obj.name.Equals(nh.Tran.gameObject.name))
        {
            id = 9;
            obj = nf.Tran.gameObject;
        }
        if (id <= 0) return;
        movechuang.Pause();

        AStarRun aStarRun = sceneAsset.Tran.GetComponent<AStarRun>();

        float time = Vector3.Distance(sceneAsset.Tran.position, obj.transform.position + obj.transform.forward * 1.5f);

        Vector3[] points = aStarRun.AStarFindWay(sceneAsset.Tran.position, obj.transform.position + obj.transform.forward * 1.5f);
        if (points == null) return;
        for (int i = 0; i < points.Length; i++)
        {
            points[i].y = sceneAsset.Tran.position.y;
            if(i < points.Length - 1) Debug.DrawLine(points[i], points[i+1]);
            //points[i] = sceneAsset.Tran.parent.InverseTransformPoint(points[i]);
            //points[i].y = sceneAsset.Tran.localPosition.y;
        }
        Tweener moveTw = sceneAsset.Tran.DOPath(points, time, PathType.Linear, PathMode.Ignore);
        moveTw.onUpdate += delegate ()
        {
            AudioManager.Instance.PlayJiaobuSound(2, true);
            CameraMng.MainCamera.transform.LookAt(new Vector3(obj.transform.position.x, CameraMng.MainCamera.transform.position.y, obj.transform.position.z));
        };
        moveTw.onComplete += delegate ()
        {
            AudioManager.Instance.PlayJiaobuSound(2, false);
            sceneAsset.Tran.LookAt(new Vector3(obj.transform.position.x, sceneAsset.Tran.position.y, obj.transform.position.z));
            CameraMng.mainCameraParent.position = CameraMng.MainCamera.transform.position;
            CameraMng.mainCameraParent.eulerAngles = CameraMng.MainCamera.transform.eulerAngles;
            CameraMng.MainCamera.transform.localPosition = Vector3.zero;
            CameraMng.MainCamera.transform.localEulerAngles = Vector3.zero;
            SongsDataMng.GetInstance().SetNextTaskData(id);
            UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
            UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);

            if (!lookSongNum.Contains(id)) lookSongNum.Add(id);
            MainPlayer.songResultInfo.FillAnswer(19, string.Empty, lookSongNum.Count, AnswerType.Operating);

            movechuang.Play();
        };
        
    }
}