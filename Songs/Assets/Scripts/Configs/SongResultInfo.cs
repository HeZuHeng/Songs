using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
public class SongResultInfo {

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern void SendSongResult(string songResult);
    [DllImport("__Internal")]
    public static extern void UnityToHtmlData(string songResult);

#else
    public static void SendSongResult(string songResult) { }
    public static void UnityToHtmlData(string songResult){}
    //public static void InputEnd() { }
#endif

    /// <summary>
    /// 单选题
    /// </summary>
    public List<AnswerResultData> singleChoices = new List<AnswerResultData>();
    /// <summary>
    /// 多选题
    /// </summary>
    public List<AnswerResultData> multipleChoices = new List<AnswerResultData>();
    /// <summary>
    /// 填空题
    /// </summary>
    public List<AnswerResultData> fillInTheBlanks = new List<AnswerResultData>();
    /// <summary>
    /// 操作题
    /// </summary>
    public List<AnswerResultData> operatings = new List<AnswerResultData>();
    /// <summary>
    /// 总结
    /// </summary>
    public string summary;
    /// <summary>
    /// 总结
    /// </summary>
    public float summaryMinute;
    /// <summary>
    /// 总分
    /// </summary>
    public string totalMinute;

    public SongResultInfo()
    {
        singleChoices.Add(new AnswerResultData(1,"A",0));
        singleChoices.Add(new AnswerResultData(2, "B", 0));
        singleChoices.Add(new AnswerResultData(3, "B", 0));

        AnswerResultData a1 = new AnswerResultData(1, "0", 0);
        a1.childs.Add(new AnswerChildResultData(1, "浪漫主义色彩1,浪漫主义色彩5",0));
        a1.childs.Add(new AnswerChildResultData(2, "浪漫主义色彩3,浪漫主义色彩4", 0));
        a1.childs.Add(new AnswerChildResultData(3, "浪漫主义色彩2", 0));
        multipleChoices.Add(a1);
        multipleChoices.Add(new AnswerResultData(2, "A、D", 0));
        multipleChoices.Add(new AnswerResultData(3, "A、B", 0));
        multipleChoices.Add(new AnswerResultData(4, "A、B、C", 0));
        multipleChoices.Add(new AnswerResultData(5, "A、B、D", 0));
        multipleChoices.Add(new AnswerResultData(6, "B、C", 0));

        AnswerResultData a2 = new AnswerResultData(1, "0", 0);
        a2.childs.Add(new AnswerChildResultData(1, "myself", 0));
        a2.childs.Add(new AnswerChildResultData(2, "me,you", 0));
        a2.childs.Add(new AnswerChildResultData(3, "parents", 0));
        a2.childs.Add(new AnswerChildResultData(4, "energy", 0));
        fillInTheBlanks.Add(a2);
        fillInTheBlanks.Add(new AnswerResultData(2, "democracy", 0));
        fillInTheBlanks.Add(new AnswerResultData(3, "the individual", 0));
        fillInTheBlanks.Add(new AnswerResultData(4, "the collective", 0));

        AnswerResultData o3 = null;
        AnswerResultData o7 = null;
        AnswerResultData o14 = null;
        AnswerResultData o19 = null;
        for (int i = 0; i < 22; i++)
        {
            operatings.Add(new AnswerResultData(i + 1, 0));
            if(i == 2)
            {
                o3 = operatings[i];
            }
            if (i == 6)
            {
                o7 = operatings[i];
            }
            if (i == 13)
            {
                o14 = operatings[i];
            }
            if (i == 18)
            {
                o19 = operatings[i];
            }
        }

        o3.childs.Add(new AnswerChildResultData(1,0));
        o3.childs.Add(new AnswerChildResultData(2, 0));
        o3.childs.Add(new AnswerChildResultData(3, 0));

        o7.childs.Add(new AnswerChildResultData(1, 0));
        o7.childs.Add(new AnswerChildResultData(2, 0));
        o7.childs.Add(new AnswerChildResultData(3, 0));

        o14.childs.Add(new AnswerChildResultData(1, 0));
        o14.childs.Add(new AnswerChildResultData(2, 0));
        o14.childs.Add(new AnswerChildResultData(3, 0));
        o14.childs.Add(new AnswerChildResultData(4, 0));

        o19.childs.Add(new AnswerChildResultData(1, 0));
        o19.childs.Add(new AnswerChildResultData(2, 0));
        o19.childs.Add(new AnswerChildResultData(3, 0));
        o19.childs.Add(new AnswerChildResultData(4, 0));
        summary = string.Empty;
    }

    public AnswerResultData FindAnswer(int id, AnswerType answerType)
    {
        List<AnswerResultData> list = null;
        switch (answerType)
        {
            case AnswerType.FillInTheBlank:
                list = fillInTheBlanks;
                break;
            case AnswerType.SingleChoice:
                list = singleChoices;
                break;
            case AnswerType.MultipleChoice:
                list = multipleChoices;
                break;
            case AnswerType.Operating:
                list = operatings;
                break;
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Id == id)
            {
                return list[i];
            }
        }
        return null;
    }

    public void FillAnswer(int id,string answer,int min, AnswerType answerType)
    {
        AnswerResultData answerResultData = FindAnswer(id, answerType);
        if (answerResultData != null) answerResultData.FillAnswer(answer, min);
        Debug.Log(" answerType = " + answerType + "Id = " + answerResultData.Id + " : minute = " + answerResultData.minute);
    }

    public void FillAnswer(int id, int childId, string answer, int min, AnswerType answerType)
    {
        AnswerResultData answerResultData = FindAnswer(id, answerType);
        if (answerResultData != null) answerResultData.FillAnswer(childId, answer, min);
        Debug.Log(" answerType = " + answerType + "Id = " + answerResultData.Id + " : minute = " + answerResultData.minute);
    }

    public void FillSummary(string summary)
    {
        this.summary = summary;
    }

    public void SendLoaded(string jsonData,int totalMin)
    {
        SendSongResult(jsonData);
        if(totalMin >= 90)
        {
            UnityToHtmlData(totalMin + "_优秀");
        }
        if (totalMin >= 80 && totalMin < 90)
        {
            UnityToHtmlData(totalMin + "_良好");
        }
        if (totalMin >= 60 && totalMin < 80)
        {
            UnityToHtmlData(totalMin + "_及格");
        }
        if (totalMin < 60)
        {
            UnityToHtmlData(totalMin + "_不及格");
        }
    }
}

[System.Serializable]
public class AnswerResultData
{
    /// <summary>
    /// 题目编号
    /// </summary>
    public int Id;
    /// <summary>
    /// 参考答案
    /// </summary>
    public string answer;
    /// <summary>
    /// 答案
    /// </summary>
    public string mAnswer;
    /// <summary>
    /// 分数
    /// </summary>
    public int minute;
    /// <summary>
    /// 子题目
    /// </summary>
    public List<AnswerChildResultData> childs = new List<AnswerChildResultData>();

    public AnswerResultData(int id, int minute)
    {
        Id = id;
        this.answer = string.Empty;
        this.minute = minute;
    }

    public AnswerResultData(int id, string answer, int minute)
    {
        Id = id;
        this.answer = answer;
        this.minute = minute;
    }

    public void FillAnswer(string answer, int min)
    {
        mAnswer = answer;
        minute = min;
        //Debug.Log("Id = " + Id + " : minute = " + minute);
    }

    public void FillAnswer(int childId,string answer, int min)
    {
        int totalMin = 0;
        string str = string.Empty;
        for (int i = 0; i < childs.Count; i++)
        {
            if(childs[i].Id == childId)
            {
                childs[i].FillAnswer(answer, min);
            }
            totalMin += childs[i].minute;
            if(i == 0)
            {
                str = childs[i].answer;
            }
            else
            {
                str += "," + childs[i].answer;
            }
        }
        mAnswer = str;
        minute = totalMin;
        //Debug.Log("Id = " + Id + " : minute = " + minute);
    }
}

[System.Serializable]
public class AnswerChildResultData
{
    /// <summary>
    /// 题目编号
    /// </summary>
    public int Id;
    /// <summary>
    /// 参考答案
    /// </summary>
    public string answer;
    /// <summary>
    /// 答案
    /// </summary>
    public string mAnswer;
    /// <summary>
    /// 分数
    /// </summary>
    public int minute;

    public AnswerChildResultData(int id, int minute)
    {
        Id = id;
        this.answer = string.Empty;
        this.minute = minute;
    }

    public AnswerChildResultData(int id, string answer, int minute)
    {
        Id = id;
        this.answer = answer;
        this.minute = minute;
    }

    public void FillAnswer(string answer, int min)
    {
        mAnswer = answer;
        minute = min;
    }
}
