using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using DG.Tweening;
using AQUAS;
using UnityEngine.Events;
using Slate;
using LaoZiCloudSDK.CameraHelper;

public class CameraMng
{
    private static CameraMng _instance = null;
    public static CameraMng GetInstance()
    {
        if (null == _instance)
        {
            _instance = new CameraMng();
        }

        return _instance;
    }
    static Transform mainCameraParent = null;
    static Camera mainCamera = null;
    public static Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            return mainCamera;
        }
        private set
        {
            mainCamera = value;
        }
    }

    public static Vector3 InitPosition = Vector3.zero;
    public static Vector3 InitRotation = Vector3.zero;
    public ThirdPersonUserControl  UserControl{ get; set; }
    public Cutscene PlayCutscene { get; set; }
    public void Init(Transform transform)
    {
        mainCameraParent = transform;
    }

    public void InitScene(Camera camera)
    {
        if (camera != null)
        {
            camera.tag = "MainCamera";
            MainCamera = camera;
            MainCamera.transform.SetParent(mainCameraParent);
            if (MainCamera.gameObject.GetComponent<InputEvent>())
            {
                MainCamera.gameObject.AddComponent<InputEvent>();
            }
            ResetMove();
        }
    }
    public void InitScene(Cutscene playCutscene)
    {
        PlayCutscene = playCutscene;
        PlayCutscene.transform.SetParent(mainCameraParent);
    }

    public void InitScene(Vector3 pos, Vector3 rota)
    {
        InitPosition = pos;
        InitRotation = rota;
        ResetMove();
    }

    public void InitPlayer(Transform tran)
    {
        CapsuleCollider capsuleCollider = tran.gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 1.6f;
        capsuleCollider.radius = 0.25f;
        capsuleCollider.center = new Vector3(0, 0.8f, 0);
        Rigidbody rigidbody = tran.gameObject.AddComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.None;
        tran.gameObject.AddComponent<ThirdPersonCharacter>();
        UserControl = tran.gameObject.AddComponent<ThirdPersonUserControl>();
    }

    public void ResetMove()
    {
        UserControl.gameObject.SetActive(false);
        if (MainCamera == null) return;
        UserControl.transform.position = InitPosition;
        UserControl.transform.eulerAngles = InitRotation;
        AQUAS_Look look = MainCamera.gameObject.GetComponent<AQUAS_Look>();
        if(look != null) GameObject.DestroyImmediate(look);
        AQUAS_Walk walk = MainCamera.gameObject.GetComponent<AQUAS_Walk>();
        if (walk != null) GameObject.DestroyImmediate(walk);
        CharacterController controller = MainCamera.gameObject.GetComponent<CharacterController>();
        if (walk != null) GameObject.DestroyImmediate(controller);
    }

    public void DOTweenPaly(UnityAction action)
    {
        if(PlayCutscene != null)
        {
            PlayCutscene.Play(delegate()
            {
                PlayCutscene.gameObject.SetActive(false);
                if (action != null) action();
            });
            return;
        }

        ResetMove();
        DOTweenPath tweenPath = MainCamera.GetComponent<DOTweenPath>();
        if (tweenPath != null)
        {
            if (tweenPath.onComplete == null) tweenPath.onComplete = new UnityEngine.Events.UnityEvent();
            tweenPath.onComplete.AddListener(action);
            tweenPath.DOPlay();
        }
        else
        {
           if(action != null) action();
        }
    }

    public void SetThirdPersonMove()
    {
        GameObject go = new GameObject("Parent");
        go.transform.position = MainCamera.transform.position + MainCamera.transform.forward * 3;
        go.transform.rotation = MainCamera.transform.rotation;

        MainCamera.transform.SetParent(go.transform);
        MainCamera.transform.localPosition = -Vector3.forward * 3f + Vector3.up * 1.6f;
        MainCamera.transform.localRotation = Quaternion.identity;

        UserControl.transform.position = InitPosition;
        UserControl.transform.eulerAngles = InitRotation;
        UserControl.gameObject.SetActive(true);
        UserControl.SetMainCamera(go.transform);
        AQUAS_Look _Look = MainCamera.gameObject.AddComponent<AQUAS_Look>();
        _Look._isLocked = false;
        _Look.SetParent(go.transform);
    }

    public void SetGodRoamsMove()
    {
        UserControl.transform.position = InitPosition;
        UserControl.transform.eulerAngles = InitRotation;
        UserControl.gameObject.SetActive(true);
        UserControl.SetMainCamera(MainCamera.transform, true);
        AQUAS_Look _Look = MainCamera.gameObject.AddComponent<AQUAS_Look>();
        _Look._isLocked = true;
        _Look.SetParent(null);
    }

    public void SetFirstPersonMove()
    {
        UserControl.transform.position = InitPosition;
        UserControl.transform.eulerAngles = InitRotation;
        UserControl.gameObject.SetActive(false);
        MainCamera.gameObject.AddComponent<CharacterController>();
        AQUAS_Look _Look = MainCamera.gameObject.AddComponent<AQUAS_Look>();
        _Look._isLocked = true;
        _Look.SetParent(null);
        MainCamera.gameObject.AddComponent<AQUAS_Walk>();

    }
}