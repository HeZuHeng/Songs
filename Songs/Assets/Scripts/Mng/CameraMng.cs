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
    public void Start(Transform transform)
    {
        GameObject go = new GameObject("CameraParent");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        mainCameraParent = go.transform;
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
    }

    public void InitPlayer(Transform tran)
    {
        CapsuleCollider capsuleCollider = tran.gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 1.6f;
        capsuleCollider.radius = 0.25f;
        capsuleCollider.center = new Vector3(0, 0.8f, 0);
        Rigidbody rigidbody = tran.gameObject.GetComponent<Rigidbody>();
        if(rigidbody == null)
        {
            rigidbody = tran.gameObject.AddComponent<Rigidbody>();
        }
        rigidbody.constraints = RigidbodyConstraints.None;
        tran.gameObject.AddComponent<ThirdPersonCharacter>();
        UserControl = tran.gameObject.AddComponent<ThirdPersonUserControl>();
    }

    public void ResetMove()
    {
        if(UserControl != null) UserControl.gameObject.SetActive(false);
        PlayCutscene = null;
        if (MainCamera == null) return;

        ResetCamera();
    }

    public void DOTweenPaly(UnityAction action)
    {
        if (PlayCutscene != null)
        {
            PlayCutscene.Play(delegate()
            {
                PlayCutscene.gameObject.SetActive(false);
                if (action != null) action();
            });
            return;
        }
    }

    private void ResetCamera()
    {
        if (UserControl != null)
        {
            UserControl.transform.position = InitPosition;
            UserControl.transform.eulerAngles = InitRotation;
        }
        AQUAS_Look look = MainCamera.gameObject.GetComponent<AQUAS_Look>();
        if (look != null) GameObject.DestroyImmediate(look);
        AQUAS_Walk walk = MainCamera.gameObject.GetComponent<AQUAS_Walk>();
        if (walk != null) GameObject.DestroyImmediate(walk);
        CharacterController controller = MainCamera.gameObject.GetComponent<CharacterController>();
        if (walk != null) GameObject.DestroyImmediate(controller);
    }

    public void SetThirdPersonMove()
    {
        ResetCamera();

        mainCameraParent.transform.position = MainCamera.transform.position + MainCamera.transform.forward * 3;
        mainCameraParent.transform.rotation = MainCamera.transform.rotation;

        MainCamera.transform.SetParent(mainCameraParent.transform);
        MainCamera.transform.localPosition = -Vector3.forward * 3f + Vector3.up * 1.6f;
        MainCamera.transform.localRotation = Quaternion.identity;

        UserControl.transform.position = InitPosition;
        UserControl.transform.eulerAngles = InitRotation;
        UserControl.gameObject.SetActive(true);
        UserControl.SetMainCamera(mainCameraParent.transform);
        AQUAS_Look _Look = MainCamera.gameObject.AddComponent<AQUAS_Look>();
        _Look._isLocked = false;
        _Look.SetParent(mainCameraParent.transform);
    }

    public void SetGodRoamsMove()
    {
        ResetCamera();

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
        ResetCamera();

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