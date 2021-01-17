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

    public string m_Text;
    public string m_TextC;//中文
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
        SongsDataMng.GetInstance().orEnglishChange += OnEnglishChange;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SongsDataMng.GetInstance().orEnglishChange -= OnEnglishChange;
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
        answerParent.gameObject.SetActive(true);
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
        else
        {
            answerParent.gameObject.SetActive(true);
            startParent.gameObject.SetActive(false);
            Show(question);
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
            trendsText.m_CallBack.AddListener(OnEnd);
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

        //Sprite obj = Resources.Load<Sprite>("Sprites/PlayerIcon/" + question.icon);
        //if (obj != null)
        //{
        //    icon.sprite = obj;
        //}
        //else
        //{
        //    icon.enabled = false;
        //}
        string[] ecs = question.head.Split('|');
        if (ecs.Length > 0)
        {
            m_Text = ecs[0];
        }
        if (ecs.Length > 1)
        {
            m_TextC = ecs[1];
        }
        head.text = m_Text;
        errorTip.enabled = false;
        bool showTip = false;
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
            if (question.answers != null)
            {
                showTip = question.answers.Contains(i);
            }
            tran.GetComponentInChildren<AnswerItemUI>().Show(question.questions[i], showTip);
            tran.localPosition = new Vector3(100 + i * 600, -50, tran.localPosition.z);
            tran.localScale = Vector3.one;
            tran.gameObject.SetActive(true);
        }
        scrollRect.content.sizeDelta = new Vector2(200 + question.questions.Count * 600, scrollRect.content.sizeDelta.y);
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
        
        
        if (question.answers.Count > 0)
        {
            bool val = t.Count == question.answers.Count;
            if (val)
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
                if (num <= 1)
                {
                    errorTip.text = "选择错误！请重新选择。";
                    StopCoroutine(Reset());
                    StartCoroutine(Reset());
                    SceneController.GetInstance().ToState(State.TalkCameraError, null);
                }
                else
                {
                    errorTip.text = question.errorTip;
                }
                errorTip.enabled = true;
                return;
            }
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
        trendsText.m_CallBack.RemoveListener(OnEnd);
        UIMng.Instance.ConcealUI(UIType.AnswerWnd);

        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question.onQuestionEnd != null) question.onQuestionEnd.Invoke();
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
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.3f);
        Show();
    }


    void OnEnglishChange(bool val)
    {
        if (!val)
        {
            //CancelInvoke("ShowEnglish");
            ShowEnglish();
        }
        if (val)
        {
            head.text = m_TextC;
            //CancelInvoke("ShowEnglish");
            //Invoke("ShowEnglish", 10);
        }
    }

    void ShowEnglish()
    {
        head.text = m_Text;
    }
}
