using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HTMMapWnd : UIBase
{
    public GameObject page_map;
    public GameObject Image_map_start;

    public Button Button_next;
    public Button Button_start;


    protected override void Awake()
    {
        base.Awake();
        Type = UIType.HTMMapWnd;
        MutexInterface = true;
        Initialized();
    }

    private void Initialized()
    {
        if (page_map == null) page_map = UIUtility.Control("page_map", gameObject);
        if (Image_map_start == null) Image_map_start = UIUtility.Control("Image_map_start", gameObject);
        if(Button_next != null) Button_next.onClick.AddListener(OnClickNext);
        if (Button_start != null) Button_start.onClick.AddListener(OnClickStart);
    }



    protected override void OnEnable()
    {
        base.OnEnable();
        ShowPage(0);
    }

    public void ShowPage(int pagenum)
    {
        if (pagenum == 0)
        {
            if (page_map != null) page_map.SetActive(true);
            if (Image_map_start != null) Image_map_start.SetActive(false);
        }
        else if (pagenum == 1)
        {
            if (page_map != null) page_map.SetActive(false);
            if (Image_map_start != null) Image_map_start.SetActive(true);
        }
        else
            Debug.LogError("未定义页面序号");


    }





    public void OnClickNext()
    {
        //to do:切换界面
        Debug.Log("切换界面");
        ShowPage(1);
    }

    public void OnClickStart()
    {
        //to do:下一步起航
        Debug.Log("下一步起航");
    }

    public void OnEnd()
    {
       
    }

   
}
