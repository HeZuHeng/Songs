using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class AnswerWnd : UIBase
{
    public RectTransform answerParent;
    public RectTransform startParent;

    public ScrollRect scrollRect;

    public TrendsText trendsText;
    public Text head;
    public Text errorTip;
    public Button confirmBtn;
    public Image icon;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.SettingWnd;
        MutexInterface = false;
        confirmBtn.onClick.AddListener(OnConfirm);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Show();
        transform.SetAsLastSibling();
    }

    void Show()
    {
        answerParent.gameObject.SetActive(false);
        startParent.gameObject.SetActive(false);

        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;


        if (!string.IsNullOrEmpty(question.des))
        {
            startParent.gameObject.SetActive(true);
            trendsText.m_CallBack.RemoveListener(OnReadDes);
            trendsText.m_CallBack.AddListener(OnReadDes);
            trendsText.Play(question.des.Replace("\\n", "\n"));
            return;
        }
        if (!string.IsNullOrEmpty(question.startParsing))
        {
            startParent.gameObject.SetActive(true);
            trendsText.m_CallBack.RemoveListener(OnStartParsing);
            trendsText.m_CallBack.AddListener(OnStartParsing);
            trendsText.Play(question.startParsing.Replace("\\n", "\n"));
            return;
        }
        Show(question);
    }

    private void OnReadDes()
    {
        trendsText.m_CallBack.RemoveListener(OnReadDes);
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;
        if (!string.IsNullOrEmpty(question.startParsing))
        {
            startParent.gameObject.SetActive(true);
            trendsText.m_CallBack.RemoveListener(OnStartParsing);
            trendsText.m_CallBack.AddListener(OnStartParsing);
            trendsText.Play(question.startParsing.Replace("\\n", "\n"));
        }
    }

    void OnStartParsing()
    {
        trendsText.m_CallBack.RemoveListener(OnStartParsing);
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;
        answerParent.gameObject.SetActive(true);
        startParent.gameObject.SetActive(false);
        Show(question);
    }

    void Show(QuestionBankData question)
    {
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            scrollRect.content.GetChild(i).gameObject.SetActive(false);
            scrollRect.content.GetChild(i).GetComponentInChildren<Toggle>().isOn = false;
        }

        Sprite obj = Resources.Load<Sprite>("Sprites/" + question.icon);
        if (obj != null)
        {
            icon.sprite = obj;
        }
        else
        {
            icon.enabled = false;
        }
        head.text = question.head;
        errorTip.enabled = false;
        Transform tran = null;
        int count = scrollRect.content.childCount;
        string[] questions = question.questions.Split('|');
        for (int i = 0; i < questions.Length; i++)
        {
            if (count > i)
            {
                tran = scrollRect.content.GetChild(i);
            }
            else
            {
                tran = Instantiate(scrollRect.content.GetChild(0));
                tran.transform.SetParent(scrollRect.content);
            }
            tran.GetComponentInChildren<Text>().text = questions[i];
            tran.localPosition = new Vector3(0, i * -100, tran.localPosition.z);
            tran.gameObject.SetActive(true);
        }
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.localPosition.x, questions.Length * 100);
        scrollRect.content.localPosition = Vector3.zero;
    }

    void OnConfirm()
    {
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        List<int> t = new List<int>();
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            if (scrollRect.content.GetChild(i).GetComponentInChildren<Toggle>().isOn)
            {
                t.Add(i);
            }
        }
        bool val = true;
        if(t.Count == question.answers.Count)
        {
            for (int i = 0; i < t.Count; i++)
            {
                if (!question.answers.Contains(t[i]))
                {
                    val = false;
                }
            }
        }
        if (!val)
        {
            errorTip.text = question.errorTip;
            errorTip.enabled = true;
            return;
        }

        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        if (taskData != null)
        {
            if (taskData.type == TaskType.Question)
            {
                if (taskData.val.Equals(question.Id.ToString()))
                {
                    taskData.TaskState = TaskState.End;
                }
            }
        }
        UIMng.Instance.ConcealUI(UIType.AnswerWnd);
    }
}
