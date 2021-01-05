using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;
using Coffee.UIEffects;

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
    UIShiny taskNameParentShiny;
    Animator taskNameParentAnimator;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.MainDialogueWnd;
        MutexInterface = false;
        taskNameParentShiny = taskNameParent.GetComponentInChildren<UIShiny>();
        taskNameParentAnimator = taskNameParent.GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        next.onClick.AddListener(OnRun);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Show();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (taskData != null)
        {
            if (taskData.type == TaskType.Talk)
            {
                int talkTargetId = 0;
                int.TryParse(taskData.val, out talkTargetId);
                SceneAssetObject sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(talkTargetId);
                if (sceneAsset != null)
                {
                    sceneAsset.PlayAnimator("talk", false, 1, null);
                }
            }
            taskData.onStateChange.RemoveListener(OnStateChange);
        }
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
        talkParent.gameObject.SetActive(false);
        taskNameParent.gameObject.SetActive(false);
        next.gameObject.SetActive(false);

        taskNameParentShiny.enabled = false;
        taskNameParentAnimator.enabled = false;

        taskData.onStateChange.RemoveListener(OnStateChange);
        taskData.onStateChange.AddListener(OnStateChange);
        if (taskData.TaskState == TaskState.Start) taskData.TaskState = TaskState.Run;
        talkIcon.enabled = false;

        int talking = 0;
        string talkId = "0";
        taskName.text = taskData.name;
        switch (taskData.type)
        {
            case TaskType.OpenWnd:
                UIType type = (UIType)System.Enum.Parse(typeof(UIType), taskData.val);
                UIMng.Instance.OpenUI(type);
                break;
            case TaskType.Click:

                break;
            case TaskType.TaskChange:
                taskNameParentShiny.enabled = true;
                taskNameParentAnimator.enabled = true;
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
                if(sceneAsset == null)
                {
                    taskData.TaskState = TaskState.End;
                    return;
                }
                if((AnimatorType)aType == AnimatorType.BOOl)
                {
                    bool b = false;
                    bool.TryParse(strs[3], out b);
                    bool loop = sceneAsset.PlayAnimator(strs[4], b, speed,delegate(string aName)
                    {
                        if (aName.Equals(strs[4]))
                        {
                            taskData.TaskState = TaskState.End;
                        }
                        if(strs.Length >= 6)
                        {
                            sceneAsset.PlayAnimator(strs[4], !b, speed, null);
                        }
                    });
                    SceneController.GetInstance().AddPlayAnimator(sceneAsset);
                }
                else if ((AnimatorType)aType == AnimatorType.FLOAT)
                {
                    float f = 0;
                    float.TryParse(strs[3], out f);
                    bool loop = sceneAsset.PlayAnimator(strs[4], f, speed, delegate (string aName)
                    {
                        if (aName.Equals(strs[4]))
                        {
                            taskData.TaskState = TaskState.End;
                        }
                        if (strs.Length >= 6)
                        {
                            sceneAsset.PlayAnimator(strs[4], 1 - f, speed, null);
                            taskData.TaskState = TaskState.End;
                        }
                    });
                    SceneController.GetInstance().AddPlayAnimator(sceneAsset);
                }
                break;
            case TaskType.Question:
                int qId = 0;
                int.TryParse(taskData.val,out qId);
                SongsDataMng.GetInstance().SetQuestion(qId);
                UIMng.Instance.ActivationUI(UIType.AnswerWnd);
                break;
            case TaskType.QATalk:
                int qaId = 0;
                int.TryParse(taskData.val, out qaId);
                SongsDataMng.GetInstance().SetQuestion(qaId);
                UIMng.Instance.OpenUI(UIType.QATalkWnd);
                break;
            case TaskType.StartState:
                State state = (State)System.Enum.Parse(typeof(State), taskData.val);
                SceneController.GetInstance().ToState(state,delegate(State end) { 
                    if(state == end)
                    {
                        taskData.TaskState = TaskState.End;
                    }
                });
                break;
            case TaskType.Move:
                break;
            case TaskType.LookSong:
                SongsDataMng.GetInstance().GetSongFilePath = taskData.val;
                UIMng.Instance.ActivationUI(UIType.LeftDialogueWnd);
                break;
            case TaskType.DOTween:
                if(Application.platform == RuntimePlatform.WindowsEditor)
                {
                    taskData.TaskState = TaskState.End;
                }
                else
                {
                    CameraMng.GetInstance().DOTweenPaly(delegate ()
                    {
                        taskData.TaskState = TaskState.End;
                    });
                }
                break;
            case TaskType.PersonMove:
                CameraMng.GetInstance().SetPersonMove();
                if ("end".Equals(taskData.val))
                {
                    taskData.TaskState = TaskState.End;
                }
                break;
            case TaskType.GodRoams:
                CameraMng.GetInstance().SetGodRoamsMove();
                if ("end".Equals(taskData.val))
                {
                    taskData.TaskState = TaskState.End;
                }
                break;
            case TaskType.ThirdPerson:
                CameraMng.GetInstance().SetThirdPersonMove();
                if ("end".Equals(taskData.val))
                {
                    taskData.TaskState = TaskState.End;
                }
                break;
            case TaskType.FirstPerson:
                CameraMng.GetInstance().SetFirstPersonMove();
                if ("end".Equals(taskData.val))
                {
                    taskData.TaskState = TaskState.End;
                }
                break;
            case TaskType.Talk:
                talkId = taskData.val;
                break;
        }

        talking = !string.IsNullOrEmpty(taskData.des) ? 1 : 0;

        talkParent.gameObject.SetActive(talking == 1);
        taskNameParent.gameObject.SetActive(talking == 0);
        if (talking == 1 && !string.IsNullOrEmpty(taskData.des))
        {
            ModelData modelData = SongsDataMng.GetInstance().GetModelData(talkId);
            string iconN = string.Empty;
            if (modelData != null)
            {
                int talkTargetId = 0;
                int.TryParse(talkId,out talkTargetId);
                SceneAssetObject sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(talkTargetId);
                if(sceneAsset != null)
                {
                    sceneAsset.PlayAnimator("talk", true, 1, null);
                }
                talkName.text = modelData.name;
                iconN = modelData.icon;
            }
            else
            {
                talkName.text = "系统提示";
                iconN = string.Empty;
                //SceneAssetObject sceneAssetObject = SceneMng.GetInstance().GetSceneAssetObject(1);
                //if ("nyk".Equals(sceneAssetObject.URL))
                //{
                //    iconN = "nyk";
                //}
                //else
                //{
                //    iconN = "nvyk";
                //}
            }
            if (!string.IsNullOrEmpty(iconN)) {
                Sprite obj = Resources.Load<Sprite>("Sprites/PlayerIcon/" + iconN);
                if (obj != null)
                {
                    talkIcon.sprite = obj;
                }
                talkIcon.enabled = true;
            }
            else
            {
                talkIcon.enabled = false;
            }

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
        if (taskData.type == TaskType.Talk && taskData != null && taskData.next != 0)
        {
            int talkTargetId = 0;
            int.TryParse(taskData.val, out talkTargetId);
            SceneAssetObject sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(talkTargetId);
            if (sceneAsset != null)
            {
                sceneAsset.PlayAnimator("talk", false, 1, null);
            }
            next.gameObject.SetActive(true);
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
            taskData.onStateChange.RemoveListener(OnStateChange);
            SongsDataMng.GetInstance().SetNextTaskData(taskData);
            Show();
        }
    }

    void OnRun()
    {
        taskData.TaskState = TaskState.End;
    }

    public void OnTaskChangeClick()
    {
        if(taskData != null && taskData.type == TaskType.TaskChange)
        {
            SceneTaskData sceneTask = SongsDataMng.GetInstance().GetSceneTaskDataFromName(taskData.val);
            SongsDataMng.GetInstance().SetSceneTaskData(sceneTask);
            UIMng.Instance.ActivationUI(UIType.LoadingWnd);
        }
    }
}
