using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class MainDialogueWnd : UIBase
{
    public RectTransform talkParent;
    public RectTransform taskNameParent;

    public Image talkIcon;
    public Text talkName;
    public Text talkContent;

    public Text taskName;
    public Button next;

    TaskData taskData;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.MainDialogueWnd;
        MutexInterface = false;
    }

    protected override void Start()
    {
        base.Start();
        next.onClick.AddListener(OnNext);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Show();
    }

    void Show()
    {
        taskData = SongsDataMng.GetInstance().GetTaskData;
        if(taskData == null)
        {
            UIMng.Instance.ConcealUI(this.Type);
            return;
        }
        bool talking = false;
        taskName.text = taskData.name;
        switch (taskData.type)
        {
            case TaskType.OpenWnd:
                UIType type = (UIType)System.Enum.Parse(typeof(UIType), taskData.val);
                UIMng.Instance.OpenUI(type);
                break;
            case TaskType.Click:
                break;
            case TaskType.Move:
                break;
            case TaskType.Talk:
                ModelData modelData = SongsDataMng.GetInstance().GetModelData(taskData.val);
                talkName.text = modelData.name;
                Sprite obj = Resources.Load<Sprite>("Sprites/" + modelData.icon);
                if (obj != null)
                {
                    talkIcon.sprite = obj;
                }
                talking = true;
                break;
        }
        Show(taskData.des);
        talkParent.gameObject.SetActive(talking);
        taskNameParent.gameObject.SetActive(!talking);
    }

    void Show(string content)
    {
        talkContent.text = content;
    }

    void OnNext()
    {
        if(taskData.next != 0)
        {
            SongsDataMng.GetInstance().SetNextTaskData(taskData);
        }
    }
}
