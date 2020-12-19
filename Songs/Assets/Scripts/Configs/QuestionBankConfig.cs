using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

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
    [XmlAttribute("ID")]
    public int Id;
    [XmlAttribute("图标")]
    public string icon;
    [XmlAttribute("题头")]
    public string head;
    [XmlAttribute("阅读")]
    public string des;
    [XmlAttribute("解析")]
    public string startParsing;
    [XmlAttribute("选择")]
    public string questions;
    [XmlAttribute("答案")]
    public List<int> answers;
    [XmlAttribute("答案解析")]
    public string endParsing;
    [XmlAttribute("提示")]
    public string errorTip;


    public QuestionBankData()
    {
        answers = new List<int>();
    }
}