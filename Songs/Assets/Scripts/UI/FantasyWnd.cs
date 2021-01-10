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
        "In contrast to the crowds in the object, the author’s loneliness and frustration are reflected.（与客体中的成群结队形成对比，体现作者的孤单、沮丧。）",
        "A group of brisk and lively images contrast with the poet’s melancholy.（一群舞姿轻快、活泼的意象，与诗人的忧郁惆怅形成对比。）",
        "Shining bright, lighting up the world and driving away the poet’s inner haze.（闪耀璀璨，照亮了世界并驱赶了诗人内心的阴霾。）",
        "A path that enables poet to a happy paradise and discover himself.（一条使诗人能够通往快乐的天堂、发现自我的道路。）",
        "even if aroused by the wind, it doesn’t dance as cheerfully and delightfully as daffodils do.（即使被风撩拨，它也远不如水仙跳舞跳得欢快、热烈。）"
    };

    static string[] Names =
    {
        "一朵云","成片且摇曳的水仙花","漫天闪烁的星星","银河","粼粼水波"
    };

    public Image[] images;
    public UIShadow[] shadows;
    public ScrollRect scrollRect;
    public TrendsText tip;

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

        SceneController.TerrainController.SetDetailObjects(0.1f);
        SceneController.TerrainController.SetWater(false);
        SceneController.TerrainController.SetSky(0,1f);
        UIMng.Instance.ActivationUI(UIType.MemoryWnd);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIMng.Instance.ConcealUI(UIType.MemoryWnd);
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
            yield return new WaitForSeconds(0.3f);
        }

        if (index == 1)
        {
            SceneController.TerrainController.SetDetailObjects(1);
            SongsDataMng.GetInstance().Player.MemoryNum = 1;
            yield return new WaitForSeconds(1f);
            tip.transform.parent.gameObject.SetActive(false);
            bankData = SongsDataMng.GetInstance().SetQuestion(1);
            bankData.onQuestionEnd.RemoveListener(NextFantasy);
            bankData.onQuestionEnd.AddListener(NextFantasy);
            UIMng.Instance.ActivationUI(UIType.AnswerWnd);
        }

        if (index == 2)
        {
            SceneController.TerrainController.SetSky(2, 0.3f);
            yield return new WaitForSeconds(0.3f);
        }

        if (index == 3)
        {
            SceneController.TerrainController.SetSky(3, 0.1f);
            SongsDataMng.GetInstance().Player.MemoryNum = 2;
            yield return new WaitForSeconds(1f);
            tip.transform.parent.gameObject.SetActive(false);
            bankData = SongsDataMng.GetInstance().SetQuestion(2);
            bankData.onQuestionEnd.RemoveListener(NextFantasy);
            bankData.onQuestionEnd.AddListener(NextFantasy);
            UIMng.Instance.ActivationUI(UIType.AnswerWnd);
        }
        if (index == 4)
        {
            SceneController.TerrainController.SetSky(1, 1f);
            SceneController.TerrainController.SetWater(true);
            SongsDataMng.GetInstance().Player.MemoryNum = 3;
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
            NextQuestion();
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
