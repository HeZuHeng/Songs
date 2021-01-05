using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotItemUI : MonoBehaviour
{
    public SceneTaskData sceneTaskData;
    public Text plotName;
    public Image icon;
    public Font font;
    public Font font2;

    public void Show(SceneTaskData taskData)
    {
        sceneTaskData = taskData;
        if (taskData.name.Contains("饮酒"))
        {
            plotName.font = font;
        }
        else
        {
            plotName.font = font2;
        }
        plotName.text = taskData.name;

        Sprite obj = Resources.Load<Sprite>("Sprites/SceneIcon/" + taskData.icon);
        if(obj != null)
        {
            icon.sprite = obj;
        }
    }
}
