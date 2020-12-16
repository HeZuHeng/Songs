using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class SettingWnd : UIBase
{
    public RectTransform openParent;
    public RectTransform hlepParent;
    public RectTransform purposeParent;

    public Toggle help;
    public Toggle purpose;
    public Toggle story;
    public Toggle task;
    public Toggle open;
    public Toggle music;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.SettingWnd;
        MutexInterface = false;

        help.onValueChanged.AddListener(OnHelp);
        purpose.onValueChanged.AddListener(OnPurpose);
        story.onValueChanged.AddListener(OnStory);
        task.onValueChanged.AddListener(OnTask);
        open.onValueChanged.AddListener(OnOpen);
        music.onValueChanged.AddListener(OnMusic);
    }

    protected override void Start()
    {
        base.Start();
        music.isOn = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        help.isOn = true;
        purpose.isOn = false;
        story.isOn = false;
        task.isOn = true;
        open.isOn = false;
        CancelInvoke("HideHelp");
        Invoke("HideHelp", 2);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CancelInvoke("HideHelp");
    }

    void HideHelp()
    {
        help.isOn = false;
    }

    void OnHelp(bool val)
    {
        hlepParent.gameObject.SetActive(val);
    }
    void OnPurpose(bool val)
    {
        purposeParent.gameObject.SetActive(val);
    }

    void OnStory(bool val)
    {
        if (val)
        {
            UIMng.Instance.OpenUI(UIType.SelectPlotWnd);
        }
        else
        {
            UIMng.Instance.OpenUI(UIType.NONE);
        }
    }

    void OnTask(bool val)
    {
        if (val)
        {
            UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);
        }
        else
        {
            if(SongsDataMng.GetInstance().GetTaskData.TaskState == TaskState.Start) UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        }
    }

    void OnOpen(bool val)
    {
        openParent.gameObject.SetActive(val);
    }

    void OnMusic(bool val)
    {
        AudioSource  audioSource = CameraMng.MainCamera.gameObject.GetComponent<AudioSource>();
        if(audioSource != null)
        {
            audioSource.enabled = val;
        }
    }

}
