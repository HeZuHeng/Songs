using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class AnswerWnd : UIBase
{
    static string[] ABCDEFG = new string[7] { "A", "B", "C", "D", "E", "F", "G", };

    public RectTransform answerParent;
    public RectTransform startParent;

    public ScrollRect scrollRect;
    public Text trendsTextHead;
    public TrendsText trendsText;
    public Text head;
    public Text errorTip;
    public Button confirmBtn;
    public Image icon;

    public Button btn1;
    public Button btn2;
    public Button btn3;

    public string m_Text;
    public string m_TextC;//中文
    private int num;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.SettingWnd;
        MutexInterface = false;
        confirmBtn.onClick.AddListener(OnConfirm);
        btn1.onClick.AddListener(OnConfirm1);
        btn2.onClick.AddListener(OnConfirm2);
        btn3.onClick.AddListener(OnConfirm3);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        num = 0;
        Show();
        m_TextC = string.Empty;
        m_Text = string.Empty;
        transform.SetAsLastSibling();
        
        SongsDataMng.GetInstance().orEnglishChange += OnEnglishChange;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SongsDataMng.GetInstance().orEnglishChange -= OnEnglishChange;
        StopAllCoroutines();
        CancelInvoke("ConfirmEnd");
    }

    void Show()
    {
        answerParent.gameObject.SetActive(false);
        startParent.gameObject.SetActive(false);
        btn3.gameObject.SetActive(false);
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;


        if (!string.IsNullOrEmpty(question.des))
        {
            startParent.gameObject.SetActive(true);
            //trendsText.m_CallBack.RemoveListener(OnReadDes);
            //trendsText.m_CallBack.AddListener(OnReadDes);
            btn3.gameObject.SetActive(true);
            trendsTextHead.text = "诗歌阅读";
            trendsText.Play(question.des.Replace("\\n", "\n"));
            return;
        }
        if (!string.IsNullOrEmpty(question.startParsing))
        {
            startParent.gameObject.SetActive(true);
            trendsTextHead.text = "诗歌解析";
            //trendsText.m_CallBack.RemoveListener(OnStartParsing);
            //trendsText.m_CallBack.AddListener(OnStartParsing);
            btn3.gameObject.SetActive(true);
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
            trendsTextHead.text = "诗歌解析";
            //trendsText.m_CallBack.RemoveListener(OnStartParsing);
            //trendsText.m_CallBack.AddListener(OnStartParsing);
            btn3.gameObject.SetActive(true);
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
            //trendsText.m_CallBack.RemoveListener(OnEnd);
            //trendsText.m_CallBack.AddListener(OnEnd);
            btn3.gameObject.SetActive(true);
            trendsTextHead.text = "答案解析";
            trendsText.Play(question.endParsing.Replace("\\n", "\n"));
            return;
        }
    }

    void Show(QuestionBankData question)
    {
        btn1.gameObject.SetActive(!string.IsNullOrEmpty(question.des));
        btn2.gameObject.SetActive(!string.IsNullOrEmpty(question.startParsing));
        btn3.gameObject.SetActive(false);
        confirmBtn.gameObject.SetActive(true);
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
        float total = 50 + question.questions.Count * 450;
        float startPos = 50 + (5 - question.questions.Count) * 150;
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
                showTip = question.answers.Contains(i) && !string.IsNullOrEmpty(question.errorTip);
            }
            tran.localPosition = new Vector3(startPos + i * 450, -70, tran.localPosition.z);
            tran.GetComponentInChildren<AnswerItemUI>().Show(question.questions[i], showTip);
            tran.localScale = Vector3.one;
            tran.gameObject.SetActive(true);
        }
        scrollRect.content.sizeDelta = new Vector2(total, scrollRect.content.sizeDelta.y);
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
                scrollRect.content.GetChild(i).GetComponentInChildren<AnswerItemUI>().Show(question.answers != null && question.answers.Contains(i));
            }
        }
        string ansStr = string.Empty;
        int minute = 0;

        if (question.answers.Count > 0 && t.Count <= question.answers.Count)
        {
            for (int i = 0; i < t.Count; i++)
            {
                if (t[i] == question.answers[i])
                {
                    minute += question.minute;
                    if (i == 0)
                    {
                        if(question.answerType == AnswerType.FillInTheBlank)
                        {
                            ansStr = question.questions[t[i]];
                        }
                        else if (question.answerType == AnswerType.SingleChoice || question.answerType == AnswerType.MultipleChoice)
                        {
                            ansStr = ABCDEFG[t[i]];
                        }
                    }
                    else
                    {
                        if (question.answerType == AnswerType.FillInTheBlank)
                        {
                            ansStr += "," + question.questions[t[i]];
                        }
                        else if (question.answerType == AnswerType.SingleChoice || question.answerType == AnswerType.MultipleChoice)
                        {
                            ansStr = "," + ABCDEFG[t[i]];
                        }
                    }
                }
            }
        }

        //if (minute == 0 && question.errorTip != null)
        //{
        //    num++;
        //    if (num <= 1)
        //    {
        //        errorTip.text = "选择错误！请重新选择。";
        //        StopCoroutine(Reset());
        //        StartCoroutine(Reset());
        //        SceneController.GetInstance().ToState(State.TalkCameraError, null);
        //    }
        //    else
        //    {
        //        errorTip.text = question.errorTip;
        //    }
        //    errorTip.enabled = true;
        //    return;
        //}

        if (question.answerChildId > 0)
        {
            MainPlayer.songResultInfo.FillAnswer(question.answerId, question.answerChildId, ansStr, minute, question.answerType);
        }
        else
        {
            MainPlayer.songResultInfo.FillAnswer(question.answerId, ansStr, minute, question.answerType);
        }
        confirmBtn.gameObject.SetActive(false);
        CancelInvoke("ConfirmEnd");
        Invoke("ConfirmEnd",1f);
    }

    void ConfirmEnd()
    {
        confirmBtn.gameObject.SetActive(true);
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (!string.IsNullOrEmpty(question.endParsing))
        {
            OnEndParsing();
        }
        else
        {
            OnEnd();
        }
    }

    void OnConfirm1()
    {
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;
        startParent.gameObject.SetActive(true);
        btn3.gameObject.SetActive(true);
        num = 1;
        trendsTextHead.text = "诗歌阅读";
        trendsText.Show(question.des.Replace("\\n", "\n"));
    }

    void OnConfirm2()
    {
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;
        startParent.gameObject.SetActive(true);
        btn3.gameObject.SetActive(true);
        num = 2;
        trendsTextHead.text = "诗歌解析";
        trendsText.Show(question.startParsing.Replace("\\n", "\n"));
    }

    void OnConfirm3()
    {
        if(num > 0)
        {
            num = 0;
            startParent.gameObject.SetActive(false);
            return;
        }
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;
        if (!string.IsNullOrEmpty(question.endParsing) && startParent.gameObject.activeSelf)
        {
            if (question.endParsing.Replace("\\n", "\n").Contains(trendsText.m_Text))
            {
                OnEnd();
                startParent.gameObject.SetActive(false);
                return;
            }
        }

        if (!string.IsNullOrEmpty(question.startParsing) && startParent.gameObject.activeSelf)
        {
            if (question.startParsing.Replace("\\n", "\n").Contains(trendsText.m_Text))
            {
                OnStartParsing();
                return;
            }
        }

        if (!string.IsNullOrEmpty(question.des) && startParent.gameObject.activeSelf)
        {
            if (question.des.Replace("\\n", "\n").Contains(trendsText.m_Text))
            {
                OnReadDes();
                return;
            }
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
        if (val && !string.IsNullOrEmpty(m_TextC))
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
