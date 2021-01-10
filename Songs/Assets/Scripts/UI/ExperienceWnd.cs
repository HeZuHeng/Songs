using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class ExperienceWnd : UIBase
{
    public Text[] texts;

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
        index = 0;
        ExperienceUtil.Instance.SetTexture(textures[index]);
        //CancelInvoke("DoMove");
        Invoke("DoMove", 1);

        index1 = 0;
        texts[index1].enabled = true;
        CancelInvoke("ShowText");
        InvokeRepeating("ShowText",3,3);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ExperienceUtil.Instance.InitPosition();
    }

    void ShowText()
    {
        texts[index1].enabled = false;
        index1++;

        texts[index1].enabled = true;
        if (index1 >= texts.Length - 1)
        {
            CancelInvoke("ShowText");
        }
    }

    void DoMove()
    {
        ExperienceUtil.Instance.DoMoveCamera(textures[index], new Vector3(1, 2, -4), 7.5f, delegate () {
            index++;
            ExperienceUtil.Instance.DoMoveCamera(textures[index], new Vector3(-1, 1, -6), 7.5f, delegate () {
                OnClose();
            });
        });
    }

    public void OnClose()
    {
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
