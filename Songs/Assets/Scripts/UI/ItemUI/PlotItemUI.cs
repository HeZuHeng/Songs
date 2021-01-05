using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotItemUI : MonoBehaviour
{
    public SceneTaskData sceneTaskData;
    public Text plotName;
    public Image icon;

    public void Show(SceneTaskData taskData)
    {
        sceneTaskData = taskData;
        plotName.text = taskData.name;

        Sprite obj = Resources.Load<Sprite>("Sprites/SceneIcon/" + taskData.icon);
        if(obj != null)
        {
            icon.sprite = obj;
        }
    }
}
