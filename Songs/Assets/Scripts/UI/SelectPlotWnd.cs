using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class SelectPlotWnd : UIBase
{
    public EnhanceScrollView scrollFlow;
    public Button confirmBtn;
    public Button lastBtn;
    public Button nextBtn;
    public Text taskName;

    public Button returnBtn;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.SelectPlotWnd;
        MutexInterface = false;
        
    }

    protected override void Start()
    {
        base.Start();
        confirmBtn.onClick.AddListener(OnConfirm);
        lastBtn.onClick.AddListener(OnLast);
        nextBtn.onClick.AddListener(OnNext);
        returnBtn.onClick.AddListener(OnReturn);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //returnBtn.gameObject.SetActive(SongsDataMng.GetInstance().GetSceneTaskData != null);
        transform.SetAsLastSibling();
        Show();
    }

    void Show()
    {
        Transform parent = scrollFlow.transform.GetChild(0);
        for (int i = 0; i < parent.childCount; )
        {
            if(i == 0){
                parent.GetChild(i).gameObject.SetActive(false);
                i++;
            }
            else
            {
                DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }

        TasksConfig tasksConfig = SongsDataMng.GetInstance().GetTasksConfig;
        if (tasksConfig == null) return;
        taskName.text = tasksConfig.name;
        SceneTaskData sceneTaskData = SongsDataMng.GetInstance().GetSceneTaskData;
        Transform tran = null;
        int child = parent.childCount;
        int curId = 0;
        for (int i = 0; i < tasksConfig.datas.Count; i++)
        {
            if(child > i)
             {
                tran = parent.GetChild(i);
            }
            else
            {
                tran = Instantiate(tran);
                tran.name = "Item"+ i;
                tran.SetParent(parent);
                tran.localPosition = Vector3.zero;
                tran.localScale = Vector3.one;
            }
            if (sceneTaskData != null && sceneTaskData.name.Equals(tasksConfig.datas[i].name))
            {
                curId = i + 1;
            }
            tran.GetComponent<PlotItemUI>().Show(tasksConfig.datas[i]);
            tran.gameObject.SetActive(true);
        }
        //scrollFlow.startCenterIndex = curId;
        scrollFlow.Refrshed();
        SetItem(curId);
    }

    void SetItem(int curId)
    {
        if(curId <0 || curId >= scrollFlow.listEnhanceItems.Count)
        {
            curId = 0;
        }
        scrollFlow.SetHorizontalTargetItemIndex(scrollFlow.listEnhanceItems[curId]);
    }

    void OnConfirm()
    {
        PlotItemUI plotItemUI = scrollFlow.curCenterItem.GetComponent<PlotItemUI>();
        if (SongsDataMng.GetInstance().GetSceneTaskData != plotItemUI.sceneTaskData)
        {
            SongsDataMng.GetInstance().SetSceneTaskData(plotItemUI.sceneTaskData);
            UIMng.Instance.ActivationUI(UIType.LoadingWnd);
        }
        else
        {
            UIMng.Instance.ConcealUI(UIType.SelectPlotWnd);
        }
    }

    void OnLast()
    {
        scrollFlow.OnBtnLeftClick();
    }

    void OnNext()
    {
        scrollFlow.OnBtnRightClick();
    }

    void OnReturn()
    {
        UIMng.Instance.ConcealUI(UIType.SelectPlotWnd);
        UIMng.Instance.OpenUI(UIType.IntroductionWnd);
    }
}
