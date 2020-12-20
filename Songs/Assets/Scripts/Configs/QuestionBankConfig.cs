using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class QuestionBankConfig
{
    [XmlElement("QuestionBankDatas")]
    public List<QuestionBankData> datas;

    public QuestionBankConfig() { datas = new List<QuestionBankData>(); }

}

[Serializable]
public class QuestionBankData
{
    [XmlElement("ID")]
    public int Id;
    [XmlElement("图标")]
    public string icon;
    [XmlElement("题头")]
    public string head;
    [XmlElement("阅读")]
    public string des;
    [XmlElement("解析")]
    public string startParsing;
    [XmlElement("选择")]
    public List<string> questions;
    [XmlElement("答案")]
    public List<int> answers;
    [XmlElement("答案解析")]
    public string endParsing;
    [XmlElement("提示")]
    public string errorTip;

    [NonSerialized]
    public QuestionEndEvent onQuestionEnd = new QuestionEndEvent();
    public QuestionBankData()
    {
        questions = new List<string>();
        answers = new List<int>();
    }
}

public class QuestionEndEvent : UnityEvent
{
    public QuestionEndEvent() { }
}