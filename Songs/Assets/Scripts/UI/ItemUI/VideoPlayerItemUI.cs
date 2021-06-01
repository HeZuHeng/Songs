using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPlayerItemUI : MonoBehaviour
{
    public Text[] texts;
    public VideoPlayer videoPlayer;
    public string videoName;
    public float startTime;
    public Button next;
    public bool IsClose = false;
    int index1 = 0;

    private void Awake()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if(next != null) next.onClick.AddListener(OnClose);
        
    }

    private void OnEnable()
    {
        DoMove();
        
        index1 = 0;
        if (index1 < texts.Length) texts[index1].gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].gameObject.SetActive(false);
        }
    }


    void ShowText()
    {
        if (index1 < texts.Length) texts[index1].gameObject.SetActive(false);
        index1++;

        if (index1 < texts.Length) texts[index1].gameObject.SetActive(true);
        if (index1 >= texts.Length - 1)
        {
            CancelInvoke("ShowText");
        }
    }

    void DoMove()
    {
        ExperienceUtil.Instance.Close();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.streamingAssetsPath + "/"+ videoName;
        videoPlayer.loopPointReached -= OnLoopPointReached;
        videoPlayer.loopPointReached += OnLoopPointReached;
        videoPlayer.prepareCompleted -= OnPrepareCompleted;
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        //videoPlayer.enabled = true;
        //Application.targetFrameRate = 30;
        videoPlayer.Play();

    }

    void OnPrepareCompleted(VideoPlayer videoPlayer)
    {
        CancelInvoke("ShowText");
        float time = videoPlayer.frameCount / videoPlayer.frameRate / texts.Length;
        InvokeRepeating("ShowText", time + startTime, time);
    }

    void OnLoopPointReached(VideoPlayer videoPlayer)
    {
        OnClose();
    }

    void OnClose()
    {
        //Application.targetFrameRate = -1;
        if (!IsClose) gameObject.SetActive(false);
    }
}