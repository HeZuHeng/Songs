using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class QATalkWnd : UIBase
{
    public TrendsText songText;
    public TrendsText head;
    public Text tip;
    public Text talkName;
    public Image icon;

    public RectTransform rectTransform;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.QATalkWnd;
        MutexInterface = true;

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        rectTransform.gameObject.SetActive(false);
        Show();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    void Show()
    {
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        StopAllCoroutines();
        StartCoroutine(GetSongFileText(question.des));
        tip.text = question.startParsing;

        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        ModelData modelData = SongsDataMng.GetInstance().GetModelData(taskData.val);
        if (modelData != null)
        {
            talkName.text = modelData.name;
            Sprite obj = Resources.Load<Sprite>("Sprites/PlayerIcon/" + modelData.icon);
            if (obj != null)
            {
                icon.sprite = obj;
            }
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
            talkName.text = "系统";
        }
        string des = string.Format(question.head, SongsDataMng.GetInstance().Player.name);
        head.m_CallBack.RemoveListener(OnEndParsing);
        head.m_CallBack.RemoveListener(OnHeadEnd);
        head.m_CallBack.AddListener(OnHeadEnd);
        head.Play(des.Replace("\\n", "\n"));
    }

    IEnumerator GetSongFileText(string path)
    {
        UnityWebRequest unityWeb = UnityWebRequest.Get(Application.streamingAssetsPath + path);
        yield return unityWeb.SendWebRequest();
        if (unityWeb.isDone)
        {
            if (!unityWeb.isHttpError)
            {
                songText.Play(unityWeb.downloadHandler.text);
            }
        }
    }

    void OnHeadEnd()
    {
        rectTransform.gameObject.SetActive(true);
    }

    public void OnSelected()
    {
        rectTransform.gameObject.SetActive(false);
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;
        head.m_CallBack.RemoveListener(OnHeadEnd);
        head.m_CallBack.RemoveListener(OnEndParsing);
        head.m_CallBack.AddListener(OnEndParsing);
        head.Play(question.endParsing.Replace("\\n", "\n"));
    }

    void OnEndParsing()
    {
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        if (taskData != null)
        {
            if (taskData.type == TaskType.QATalk)
            {
                if (taskData.val.Equals(question.Id.ToString()))
                {
                    taskData.TaskState = TaskState.End;
                }
            }
        }
        UIMng.Instance.OpenUI(UIType.NONE);
    }
}
