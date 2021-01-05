using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerItemUI : MonoBehaviour
{
    public RectTransform rect;
    public Text qus;
    public Image tip;

    float radian = 0; // 弧度
    float perRadian = 0.03f; // 每次变化的弧度
    float radius = 30f; // 半径
    Vector3 oldPos; // 开始时候的坐标

    bool showTip;
    float time;
    float t = 4;
    // Start is called before the first frame update
    void Start()
    {
        if(rect == null)
        {
            rect = transform as RectTransform;
        }
        oldPos = transform.localPosition; // 将最初的位置保存到oldPos
    }

    // Update is called once per frame
    void Update()
    {
        radian += perRadian; // 弧度每次加0.03
        float dy = Mathf.Cos(radian) * radius; // dy定义的是针对y轴的变量，也可以使用sin，找到一个适合的值就可以
        transform.localPosition = oldPos + new Vector3(dy / t, dy, 0);

        if (showTip && Time.time - time > 3)
        {
            time = Time.time;
            tip.enabled = !tip.enabled;
            perRadian = Random.Range(0.02f, 0.1f);
            if(perRadian > 0.05f)
            {
                t *= -1;
            }
        }
    }

    public void Show(string text,bool val)
    {
        qus.text = text;
        showTip = val;
        time = Time.time;
        tip.enabled = false;
        perRadian = Random.Range(0.01f, 0.1f);
    }
}
