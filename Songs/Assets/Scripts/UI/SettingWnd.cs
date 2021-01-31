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

    public Toggle chinese;

    public Button btn;

    public Transform aParent;
    public Text minute;
    public Text summary;

    public RectTransform musicCheck;
    public RectTransform chineseCheck;

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
        chinese.onValueChanged.AddListener(OnChinese);
        btn.onClick.AddListener(OnBtn);
    }

    protected override void Start()
    {
        base.Start();
        music.isOn = true;
        task.isOn = true;

        UIMng.Instance.ActivationUI(UIType.MainDialogueWnd);
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
        aParent.gameObject.SetActive(false);
        transform.SetSiblingIndex(100);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        task.isOn = false;
        CancelInvoke("HideHelp");
        CancelInvoke("ShowEnglish");
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
            UIMng.Instance.ActivationUI(UIType.SelectPlotWnd);
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
            UIMng.Instance.ConcealUI(UIType.MainDialogueWnd);
        }
    }

    void OnOpen(bool val)
    {
        openParent.gameObject.SetActive(val);
    }

    void OnMusic(bool val)
    {
        musicCheck.gameObject.SetActive(!val);
        SceneController.TerrainController.SetAllAudio(val);
    }

    void OnChinese(bool val)
    {
        SongsDataMng.GetInstance().orEnglishChange?.Invoke(val);
        chineseCheck.gameObject.SetActive(val);
        if (val)
        {
            CancelInvoke("ShowEnglish");
            Invoke("ShowEnglish", 10);
        }
    }

    void ShowEnglish()
    {
        chinese.isOn = false;
    }

    void OnBtn()
    {
        if (aParent.gameObject.activeSelf)
        {
            aParent.gameObject.SetActive(false);
            return;
        }
        aParent.gameObject.SetActive(true);

        int m = 0;
        for (int i = 0; i < MainPlayer.songResultInfo.singleChoices.Count; i++)
        {
            m += MainPlayer.songResultInfo.singleChoices[i].minute;
        }
        for (int i = 0; i < MainPlayer.songResultInfo.multipleChoices.Count; i++)
        {
            m += MainPlayer.songResultInfo.multipleChoices[i].minute;
        }
        for (int i = 0; i < MainPlayer.songResultInfo.fillInTheBlanks.Count; i++)
        {
            m += MainPlayer.songResultInfo.fillInTheBlanks[i].minute;
        }
        for (int i = 0; i < MainPlayer.songResultInfo.operatings.Count; i++)
        {
            m += MainPlayer.songResultInfo.operatings[i].minute;
        }

        if(m > 80)
        {
            m = 80;
        }
        minute.text = m.ToString();
        summary.text = MainPlayer.songResultInfo.summary;
    }
}
