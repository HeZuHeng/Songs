using Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryWnd : UIBase
{
    public Slider slider;
    public Image image;
    public Text text;
    protected override void Awake()
    {
        base.Awake();
        Type = UIType.MemoryWnd;
        MutexInterface = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnMemoryNumChange(0);
        SongsDataMng.GetInstance().Player.OnMemoryNumChange.AddListener(OnMemoryNumChange);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SongsDataMng.GetInstance().Player.OnMemoryNumChange.RemoveListener(OnMemoryNumChange);
    }

    void OnMemoryNumChange(int num)
    {
        slider.value = SongsDataMng.GetInstance().Player.MemoryNum / 4f;
        if(SongsDataMng.GetInstance().Player.MemoryNum < 3)
        {
            text.text = "Memory";
            image.gameObject.SetActive(false);
        }
        else
        {
            text.text = "Forever";
            image.gameObject.SetActive(true);
        }
    }
}
