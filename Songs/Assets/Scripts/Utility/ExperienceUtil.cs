using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExperienceUtil : MonoBehaviour
{
    public static ExperienceUtil Instance;
    public Camera mCamera;
    public Transform plane;
    public RenderTexture Render;
    private Material material;

    Vector3 initPosition;

    private void Awake()
    {
        Instance = this;
        initPosition = mCamera.transform.localPosition;
        material = plane.GetComponent<MeshRenderer>().sharedMaterial;
    }
    public void InitPosition()
    {
        mCamera.enabled = true;
        mCamera.transform.localPosition = initPosition;
    }
    public void SetTexture(Texture texture)
    {
        material.SetTexture("_MainTex", texture);
    }
    public void DoMoveCamera(Texture texture, Vector3 pos,float time, TweenCallback callback,bool init = true)
    {
        material.SetTexture("_MainTex", texture);
        if(init) mCamera.transform.localPosition = initPosition;
        Tween tween = mCamera.transform.DOLocalMove(pos, time);
        tween.onComplete += callback;
    }

    public void Close()
    {
        mCamera.enabled = false;
    }
}
