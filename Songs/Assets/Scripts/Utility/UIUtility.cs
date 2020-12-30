using System;
using System.Collections.Generic;
using UnityEngine;
public class UIUtility
{
    public static GameObject Control(string name, GameObject gameObj)
    {
        if (null == gameObj) return null;
        Transform trans = Control(name, gameObj.transform);
        if (trans == null) return null;
        return trans.gameObject;
    }

    public static Transform Control(string name, Transform gametransform)
    {
        if (null == gametransform || gametransform.childCount == 0)
            return null;

        for (int i = 0; i < gametransform.childCount; ++i)
        {
            Transform ctrans = gametransform.GetChild(i);
            if (ctrans.name.Equals(name))
                return ctrans;
        }

        for (int i = 0; i < gametransform.childCount; ++i)
        {
            Transform ttTrans = Control(name, gametransform.GetChild(i));
            if (ttTrans != null)
                return ttTrans;
        }
        return null;
    }
}
