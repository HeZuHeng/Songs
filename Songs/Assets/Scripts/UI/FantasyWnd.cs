using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;
using DG.Tweening;

public class FantasyWnd : UIBase
{
    static string[] Tips =
    {
        "In contrast to the crowds in the object, the author’s loneliness and frustration are reflected.（与客体中的成群结队形成对比，体现作者的孤单、沮丧。）",
        "A group of brisk and lively images contrast with the poet’s melancholy.（一群舞姿轻快、活泼的意象，与诗人的忧郁惆怅形成对比。）",
        "Shining bright, lighting up the world and driving away the poet’s inner haze.（闪耀璀璨，照亮了世界并驱赶了诗人内心的阴霾。）",
        "A path that enables poet to a happy paradise and discover himself.（一条使诗人能够通往快乐的天堂、发现自我的道路。）",
        "even if aroused by the wind, it doesn’t dance as cheerfully and delightfully as daffodils do.（即使被风撩拨，它也远不如水仙跳舞跳得欢快、热烈。）"
    };

    static string[] Names =
    {
        "一朵云","成片且摇曳的水仙花","漫天闪烁的星星","银河","粼粼水波"
    };
    public ScrollRect scrollRect;
    public Text tip;

    int index = 0;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.FantasyWnd;
        MutexInterface = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        index = 0;
        Show();
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            scrollRect.content.GetChild(i).localScale = Vector3.one;
            scrollRect.content.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void OnCilck(Text game)
    {
        if (game.text.Equals(Names[index]))
        {
            Tweener tweener = game.transform.parent.DOScale(Vector3.one * 0.1f, 1);
            tweener.onComplete += delegate ()
            {
                tweener.onComplete = null;
                game.transform.parent.gameObject.SetActive(false);
                index++;
                
                if(index == 2)
                {
                    SongsDataMng.GetInstance().SetQuestion(1);
                    UIMng.Instance.ActivationUI(UIType.AnswerWnd);
                }
                if (index == 4)
                {
                    SongsDataMng.GetInstance().SetQuestion(2);
                    UIMng.Instance.ActivationUI(UIType.AnswerWnd);
                }
                if (index == 5)
                {
                    QuestionBankData bankData = SongsDataMng.GetInstance().SetQuestion(3);
                    bankData.onQuestionEnd.AddListener(Close);
                    UIMng.Instance.ActivationUI(UIType.AnswerWnd);
                }
                
                Show();
            };
        }
        else
        {
            tip.transform.parent.DOShakeRotation(2, new Vector3(10, 7, 0));
            tip.transform.parent.DOShakePosition(2, new Vector3(5, 6, 0));
        }
    }

    void Show()
    {
        if (index >= Names.Length)
        {
            tip.transform.parent.gameObject.SetActive(false);
            return;
        }
        tip.transform.parent.gameObject.SetActive(true);
        tip.text = Tips[index];
    }

    void Close()
    {
        UIMng.Instance.OpenUI(UIType.NONE);
    }
}
