using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnswerItemUI : MonoBehaviour
{
    public RectTransform rect;
    public Text qus;
    public Image tip;
    public Image f;
    public RectTransform toggle;

    float radian = 0; // 弧度
    float perRadian = 0.03f; // 每次变化的弧度
    float radius = 10f; // 半径
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

        if (showTip && Time.time - time > 30)
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
        toggle.gameObject.SetActive(true);
        qus.text = text;
        showTip = val;
        time = Time.time;
        tip.enabled = false;
        perRadian = Random.Range(0.01f, 0.1f);
        oldPos = transform.localPosition; // 将最初的位置保存到oldPos
        f.gameObject.SetActive(false);
    }

    public void Show(bool val)
    {
        toggle.gameObject.SetActive(false);
        f.color = val ? Color.green : Color.red;

        Tween tween = f.DOColor(new Color(f.color.r, f.color.g, f.color.b, 30), 0.2f);
        tween.SetLoops(-1,LoopType.Yoyo);
        f.gameObject.SetActive(true);
    }
}
