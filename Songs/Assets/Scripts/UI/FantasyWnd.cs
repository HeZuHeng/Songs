using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;
using DG.Tweening;
using Coffee.UIEffects;
using UnityEngine.EventSystems;

public class FantasyWnd : UIBase
{
    static string[] Tips =
    {
        "In contrast to thousands of daffodils in the objective world, the speaker’s loneliness and frustration are highlighted.|与客体世界中无数的水仙花形成对比，体现说话者的孤单、沮丧。",
        "In contrast with a group of brisk and lively flowers, the speaker is melancholy.|一群舞姿轻快、活泼的意象，与说话者的忧郁惆怅形成对比。",
        "Shining bright, lighting up the world and driving away the speaker’s inner haze.|闪耀璀璨，照亮了世界并驱赶了说话者内心的阴霾。",
        "A path that enables the speaker to get into the happy paradise and discover himself.|一条使说话者能够通往快乐的天堂、发现自我的道路。",
        "In their gleeful fluttering and dancing, the daffodils outdo ___ of lake.|___远不如水仙跳舞跳得欢快、热烈。"
    };

    static string[] SongTexts =
    {
        "I wandered lonely as a cloud \nThat floats on high o’er vales and hills \nWhen all at once I saw a crowd,\nhost, of golden daffodils; \nBesides the lake, beneath the trees, \nFluttering and dancing in the breeze.",
        "Continuous as the stars that shine \nAnd twinkle on the Milky Way, \nThey stretched in never-ending line \nAlong the margin of a bay: \nTen thousand saw I at a glance, \nTossing their heads in sprightly dance.",
        "The waves beside them danced; but they \n Outdid the sparking waves in glee: \nA poet could not but be gay, \nIn such a jocund company! \nI gazed – and gazed – but little thought \nWhat wealth the show to me had brought."
    };

    static string[] Names =
    {
        "一朵云","成片且摇曳的水仙花","漫天闪烁的星星","银河","粼粼水波"
    };

    public Image[] images;
    public UIShadow[] shadows;
    public ScrollRect scrollRect;
    public TrendsText tip;
    public Text tip2;
    public Button btnTip;
    public Text songText;

    public FantasyItemUI[] itemUIs;

    int index = 0;
    Image curImage = null;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.FantasyWnd;
        MutexInterface = true;
    }

    protected override void Start()
    {
        base.Start();
        btnTip.onClick.AddListener(OnClickBtnTip);
        for (int i = 0; i < itemUIs.Length; i++)
        {
            itemUIs[i].OnPointUp += OnPointUpIamge;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        index = 0;
        curImage = null;
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            scrollRect.content.GetChild(i).localScale = Vector3.one;
            scrollRect.content.GetChild(i).gameObject.SetActive(true);
        }
        Show();
        tip2.text = "Stanza one";
        SceneController.TerrainController.SetDetailObjects(0.001f);
        SceneController.TerrainController.SetWater(false);
        SceneController.TerrainController.SetSky(0,1f);
        UIMng.Instance.ActivationUI(UIType.MemoryWnd);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIMng.Instance.ConcealUI(UIType.MemoryWnd);
    }

    void OnClickBtnTip()
    {
        if(index < 1)
        {
            songText.text = SongTexts[0];
        }
        else if (index < 3)
        {
            songText.text = SongTexts[1];
        }
        else
        {
            songText.text = SongTexts[2];
        }
        songText.transform.parent.gameObject.SetActive(true);
    }

    void OnPointUpIamge(Image image)
    {
        if (image != images[index])
        {
            image.gameObject.SetActive(true);
            tip.transform.parent.DOShakeRotation(2, new Vector3(10, 7, 0));
            tip.transform.parent.DOShakePosition(2, new Vector3(5, 6, 0));
            return;
        }
        StopAllCoroutines();
        StartCoroutine(OnShowFantasy(image));
    }

    IEnumerator OnShowFantasy(Image image)
    {
        
        image.gameObject.SetActive(false);
        QuestionBankData bankData = null;
        if (index == 0)
        {
           
            SceneController.TerrainController.SetSky(1, 0.7f);
            MainPlayer.songResultInfo.FillAnswer(3,1, string.Empty, 1, AnswerType.Operating);
            yield return new WaitForSeconds(0.3f);
        }

        if (index == 1)
        {
            SceneController.TerrainController.SetDetailObjects(1);
            SongsDataMng.GetInstance().Player.MemoryNum = 1;
            MainPlayer.songResultInfo.FillAnswer(3, 1, string.Empty, 2, AnswerType.Operating);
            yield return new WaitForSeconds(1f);
            tip.transform.parent.gameObject.SetActive(false);
            bankData = SongsDataMng.GetInstance().SetQuestion(1);
            bankData.onQuestionEnd.RemoveListener(NextFantasy);
            bankData.onQuestionEnd.AddListener(NextFantasy);
            UIMng.Instance.ActivationUI(UIType.AnswerWnd);
            tip2.text = "Stanza two";
        }

        if (index == 2)
        {
            SceneController.TerrainController.SetSky(2, 0.3f);
            MainPlayer.songResultInfo.FillAnswer(3, 2, string.Empty, 1, AnswerType.Operating);
            yield return new WaitForSeconds(0.3f);
        }

        if (index == 3)
        {
            SceneController.TerrainController.SetSky(3, 0.1f);
            SongsDataMng.GetInstance().Player.MemoryNum = 2;
            MainPlayer.songResultInfo.FillAnswer(3, 2, string.Empty, 2, AnswerType.Operating);
            yield return new WaitForSeconds(1f);
            tip.transform.parent.gameObject.SetActive(false);
            bankData = SongsDataMng.GetInstance().SetQuestion(2);
            bankData.onQuestionEnd.RemoveListener(NextFantasy);
            bankData.onQuestionEnd.AddListener(NextFantasy);
            UIMng.Instance.ActivationUI(UIType.AnswerWnd);
            tip2.text = "Stanza three";
        }
        if (index == 4)
        {
            
            SceneController.TerrainController.SetSky(1, 1f);
            SceneController.TerrainController.SetWater(true);
            SongsDataMng.GetInstance().Player.MemoryNum = 3;
            MainPlayer.songResultInfo.FillAnswer(3, 3, string.Empty, 1, AnswerType.Operating);
            yield return new WaitForSeconds(1f);
            tip.transform.parent.gameObject.SetActive(false);
            bankData = SongsDataMng.GetInstance().SetQuestion(3);
            bankData.onQuestionEnd.RemoveListener(NextFantasy);
            bankData.onQuestionEnd.AddListener(NextFantasy);
            UIMng.Instance.ActivationUI(UIType.AnswerWnd);
        }

        if(bankData == null) NextFantasy();

    }

    void NextFantasy()
    {
        index++;
        Show();
    }

    void Show()
    {
        if (index >= Names.Length)
        {
            tip.transform.parent.gameObject.SetActive(false);
            OnEnd();// NextQuestion();
            return;
        }
        tip.transform.parent.gameObject.SetActive(true);
        tip.m_CallBack.RemoveListener(ShowIconTip);
        tip.m_CallBack.AddListener(ShowIconTip);
        tip.Play(Tips[index]);
    }

    void ShowIconTip()
    {
        for (int i = 0; i < shadows.Length; i++)
        {
            shadows[i].enabled = index == i;
        }
    }

    void NextQuestion()
    {
        QuestionBankData bankData = SongsDataMng.GetInstance().SetQuestion(4);
        bankData.onQuestionEnd.RemoveListener(NextQuestionTwo);
        bankData.onQuestionEnd.AddListener(NextQuestionTwo);
        UIMng.Instance.ActivationUI(UIType.AnswerWnd);
    }

    void NextQuestionTwo()
    {
        QuestionBankData bankData = SongsDataMng.GetInstance().SetQuestion(5);
        bankData.onQuestionEnd.RemoveListener(OnEnd);
        bankData.onQuestionEnd.AddListener(OnEnd);
        UIMng.Instance.ActivationUI(UIType.AnswerWnd);
    }

    void OnEnd()
    {
        UIMng.Instance.OpenUI(UIType.NONE);
        SongsDataMng.GetInstance().Player.MemoryNum = 4;

        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        if (taskData != null)
        {
            if (taskData.type == TaskType.OpenWnd)
            {
                UIType type = (UIType)System.Enum.Parse(typeof(UIType), taskData.val);
                if (type == Type)
                {
                    taskData.TaskState = TaskState.End;
                }
            }
        }
    }

}
