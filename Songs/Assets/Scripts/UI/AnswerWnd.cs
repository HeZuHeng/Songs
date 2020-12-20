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

    private int num;
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
        num = 0;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
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

    void OnEndParsing()
    {
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;
        answerParent.gameObject.SetActive(false);
        startParent.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(question.endParsing))
        {
            startParent.gameObject.SetActive(true);
            trendsText.m_CallBack.RemoveListener(OnEnd);
            trendsText.m_CallBack.AddListener(OnStartParsing);
            trendsText.Play(question.startParsing.Replace("\\n", "\n"));
            return;
        }
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
        for (int i = 0; i < question.questions.Count; i++)
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
            tran.GetComponentInChildren<Text>().text = question.questions[i];
            tran.localPosition = new Vector3(scrollRect.content.GetChild(0).localPosition.x, i * -100, tran.localPosition.z);
            tran.localScale = Vector3.one;
            tran.gameObject.SetActive(true);
        }
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.localPosition.x, question.questions.Count * 100);
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
        bool val = t.Count == question.answers.Count;
        if(val)
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
            num++;
            if (num >= 3)
            {
                errorTip.text = "选择错误！请重新选择。";
                StopCoroutine(Reset());
                StartCoroutine(Reset());
            }
            else
            {
                errorTip.text = question.errorTip;
            }
            errorTip.enabled = true;
            return;
        }
        if (!string.IsNullOrEmpty(question.endParsing))
        {
            OnEndParsing();
        }
        else
        {
            OnEnd();
        }
    }

    void OnEnd()
    {
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
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

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.3f);
        Show();
    }
}
