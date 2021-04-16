using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;
using UnityEngine.Video;

public class ExperienceWnd : UIBase
{
    public Text[] texts;
    public VideoPlayer videoPlayer;
    public Texture[] textures;
    int index = 0;
    int index1 = 0;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.ExperienceWnd;
        MutexInterface = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //trendsText.Play();
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].gameObject.SetActive(false);
        }
        index = 0;
        ExperienceUtil.Instance.SetTexture(textures[index]);
        //CancelInvoke("DoMove");
        //Invoke("DoMove", 1);
        DoMove();
        index1 = 0;
        texts[index1].gameObject.SetActive(true);

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ExperienceUtil.Instance.InitPosition();
    }

    void ShowText()
    {
        texts[index1].gameObject.SetActive(false);
        index1++;

        texts[index1].gameObject.SetActive(true);
        if (index1 >= texts.Length - 1)
        {
            CancelInvoke("ShowText");
        }
    }

    void DoMove()
    {
        ExperienceUtil.Instance.Close();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.streamingAssetsPath + "/ExperienceWnd.mp4";
        videoPlayer.loopPointReached -= OnLoopPointReached;
        videoPlayer.loopPointReached += OnLoopPointReached;
        videoPlayer.prepareCompleted -= OnPrepareCompleted;
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        //videoPlayer.enabled = true;
        //Application.targetFrameRate = 30;
        videoPlayer.Play();
        //ExperienceUtil.Instance.DoMoveCamera(textures[index], new Vector3(1, 2, -4), 7.5f, delegate () {
        //    index++;
        //    ExperienceUtil.Instance.DoMoveCamera(textures[index], new Vector3(-1, 1, -6), 7.5f, delegate () {
        //        OnClose();
        //    });
        //});
        
    }

    void OnPrepareCompleted(VideoPlayer videoPlayer) 
    {
        CancelInvoke("ShowText");
        float time = videoPlayer.frameCount / videoPlayer.frameRate / 5f;
        InvokeRepeating("ShowText", time + 2, time);
    }

    void OnLoopPointReached(VideoPlayer videoPlayer)
    {
        OnClose();
    }

    public void OnClose()
    {
        //Application.targetFrameRate = -1;
        UIMng.Instance.OpenUI(UIType.NONE);
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
