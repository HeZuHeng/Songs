using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;
using UnityEngine.Networking;

public class LeftDialogueWnd : UIBase
{
    public TrendsText lblStatus;
    public Button nextBtn;

    private string[] allTexts;
    int index = 0;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.LeftDialogueWnd;
        MutexInterface = false;
        nextBtn.onClick.AddListener(OnNext);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StopAllCoroutines();
        StartCoroutine(GetSongFileText(SongsDataMng.GetInstance().GetSongFilePath));
    }

    protected override void OnDisable()
    {
        CheckEnd();
        allTexts = null;
    }

    IEnumerator GetSongFileText(string path)
    {
        UnityWebRequest unityWeb = UnityWebRequest.Get(Application.streamingAssetsPath + path);
        yield return unityWeb.SendWebRequest();
        if (unityWeb.isDone)
        {
            if (!unityWeb.isHttpError)
            {
                allTexts = unityWeb.downloadHandler.text.Split('|');
                if(allTexts == null || allTexts.Length == 0)
                {
                    UIMng.Instance.ConcealUI(UIType.LeftDialogueWnd);
                }
                else
                {
                    index = 0;
                    Show();
                }
            }
            else
            {
                UIMng.Instance.ConcealUI(UIType.LeftDialogueWnd);
            }
        }
    }

    void Show()
    {
        lblStatus.m_CallBack.RemoveListener(OnNext);
        lblStatus.m_CallBack.AddListener(OnNext);
        lblStatus.Play(allTexts[index]);
    }

    void OnNext()
    {
        index++;
        if (CheckEnd())
        {
            Show();
        }
        else
        {
            UIMng.Instance.ConcealUI(UIType.LeftDialogueWnd);
        }
    }

    bool CheckEnd()
    {
        if (allTexts == null) return false;
        if (index >= allTexts.Length)
        {
            TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
            if (taskData != null)
            {
                if (taskData.type == TaskType.LookSong && !string.IsNullOrEmpty(taskData.val))
                {
                    if (taskData.val.Equals(SongsDataMng.GetInstance().GetSongFilePath))
                    {
                        taskData.TaskState = TaskState.End;
                    }
                }
            }
            return false;
        }
        return true;
    }
}
