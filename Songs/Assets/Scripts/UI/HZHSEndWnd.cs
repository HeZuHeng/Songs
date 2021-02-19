using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HZHSEndWnd : UIBase
{
    public Texture texture;
    public Image image;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HZHSEndWnd;
        MutexInterface = true;

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ExperienceUtil.Instance.InitPosition();
        ExperienceUtil.Instance.SetTexture(texture);
        image.enabled = false;
        Invoke("DoMove", 1);
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ExperienceUtil.Instance.InitPosition();
    }

    void DoMove()
    {
        ExperienceUtil.Instance.DoMoveCamera(texture, new Vector3(1.08f, -1.75f, -4), 5f, delegate () {
            ExperienceUtil.Instance.DoMoveCamera(texture, new Vector3(2.77f, 0.4f, -4), 5f, delegate () {
                image.enabled = true;
            },false);
        });
    }

    public void OnEnd()
    {
        SongsDataMng.GetInstance().SetTaskConfigData(0);
        UIMng.Instance.ActivationUI(UIType.SelectPlotWnd);
    }
}
