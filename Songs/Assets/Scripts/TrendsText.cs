using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TrendsText : MonoBehaviour
{
    public uint m_TextIndent;       //首行缩进--字符数
    public float endYieldTime = 0.5f;
    [TextArea(4, 10)]
    public string m_Text;           //显示的文本内容
    public bool m_Enable;           //是否在OnEnable中初始化Play
    private Text m_Conetnt;         //用于显示的文本
    [Header("字/每秒")]
    public float m_ShowSpeed = 1;   //动态文字的速度
                                    //与文字播放同时播放的音频
    public AudioClip m_AudioClip;
    //文字播完回调
    public CallBack m_CallBack;

    public ScrollRect m_ScrollRect;


    private char[] m_Text_Char;
    private bool suspend = false;
    private AudioSource m_AudioSource;

    private RectTransform rect;
    private void Awake()
    {
        m_Conetnt = this.GetComponent<Text>();
        rect = m_Conetnt.transform as RectTransform;
    }
    private void OnEnable()
    {
        if (m_Enable)
            Play();
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 重新播放
    /// </summary>
    public void Play()
    {
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            m_ShowSpeed = 100;
        }
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            m_ShowSpeed = 10;
        }
        suspend = false;
        //\u3000为中文空格英文空格会引起unity中Text的自动换行因此将内容中的英文空格换成中文空格
        string str = m_Text;//.Replace(" ", "\u3000");
        string LineHead = "";
        //设置段落的首行缩进的字符数
        if (m_TextIndent != 0)
        {
            for (int i = 0; i < m_TextIndent; i++)
            {
                LineHead += "\u3000";
            }
            str = str.Replace("\n", "\n" + LineHead);
            str = LineHead + str;
        }
        //将转换好的文本转换成Char数组以便于逐字读写
        m_Text_Char = str.ToCharArray();
        //当音频不为空时，处理音频的播放
        if (m_AudioClip != null)
        {
            if (m_AudioSource == null)
                m_AudioSource = this.GetComponent<AudioSource>();
            if (m_AudioSource == null)
                m_AudioSource = this.gameObject.AddComponent<AudioSource>();
            m_AudioSource.clip = m_AudioClip;
            //读写速度根据音频长度平均计算
            m_ShowSpeed = str.Length / m_AudioClip.length;
        }
        StopCoroutine("Player");
        StartCoroutine("Player");
    }
    /// <summary>
    /// 播放无语音
    /// </summary>
    /// <param name="varCentent">播放的内容</param>
    public void Play(string varCentent)
    {
        m_Text = varCentent;
        m_AudioClip = null;
        if (m_Conetnt == null) m_Conetnt = this.GetComponent<Text>();
        if (!enabled || !this.gameObject.activeSelf)
        {
            m_Enable = true;
            return;
        }
        Play();
    }
    /// <summary>
    /// 播放有语音
    /// </summary>
    /// <param name="varCentent">播放的内容</param>
    /// <param name="audio">跟踪的语言</param>
    public void Play(string varCentent, AudioClip audio)
    {
        m_Text = varCentent;
        m_AudioClip = audio;
        if(m_Conetnt == null) m_Conetnt = this.GetComponent<Text>();
        if (!enabled || !this.gameObject.activeSelf)
        {
            m_Enable = true;
            return;
        }
        Play();
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    public void Pause()
    {
        suspend = true;
    }
    /// <summary>
    /// 恢复播放
    /// </summary>
    public void Recovery()
    {
        suspend = false;
    }

    /// <summary>
    /// 停止播放（无法继续）
    /// </summary>
    public void Stop()
    {
        StopCoroutine("Player");
    }


    IEnumerator Player()
    {
        float idx = 0;
        if (m_Conetnt == null) m_Conetnt = this.GetComponent<Text>();
        m_Conetnt.text = "";
        yield return 0;
        if (m_AudioSource != null)
            m_AudioSource.Play();
        RectTransform TempCententRect = null;
        RectTransform TempScrollRect = null;
        if (m_ScrollRect != null)
        {
            TempCententRect = m_ScrollRect.content.GetComponent<RectTransform>();
            TempScrollRect = m_ScrollRect.GetComponent<RectTransform>();
        }
        while (idx <= m_Text_Char.Length)
        {
            //暂停处理
            if (suspend)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }
            float TempTimes = Time.fixedDeltaTime;
            float currfont = TempTimes * m_ShowSpeed;
            //更新应显示的字的数量
            idx += currfont;
            string currcentent = "";
            //获取显示的字的内容
            for (int i = 0; i < m_Text_Char.Length; i++)
            {
                if (i <= idx)
                    currcentent += m_Text_Char[i];
                //过滤换行字符
                else if (m_Text_Char[i] == '\n')
                    idx += 1;
                //过滤空格
                else if (m_Text_Char[i] == ' ' || m_Text_Char[i] == '\u3000')
                    idx += 1;
                else
                    break;
            }
            m_Conetnt.text = currcentent;
            //当有ScrollRect时让文字始终显示刷新文字的地方
            if (m_ScrollRect != null && m_Conetnt.preferredHeight >= TempCententRect.sizeDelta.y)
            {
                //更新滑动区域大小使之与文本框大小相同（仅高度相同）
                TempCententRect.sizeDelta = new Vector2(TempCententRect.sizeDelta.x, m_Conetnt.preferredHeight);
                //当滑动高度大于ScrollRect的显示高度时保证滑动区域始终在最下方
                if (TempCententRect.sizeDelta.y > TempScrollRect.sizeDelta.y)
                {
                    if (m_ScrollRect.verticalScrollbar != null)
                        m_ScrollRect.verticalScrollbar.value = 0;
                }
            }
            //与FixedUpdate同步避免程序卡死
            yield return new WaitForFixedUpdate();
        }
        //当音频不为空时处理音频
        if (m_AudioSource != null)
        {
            //当音频未播放完成时不结束进程
            while (m_AudioSource.isPlaying)
            {
                //与FixedUpdate同步避免程序卡死
                yield return new WaitForFixedUpdate();
            }
        }
        //音频播放完成清理音频信息
        if (m_AudioSource != null)
            Destroy(m_AudioSource);
        //等待一帧以便以上信息运行完成
        yield return new WaitForSeconds(endYieldTime);
        m_AudioSource = null;
        //执行播放完成回调
        m_CallBack?.Invoke();
    }

    [Serializable]
    public class CallBack : UnityEvent
    {
        public CallBack()
        {
        }
    }
}



