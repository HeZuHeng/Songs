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

    public VideoPlayerItemUI itemUI;

    Vector3 initPos;
    Vector3 pos;
    protected override void Awake()
    {
        base.Awake();
        itemUI.gameObject.SetActive(false);
        Type = UIType.LeftDialogueWnd;
        MutexInterface = false;
        nextBtn.onClick.AddListener(OnEnd);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        lblStatus.m_Text = string.Empty;
        lblStatus.m_AudioName = string.Empty;
        if (lblStatus.m_AudioClip != null)
        {
            AudioClip audioClip = lblStatus.m_AudioClip;
            lblStatus.m_AudioClip = null;
            audioClip.UnloadAudioData();
        }
        //StopAllCoroutines();
        StartCoroutine(GetSongFileText(SongsDataMng.GetInstance().GetSongFilePath));
    }

    protected override void OnDisable()
    {
        //CheckEnd();
        StopAllCoroutines();
        SongsDataMng.GetInstance().GetSongSoundPath = string.Empty;
        SongsDataMng.GetInstance().GetSongFilePath = string.Empty;
        lblStatus.m_Text = string.Empty;
        if (lblStatus.m_AudioClip != null)
        {
            AudioClip audioClip = lblStatus.m_AudioClip;
            lblStatus.m_AudioClip = null;
            audioClip.UnloadAudioData();
        }
    }

    IEnumerator GetSongFileText(string path)
    {
        UnityWebRequest unityWeb = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + path.ToLower());
        yield return unityWeb.SendWebRequest();
        if (unityWeb.isDone)
        {
            if (!unityWeb.isHttpError)
            {
                Show(unityWeb.downloadHandler.text,SongsDataMng.GetInstance().GetSongSoundPath);
            }
            else
            {
                UIMng.Instance.ConcealUI(UIType.LeftDialogueWnd);
            }
        }
    }

    void Show(string text,string soundName)
    {
        lblStatus.m_CallBack.RemoveListener(OnNext);
        lblStatus.m_CallBack.AddListener(OnNext);
        lblStatus.Play(text, soundName);

        if ("iwanderedlonelyasacloud4.txt".Equals(SongsDataMng.GetInstance().GetSongFilePath))
        {
            itemUI.gameObject.SetActive(true);
        }
        else
        {
            itemUI.gameObject.SetActive(false);
        }
    }

    void OnNext()
    {

    }

    void OnEnd()
    {
        CheckEnd();
        itemUI.gameObject.SetActive(false);
        UIMng.Instance.ConcealUI(UIType.LeftDialogueWnd);
    }

    void CheckEnd()
    {
        TaskData taskData = SongsDataMng.GetInstance().GetTaskData;
        if (taskData != null)
        {
            if (taskData.type == TaskType.LookSong && !string.IsNullOrEmpty(taskData.val))
            {
                if (taskData.val.Equals(SongsDataMng.GetInstance().GetSongFilePath))
                {
                    if ("iwanderedlonelyasacloud4.txt".Equals(SongsDataMng.GetInstance().GetSongFilePath))
                    {
                        MainPlayer.songResultInfo.FillAnswer(4, string.Empty, 2, AnswerType.Operating);
                    }
                    if ("yinjiuqi5.txt".Equals(SongsDataMng.GetInstance().GetSongFilePath))
                    {
                        MainPlayer.songResultInfo.FillAnswer(6, string.Empty, 2, AnswerType.Operating);
                    }
                    if ("htmsong.txt".Equals(SongsDataMng.GetInstance().GetSongFilePath))
                    {
                        MainPlayer.songResultInfo.FillAnswer(17, string.Empty, 1, AnswerType.Operating);
                    }
                    if ("tiangou.txt".Equals(SongsDataMng.GetInstance().GetSongFilePath))
                    {
                        MainPlayer.songResultInfo.FillAnswer(20, string.Empty, 1, AnswerType.Operating);
                    }

                    if ("htmsong1.txt".Equals(SongsDataMng.GetInstance().GetSongFilePath))
                    {
                        MainPlayer.songResultInfo.FillAnswer(21, string.Empty, 1, AnswerType.Operating);
                    }
                    if ("htmsong2.txt".Equals(SongsDataMng.GetInstance().GetSongFilePath))
                    {
                        MainPlayer.songResultInfo.FillAnswer(22, string.Empty, 1, AnswerType.Operating);
                    }
                    taskData.TaskState = TaskState.End;
                }
            }
        }
    }
}
