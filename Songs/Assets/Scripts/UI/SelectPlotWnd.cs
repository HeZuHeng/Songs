using MREngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Songs;

public class SelectPlotWnd : UIBase
{
    public UI_Control_ScrollFlow scrollFlow;
    public Button confirmBtn;
    public Button lastBtn;
    public Button nextBtn;

    public Button returnBtn;

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.SelectPlotWnd;
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
        returnBtn.gameObject.SetActive(SongsDataMng.GetInstance().GetSceneTaskData != null);
        Show();
    }

    void Show()
    {
        Transform parent = scrollFlow.transform;
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }

        TasksConfig tasksConfig = SongsDataMng.GetInstance().GetTasksConfig;
        Transform tran = null;
        int child = parent.childCount;
        for (int i = 0; i < tasksConfig.datas.Count; i++)
        {
            if(child > i)
             {
                tran = parent.GetChild(i);
            }
            else
            {
                tran = Instantiate(tran);
                tran.SetParent(parent);
                tran.localPosition = Vector3.zero;
                tran.localScale = Vector3.one;
            }
            tran.GetComponent<PlotItemUI>().Show(tasksConfig.datas[i]);
            tran.gameObject.SetActive(true);
        }
        scrollFlow.Refresh();
    }

    void OnConfirm()
    {
        PlotItemUI plotItemUI = scrollFlow.Current.GetComponent<PlotItemUI>();
        if(plotItemUI != null)
        {
            SongsDataMng.GetInstance().SetSceneTaskData(plotItemUI.sceneTaskData);
        }
        UIMng.Instance.OpenUI(UIType.LoadingWnd);
    }

    void OnLast()
    {

    }

    void OnNext()
    {

    }

    void OnReturn()
    {
        UIMng.Instance.OpenUI(UIType.NONE);
    }
}
