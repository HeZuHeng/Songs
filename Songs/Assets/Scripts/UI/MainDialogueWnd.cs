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
    public TrendsText talkContent;

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

    protected override void OnDisable()
    {
        base.OnDisable();
       if(taskData != null) taskData.onStateChange.RemoveListener(OnStateChange);
        talkContent.gameObject.SetActive(false);
        talkContent.m_Enable = false;
    }

    void Show()
    {
        taskData = SongsDataMng.GetInstance().GetTaskData;
        if (taskData == null)
        {
            UIMng.Instance.ConcealUI(this.Type);
            return;
        }
        taskData.onStateChange.RemoveListener(OnStateChange);
        taskData.onStateChange.AddListener(OnStateChange);
        if (taskData.TaskState == TaskState.Start) taskData.TaskState = TaskState.Run;

        int talking = 0;
        taskName.text = taskData.name;
        switch (taskData.type)
        {
            case TaskType.OpenWnd:
                UIType type = (UIType)System.Enum.Parse(typeof(UIType), taskData.val);
                UIMng.Instance.OpenUI(type);
                break;
            case TaskType.Click:
                break;
            case TaskType.Animator:
                string[] strs = taskData.val.Split('|');
                int id = 0;
                int.TryParse(strs[0],out id);
                int aType = 0;
                int.TryParse(strs[1], out aType);
                int speed = 0;
                int.TryParse(strs[2], out speed);
                SceneAssetObject sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(id);
                if((AnimatorType)aType == AnimatorType.BOOl)
                {
                    bool b = false;
                    bool.TryParse(strs[3], out b);
                    bool loop = sceneAsset.PlayAnimator(strs[4], b, speed,delegate(string aName)
                    {
                        if(strs.Length >= 6 && aName.Equals(strs[4]))
                        {
                            sceneAsset.PlayAnimator(strs[4], !b, speed, null);
                            taskData.TaskState = TaskState.End;
                        }
                    });
                    if (!loop)
                    {
                        SceneController.GetInstance().AddPlayAnimator(sceneAsset);
                    }
                }
                else if ((AnimatorType)aType == AnimatorType.FLOAT)
                {
                    float f = 0;
                    float.TryParse(strs[3], out f);
                    bool loop = sceneAsset.PlayAnimator(strs[4], f, speed, delegate (string aName)
                    {
                        if (strs.Length >= 6 && aName.Equals(strs[4]))
                        {
                            sceneAsset.PlayAnimator(strs[4], 1 - f, speed, null);
                            taskData.TaskState = TaskState.End;
                        }
                    });
                    if (!loop)
                    {
                        SceneController.GetInstance().AddPlayAnimator(sceneAsset);
                    }
                }
                talking = 1;
                break;
            case TaskType.Question:
                int qId = 0;
                int.TryParse(taskData.val,out qId);
                SongsDataMng.GetInstance().SetQuestion(qId);
                UIMng.Instance.ActivationUI(UIType.AnswerWnd);
                break;
            case TaskType.Move:
                break;
            case TaskType.LookSong:
                SongsDataMng.GetInstance().GetSongFilePath = taskData.val;
                UIMng.Instance.ActivationUI(UIType.LeftDialogueWnd);
                break;
            case TaskType.DOTween:
                CameraMng.GetInstance().DOTweenPaly(delegate()
                {
                    taskData.TaskState = TaskState.End;
                });
                break;
            case TaskType.GodRoams:
                CameraMng.GetInstance().SetGodRoamsMove();
                break;
            case TaskType.ThirdPerson:
                CameraMng.GetInstance().SetThirdPersonMove();
                break;
            case TaskType.FirstPerson:
                CameraMng.GetInstance().SetFirstPersonMove();
                break;
            case TaskType.Talk:
                ModelData modelData = SongsDataMng.GetInstance().GetModelData(taskData.val);
                if(modelData != null)
                {
                    talkName.text = modelData.name;
                    Sprite obj = Resources.Load<Sprite>("Sprites/" + modelData.icon);
                    if (obj != null)
                    {
                        talkIcon.sprite = obj;
                    }
                    talkIcon.enabled = true;
                }
                else
                {
                    talkIcon.enabled = false;
                    talkName.text = "系统";
                }
                talking = 1;
                break;
        }
        talkParent.gameObject.SetActive(talking == 1);
        taskNameParent.gameObject.SetActive(talking == 0);
        if (talking == 1)
        {
            Show(taskData.des);
        }
    }

    void Show(string content)
    {
        if (string.IsNullOrEmpty(content)) return;
        talkContent.m_CallBack.RemoveListener(OnNext);
        talkContent.m_CallBack.AddListener(OnNext);
        talkContent.gameObject.SetActive(true);
        talkContent.Play(content.Replace("\\n", "\n"));
    }

    void OnNext()
    {
        if (taskData != null && taskData.next != 0)
        {
            taskData.onStateChange.RemoveListener(OnStateChange);
            SongsDataMng.GetInstance().SetNextTaskData(taskData);
            Show();
        }
        else
        {
            talkParent.gameObject.SetActive(false);
            taskNameParent.gameObject.SetActive(true);
        }
    }

    void OnStateChange(TaskState taskState)
    {
        if(taskState == TaskState.End)
        {
            OnNext();
        }
    }
}
