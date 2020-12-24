using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FantasyCompareWnd : UIBase
{
    public RectTransform rectJh;
    public RectTransform rectSx;

    public RectTransform textJh;
    public RectTransform textSx;

    RectTransform rect;
    Vector3 initJh;
    Vector3 initSx;

    bool jh = false;
    bool sx = false;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.FantasyCompareWnd;
        MutexInterface = true;
        rect = transform as RectTransform;
        initJh = textJh.position;
        initSx = textSx.position;
    }

    protected override void OnEnable()
    {
         jh = false;
         sx = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
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

    public void OnDragJh()
    {
        Vector2 pos = textJh.anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out pos);
        textJh.anchoredPosition = pos;
        
    }
    public void OnDragJhEnd()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(rectJh, Input.mousePosition))
        {
            rectJh.DOShakeScale(0.2f, Vector3.one * 0.8f);
            Tween tween = textJh.DOMove(initJh, 0.2f);
            tween.onComplete = delegate ()
            {
                textJh.gameObject.SetActive(false);
            };
        }
        else
        {
            jh = true;
            if(jh && sx)
            {
                UIMng.Instance.OpenUI(UIType.NONE);
            }
        }
    }

    public void OnDragSx()
    {
        Vector2 pos = textSx.anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out pos);
        textSx.anchoredPosition = pos;
    }
    public void OnDragSxEnd()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(rectSx, Input.mousePosition))
        {
            rectSx.DOShakeScale(0.2f, Vector3.one * 0.8f);
            Tween tween = textSx.DOMove(initSx, 0.2f);
            tween.onComplete = delegate ()
            {
                textSx.gameObject.SetActive(false);
            };
        }
        else
        {
            sx = true;
            if (jh && sx)
            {
                UIMng.Instance.OpenUI(UIType.NONE);
            }
        }
    }
}
