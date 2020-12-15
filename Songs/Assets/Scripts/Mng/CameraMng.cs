using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using DG.Tweening;
using AQUAS;

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
    public ThirdPersonUserControl  UserControl{ get; set; }

    /// <summary>
    /// 场景小地图拍照
    /// </summary>
    /// <param name="position">相机位置</param>
    /// <param name="size">大小</param>
    /// <param name="onfinish">完成事件</param>
    public void InitScene(Camera camera)
    {
        if(camera != null)
        {
            camera.tag = "MainCamera";
            mainCamera = camera;
            DOTweenPath tweenPath = camera.GetComponent<DOTweenPath>();
            if(tweenPath != null)
            {
                tweenPath.onComplete = new UnityEngine.Events.UnityEvent();
                tweenPath.onComplete.AddListener(SetUserControlTran);
            }
            else
            {
                SetUserControlTran();
            }
        }
    }

    public void SetUserControlTran()
    {
        GameObject go = new GameObject("Parent");
        go.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 3;
        go.transform.rotation = mainCamera.transform.rotation;
        UserControl.transform.position = go.transform.position + Vector3.up;

        mainCamera.transform.SetParent(go.transform);
        mainCamera.transform.localPosition = -Vector3.forward * 3f + Vector3.up * 1.6f;
        mainCamera.transform.localRotation = Quaternion.identity;

        UserControl.SetMainCamera(go.transform);
        UserControl.gameObject.SetActive(true);
        mainCamera.gameObject.AddComponent<AQUAS_Look>();
    }
}