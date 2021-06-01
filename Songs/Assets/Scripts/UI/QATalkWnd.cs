﻿using BuildUtil;
using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class QATalkWnd : UIBase
{
    [Header("选择题")]
    public Toggle qA;
    public Toggle qB;
    public Toggle qC;
    public Toggle qD;
    public Button ok;

    [Header("简答题")]
    public TrendsText songText;
    public TrendsText head;
    public Text tip;
    public Text talkName;
    public Image icon;
    public InputField inputField;
    public Text errorTip;

    public RectTransform rectTransform;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.QATalkWnd;
        MutexInterface = true;
        ok.onClick.AddListener(OnClickOk);

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        rectTransform.gameObject.SetActive(false);
        //Show();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }
    #region 简答题
    void Show()
    {
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        StopAllCoroutines();
        StartCoroutine(GetSongFileText(question.des));

        tip.text = question.startParsing;
        errorTip.enabled = false;
        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        ModelData modelData = SongsDataMng.GetInstance().GetModelData(taskData.des);
        string iconN = string.Empty;
        if (modelData != null)
        {
            talkName.text = modelData.name;
            iconN = modelData.icon;
            Sprite obj = Resources.Load<Sprite>("Sprites/PlayerIcon/" + iconN);
            if (obj != null)
            {
                icon.sprite = obj;
            }
            icon.enabled = true;
        }
        else
        {
            talkName.text = "系统提示";
            icon.enabled = false;
        }

        string des = string.Format(question.head, SongsDataMng.GetInstance().Player.name);
        head.m_CallBack.RemoveListener(OnEndParsing);
        head.m_CallBack.RemoveListener(OnHeadEnd);
        head.m_CallBack.AddListener(OnHeadEnd);
        head.Play(des.Replace("\\n", "\n"));
    }

    IEnumerator GetSongFileText(string path)
    {
        UnityWebRequest unityWeb = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + path.ToLower());
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
        Debug.Log("答案： "+ inputField.text);
        QuestionBankData question = SongsDataMng.GetInstance().GetQuestionBankData;
        if (question == null) return;

        int answer = question.questions.IndexOf(inputField.text);

        if (answer >= 0 && question.answers != null && question.answers.Count > 0 && !question.answers.Contains(answer))
        {
            if(errorTip != null)
            {
                if (!string.IsNullOrEmpty(question.errorTip))
                {
                    errorTip.text = question.errorTip.Replace("\\n", "\n");
                    errorTip.enabled = true;
                }
                return;
            }
        }

        MainPlayer.songResultInfo.FillSummary(inputField.text);

        rectTransform.gameObject.SetActive(false);

        Sprite obj = Resources.Load<Sprite>("Sprites/PlayerIcon/" + question.icon);
        if (obj != null)
        {
            icon.sprite = obj;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
        
        head.m_CallBack.RemoveListener(OnHeadEnd);
        head.m_CallBack.RemoveListener(OnEndParsing);
        head.m_CallBack.AddListener(OnEndParsing);
        if (!string.IsNullOrEmpty(question.endParsing))
        {
            head.Play(question.endParsing.Replace("\\n", "\n"));
        }
        else
        {
            OnEndParsing();
        }
    }

    void OnEndParsing()
    {
        UIMng.Instance.OpenUI(UIType.NONE);

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
    }

    #endregion

    #region 选择题
    void OnClickOk()
    {
        string text = "";
        float min = 0;
        if (qA.isOn)
        {
            text += qA.GetComponentInChildren<Text>().text;
            //text += "                                                                                   ";
            min += 7;
        }
        if (qB.isOn)
        {
            text += qB.GetComponentInChildren<Text>().text;
            //text += "                                                                                   ";
            min += 7;
        }
        if (qD.isOn)
        {
            text += qD.GetComponentInChildren<Text>().text;
            //text += "                                                                                   ";
            min += 6;
        }
        if (qC.isOn)
        {
            text += qC.GetComponentInChildren<Text>().text;
            //text += "                                                                                   ";
            min = 0;
        }
        MainPlayer.songResultInfo.FillSummary(text);
        MainPlayer.songResultInfo.summaryMinute = min;
        UIMng.Instance.OpenUI(UIType.NONE);

        SceneAssetObject assetObject = SceneMng.GetInstance().GetSceneAssetObject(10054);
        CameraMng.MainCamera.transform.LookAt(assetObject.Tran.position);
    }

    #endregion
}
