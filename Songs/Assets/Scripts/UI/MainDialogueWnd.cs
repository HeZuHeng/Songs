using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;
using Coffee.UIEffects;
using BuildUtil;
using LaoZiCloudSDK.CameraHelper;

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
    public Image jiantou;

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

        InputManager.GetInstance().RemoveClickEventListener(OnClickEvent);
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
        jiantou.enabled = false;

        taskData.onStateChange.RemoveListener(OnStateChange);
        taskData.onStateChange.AddListener(OnStateChange);
        if (taskData.TaskState == TaskState.Start) taskData.TaskState = TaskState.Run;
        talkIcon.enabled = false;

        string talkId = "0";
        taskName.text = taskData.name;
        switch (taskData.type)
        {
            case TaskType.OpenWnd:
                UIType type = (UIType)System.Enum.Parse(typeof(UIType), taskData.val);
                UIMng.Instance.OpenUI(type);
                break;
            case TaskType.Click:
                Transform clikcObj = SceneController.TerrainController.transform.Find(taskData.val);
                if (clikcObj != null)
                {
                    MeshRenderer[] meshes = clikcObj.GetComponentsInChildren<MeshRenderer>();
                    for (int i = 0; i < meshes.Length; i++)
                    {
                        meshes[i].gameObject.AddComponent<HighlightableObject>();
                    }
                    HighlightableObjectHelper objectHelper = clikcObj.GetComponent<HighlightableObjectHelper>();
                    if (objectHelper == null) objectHelper = clikcObj.gameObject.AddComponent<HighlightableObjectHelper>();
                    objectHelper.enabled = true;
                    InputManager.GetInstance().RemoveClickEventListener(OnClickEvent);
                    InputManager.GetInstance().AddClickEventListener(OnClickEvent);
                }
                else
                {
                    taskData.TaskState = TaskState.End;
                }
                break;
            case TaskType.TaskChange:
                taskNameParentShiny.enabled = true;
                taskNameParentAnimator.enabled = true;
                jiantou.enabled = true;
                break;
            case TaskType.Animator:
                string[] strs = taskData.val.Split('|');
                int id = 0;
                int.TryParse(strs[0], out id);
                int aType = 0;
                int.TryParse(strs[1], out aType);
                int speed = 0;
                int.TryParse(strs[2], out speed);
                SceneAssetObject sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(id);
                if (sceneAsset == null)
                {
                    taskData.TaskState = TaskState.End;
                    return;
                }
                if ((AnimatorType)aType == AnimatorType.BOOl)
                {
                    bool b = false;
                    bool.TryParse(strs[3], out b);
                    int soundId = -1;
                    if (!string.IsNullOrEmpty(taskData.sound))
                    {
                        int.TryParse(taskData.sound, out soundId);
                        AudioManager.Instance.PlaySound(soundId);
                    }
                    bool loop = sceneAsset.PlayAnimator(strs[4], b, speed, delegate (string aName)
                     {
                         if (aName.Equals(strs[4]) && string.IsNullOrEmpty(taskData.des))
                         {
                             taskData.TaskState = TaskState.End;
                         }
                         if (strs.Length >= 6)
                         {
                             sceneAsset.PlayAnimator(strs[4], !b, speed, null);
                         }
                         if (soundId >0)
                         {
                             AudioManager.Instance.ChangeSoundVolume(0);
                         }
                     });
                    SceneController.GetInstance().AddPlayAnimator(sceneAsset);
                }
                else if ((AnimatorType)aType == AnimatorType.FLOAT)
                {
                    float f = 0;
                    float.TryParse(strs[3], out f);
                    int soundId = -1;
                    if (!string.IsNullOrEmpty(taskData.sound))
                    {
                        int.TryParse(taskData.sound, out soundId);
                        AudioManager.Instance.PlaySound(soundId);
                    }
                    bool loop = sceneAsset.PlayAnimator(strs[4], f, speed, delegate (string aName)
                    {
                        if (aName.Equals(strs[4]) && string.IsNullOrEmpty(taskData.des))
                        {
                            taskData.TaskState = TaskState.End;
                        }
                        if (strs.Length >= 6)
                        {
                            sceneAsset.PlayAnimator(strs[4], 1 - f, speed, null);
                        }
                        if (soundId > 0)
                        {
                            AudioManager.Instance.ChangeSoundVolume(0);
                        }
                    });
                    SceneController.GetInstance().AddPlayAnimator(sceneAsset);
                }
                break;
            case TaskType.Question:
                int qId = 0;
                int.TryParse(taskData.val, out qId);
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
                SceneController.GetInstance().ToState(state, delegate (State end) {
                    if (state == end)
                    {
                        taskData.TaskState = TaskState.End;
                    }
                });
                break;
            case TaskType.Move:
                break;
            case TaskType.LookSong:
                string[] songStrs = taskData.val.Split('|');
                SongsDataMng.GetInstance().GetSongFilePath = songStrs[0];
                SongsDataMng.GetInstance().GetSongSoundPath = taskData.sound;
                UIMng.Instance.ConcealUI(UIType.LeftDialogueWnd);
                UIMng.Instance.ActivationUI(UIType.LeftDialogueWnd);
                if (songStrs.Length > 1)
                {
                    taskData.TaskState = TaskState.End;
                    return;
                }
                break;
            case TaskType.DOTween:
                if (!string.IsNullOrEmpty(taskData.val) && taskData.val.Equals("end"))
                {
                    CameraMng.GetInstance().DOTweenPaly(null);
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
        int talking = !string.IsNullOrEmpty(taskData.des) ? 1 : 0;

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
                    sceneAsset.PlayAnimator("talk", false, 1, null);
                    sceneAsset.PlayAnimator("talk", true, 1, null);
                }
                talkName.text = modelData.name;
                iconN = string.IsNullOrEmpty(modelData.icon) ? modelData.assetName : modelData.icon;
            }
            else
            {
                talkName.text = "系统提示";
                iconN = string.Empty;
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

            Show(taskData.des, taskData.sound);
        }
    }

    void Show(string content,string soundName)
    {
        if (string.IsNullOrEmpty(content)) return;
        content = string.Format(content, SongsDataMng.GetInstance().Player.name);
        talkContent.m_CallBack.RemoveListener(OnNext);
        talkContent.m_CallBack.AddListener(OnNext);
        talkContent.gameObject.SetActive(true);
        talkContent.Play(content.Replace("\\n", "\n"), soundName);
    }

    void OnNext()
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
            if (taskData.type != TaskType.OpenWnd) UIMng.Instance.ConcealUI(UIType.LookSongWnd);
            if (taskData.type == TaskType.Talk)
            {
                UIMng.Instance.ConcealUI(UIType.LeftDialogueWnd);
            }
            if(taskData.next == 0)
            {
                talkParent.gameObject.SetActive(false);
                taskNameParent.gameObject.SetActive(true);
                return;
            }
            taskData.onStateChange.RemoveListener(OnStateChange);
            if (taskData.name.Equals("超验主义"))
            {
                MainPlayer.songResultInfo.FillAnswer(15, string.Empty, 1, AnswerType.Operating);
            }
            if (taskData.name.Equals("惠特曼的旁白"))
            {
                MainPlayer.songResultInfo.FillAnswer(16, string.Empty, 1, AnswerType.Operating);
            }
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
            UIMng.Instance.ActivationUI(UIType.SelectPlotWnd);
            if (sceneTask != null) SongsDataMng.GetInstance().SetSceneTaskData(sceneTask);
            UIMng.Instance.ActivationUI(UIType.LoadingWnd);
        }
    }

    void OnClickEvent(GameObject obj)
    {
        if (!string.IsNullOrEmpty(taskData.val) && taskData.val.Contains(obj.name))
        {
            HighlightableObjectHelper objectHelper = obj.GetComponent<HighlightableObjectHelper>();
            objectHelper.enabled = false;
            InputManager.GetInstance().RemoveClickEventListener(OnClickEvent);

            if (obj.name.Equals("菊花瓶"))
            {
                MainPlayer.songResultInfo.FillAnswer(5, string.Empty, 1, AnswerType.Operating);
            }
            taskData.TaskState = TaskState.End;
        }
    }
}
