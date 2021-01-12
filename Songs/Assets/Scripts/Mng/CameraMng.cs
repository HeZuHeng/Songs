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
    public static Transform mainCameraParent = null;
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
    public Twist Twist { get; set; }
    public ThirdPersonUserControl  UserControl{ get; set; }
    public Cutscene PlayCutscene { get; set; }
    public void Start(Transform transform)
    {
        MainCamera = transform.GetComponentInChildren<Camera>();
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
            Twist = MainCamera.GetComponent<Twist>();
            if(Twist == null)
            {
                Twist = MainCamera.gameObject.AddComponent<Twist>();
            }
            if (MainCamera.gameObject.GetComponent<InputEvent>() == null)
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
        if (tran == null) return;
        int layer = LayerMask.NameToLayer("Player");
        Transform[] transforms = tran.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].gameObject.layer = layer;
        }
        CapsuleCollider capsuleCollider = tran.gameObject.GetComponent<CapsuleCollider>();
        if(capsuleCollider == null) capsuleCollider = tran.gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 1.6f;
        capsuleCollider.radius = 0.3f;
        capsuleCollider.center = new Vector3(0, 0.8f, 0);
        PhysicMaterial physicMaterial = new PhysicMaterial();
        physicMaterial.dynamicFriction = 0.2f;
        physicMaterial.staticFriction = 0.1f;
        physicMaterial.bounciness = 0.01f;
        physicMaterial.frictionCombine = PhysicMaterialCombine.Multiply;
        physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        capsuleCollider.material = physicMaterial;

        Rigidbody rigidbody = tran.gameObject.GetComponent<Rigidbody>();
        if(rigidbody == null)
        {
            rigidbody = tran.gameObject.AddComponent<Rigidbody>();
        }
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        ThirdPersonCharacter thirdPersonCharacter = tran.gameObject.GetComponent<ThirdPersonCharacter>(); 
        if(thirdPersonCharacter == null) thirdPersonCharacter= tran.gameObject.AddComponent<ThirdPersonCharacter>();
        UserControl = tran.gameObject.GetComponent<ThirdPersonUserControl>();
        if(UserControl == null) UserControl = tran.gameObject.AddComponent<ThirdPersonUserControl>();

        AStarRun aStarRun = tran.gameObject.GetComponent<AStarRun>();
        if (aStarRun == null)
        {
            aStarRun = tran.gameObject.AddComponent<AStarRun>();
        }
    }

    public void ResetMove()
    {
        if(UserControl != null)
        {
            UserControl.gameObject.SetActive(false);
            UserControl.transform.parent.position = Vector3.zero;
            UserControl.transform.position = Vector3.zero;
        }
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
        int layer = LayerMask.NameToLayer("Player");
        MainCamera.cullingMask |= ~(1 << layer);
        mainCameraParent.transform.position = MainCamera.transform.position + MainCamera.transform.forward * 3;
        mainCameraParent.transform.rotation = MainCamera.transform.rotation;

        MainCamera.transform.SetParent(mainCameraParent.transform);
        MainCamera.transform.localPosition = -Vector3.forward * 0.8f;
        MainCamera.transform.localRotation = Quaternion.Euler(new Vector3(5,0,0));
        UserControl.offset = new Vector3(0, 1.7f, 0.5f);
        UserControl.transform.position = InitPosition;
        UserControl.transform.eulerAngles = InitRotation;
        UserControl.gameObject.SetActive(true);
        UserControl.SetMainCamera(mainCameraParent.transform);
        AQUAS_Look _Look = MainCamera.gameObject.AddComponent<AQUAS_Look>();
        _Look.offset = new Vector3(0,0,-0.8f);
        _Look._isLocked = false;
        _Look.SetParent(mainCameraParent.transform);
    }

    public void SetPersonMove()
    {
        ResetCamera();
        int layer = LayerMask.NameToLayer("Player");
        MainCamera.cullingMask &= ~(1 << layer);
        MainCamera.transform.SetParent(mainCameraParent.transform);
        MainCamera.transform.localPosition = -Vector3.forward * 0.1f;
        MainCamera.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        UserControl.offset = new Vector3(0, 1.55f, 0f);
        UserControl.transform.position = InitPosition;
        UserControl.transform.eulerAngles = InitRotation;
        UserControl.gameObject.SetActive(true);
        UserControl.SetMainCamera(mainCameraParent.transform);
        AQUAS_Look _Look = MainCamera.gameObject.AddComponent<AQUAS_Look>();
        _Look.offset = new Vector3(0, 0, 0.01f);
        _Look._isLocked = true;
        _Look.SetParent(mainCameraParent.transform);

        mainCameraParent.localRotation = Quaternion.Euler(Vector3.up * 180);
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
    int index = 0;
    public void GetPhoto()
    {
        index++;
        Rect rect = new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height));
        RenderTexture rt = new RenderTexture((int)Screen.width, (int)rect.height, 24);
        MainCamera.targetTexture = rt;
        MainCamera.Render();
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
        MainCamera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);
        string filepath = "F:\\Desstop\\"+ index+".png";//路径请自拟，这里随便写的
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(filepath, bytes);
    }
}