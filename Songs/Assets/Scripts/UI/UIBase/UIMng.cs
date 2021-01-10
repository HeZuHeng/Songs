

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Text;

/// <summary>
/// UI
/// </summary>
namespace Songs
{
    public class UIMng : MonoBehaviour
    {
        static UIMng instance;

        public static UIMng Instance
        {
            get
            {
                return instance;
            }
        }

        GameObject uIRoot = null;
        GraphicRaycaster graphicR;

        public GraphicRaycaster GraphicR { get { return graphicR; } }
        //		float width = 1920;
        //		float height = 1080;

        //float width = Screen.width;
        //float height = Screen.height;
        float scaleX = 1;

        public float ScaleX
        {
            get
            {
                return scaleX;
            }
        }

        float scaleY = 1;

        public float ScaleY
        {
            get
            {
                return scaleY;
            }
        }

        float screenW = Screen.width;
        float screenH = Screen.height;

        public float IconScaleX
        {
            get
            {
                if (screenW > screenH)
                {
                    return screenW / 1920f;
                }
                else
                {
                    return screenH / 1920f;
                }
            }
        }

        public float IconScaleY
        {
            get
            {
                if (screenW > screenH)
                {//1920/1080
                    return screenH / 1080f;
                }
                else
                {
                    return screenW / 1080f;
                }
            }
        }


        [System.NonSerialized] UIType curUIType;

        public UIType CurUIType
        {
            get { return curUIType; }
        }

        /// <summary>
        /// UIRoot
        /// </summary>
        public GameObject GetUIRoot
        {
            get
            {
                return uIRoot;
            }
        }

        Dictionary<string, UIBase> uiList = new Dictionary<string, UIBase>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            uIRoot = GameObject.Find("Root");
            if (uIRoot != null)
            {
                graphicR = uIRoot.GetComponent<GraphicRaycaster>();
                CanvasScaler ca = uIRoot.GetComponent<CanvasScaler>();
                if (ca != null)
                {
                    //ca.referenceResolution = new Vector2(Screen.width, Screen.height);
                }
            }
            scaleX = Screen.width / 1920f;
            scaleY = Screen.height / 1080f;
            InIt();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void InIt()
        {
            uiList.Clear();
        }

        /// <summary>
        /// 获取UIBase类,不能获取组合UI的类（如UIType.MainWnd）
        /// </summary>
        public T GetUIBase<T>(UIType _type) where T : UIBase
        {
            List<string> needPath = GetUIPath(_type);
            if (needPath.Count > 1)
            {
                Debug.LogError("不能获取组合UI !");
                return null;
            }
            if (needPath.Count == 1 && uiList.ContainsKey(needPath[0]))
            {
                return uiList[needPath[0]] as T;
            }
            return null;
        }

        /// <summary>
        /// 删除界面并卸载该界面所有引用（慎用，不熟悉内存机制不要用）
        /// </summary>
        public void DestroyUIBase(UIType _type, bool unLoad = false)
        {
            List<string> needPath = GetUIPath(_type);
            GameObject go = null;
            for (int i = 0; i < needPath.Count; i++)
            {
                if (uiList.ContainsKey(needPath[i]) && uiList[needPath[i]] != null)
                {
                    go = uiList[needPath[i]].gameObject;
                    if (go == null)
                        continue;
                    uiList.Remove(needPath[i]);
                    go.SetActive(false);
                    DestroyImmediate(go, true);
                    go = null;
                }
            }
        }

        /// <summary>
        /// 删除界面并卸载所有互斥界面所有引用（慎用，不熟悉内存机制不要用）
        /// </summary>
        public void DestroyAllUIBase(bool _all = false)
        {
            List<string> needPath = new List<string>();
            using (var datas = uiList.GetEnumerator())
            {
                while (datas.MoveNext())
                {
                    if (datas.Current.Value != null)
                    {
                        if (_all)
                        {
                            needPath.Add(datas.Current.Key);
                        }
                        else
                        {
                            if (datas.Current.Value.MutexInterface)
                            {
                                needPath.Add(datas.Current.Key);
                            }
                        }
                    }
                }
            }
            GameObject go = null;
            for (int i = 0; i < needPath.Count; i++)
            {
                go = uiList[needPath[i]].gameObject;
                uiList.Remove(needPath[i]);
                go.SetActive(false);
                DestroyImmediate(go, true);
                go = null;
            }
        }


        /// <summary>
        /// 关闭不互斥界面
        /// </summary>
        public void ConcealUI(UIType _type)
        {
            List<string> needPath = GetUIPath(_type);
            for (int i = 0; i < needPath.Count; i++)
            {
                if (uiList.ContainsKey(needPath[i]))
                {
                    if (!uiList[needPath[i]].MutexInterface)
                        uiList[needPath[i]].gameObject.SetActive(false);
                }
            }
            if (curUIType == _type)
            {
                curUIType = UIType.NONE;
            }
        }

        /// <summary>
        /// 打开不互斥界面
        /// </summary>
        public void ActivationUI(UIType _type, SubUIType _subType = SubUIType.NONE)
        {
            UIBase ui = null;
            List<string> needPath = GetUIPath(_type);
            for (int i = 0; i < needPath.Count; i++)
            {
                if (uiList.ContainsKey(needPath[i]))
                {
                    ui = uiList[needPath[i]];
                }
                else
                {
                    ui = ObtainUI(needPath[i], _type);
                }
                if (ui == null)
                    continue;
                if (_subType != SubUIType.NONE)
                    ui.CurSubUIType = _subType;
                if (!ui.gameObject.activeSelf)
                {
                    ui.gameObject.transform.localScale = Vector3.one;
                    ui.gameObject.SetActive(true);
                }
            }
            curUIType = _type;
        }

        /// <summary>
        /// 跨界面打开某个界面的子界面
        /// _typeda打开的界面(_type = UIType.NoNe关闭所有互斥界面)，_subType 子界面,_OnBaseType基于界面
        /// </summary>
        public void OpenSubUI(UIType _type, SubUIType _subType, UIType _OnBaseType = UIType.NONE)
        {
            List<string> onBasePath = _OnBaseType != UIType.NONE ? GetUIPath(_OnBaseType) : new List<string>();

            using (var data = uiList.GetEnumerator())
            {
                while (data.MoveNext())
                {
                    if (_OnBaseType == UIType.NONE || !onBasePath.Contains(data.Current.Key))
                    {
                        if (data.Current.Value != null && data.Current.Value.gameObject.activeSelf && data.Current.Value.MutexInterface)
                        {
                            data.Current.Value.gameObject.SetActive(false);
                        }
                    }
                }
            }
            List<string> needPath = GetUIPath(_type);
            UIBase ui = null;
            for (int i = 0; i < onBasePath.Count; i++)
            {
                if (uiList.ContainsKey(onBasePath[i]))
                {
                    ui = uiList[onBasePath[i]];
                }
                else
                {
                    ui = ObtainUI(onBasePath[i], _type);
                }
                if (ui == null)
                    continue;

                if (!ui.gameObject.activeSelf)
                    ui.gameObject.SetActive(true);
            }

            for (int i = 0; i < needPath.Count; i++)
            {
                if (uiList.ContainsKey(needPath[i]))
                {
                    ui = uiList[needPath[i]];
                }
                else
                {
                    ui = ObtainUI(needPath[i], _type);
                }
                if (ui == null)
                    continue;
                if (onBasePath.Count > 0)
                    ui.AddDepth = 10;
                ui.CurSubUIType = _subType;
                if (!ui.gameObject.activeSelf)
                {
                    ui.gameObject.transform.localScale = Vector3.one;
                    ui.gameObject.SetActive(true);
                }
            }
            curUIType = _type;
        }

        /// <summary>
        /// 打开某个界面或基于某个界面打开另一个界面
        /// _type打开的界面(_type = UIType.NoNe关闭所有互斥界面)，_OnBaseType基于界面
        /// </summary>
        public void OpenUI(UIType _type, UIType _OnBaseType = UIType.NONE)
        {
            //Debug.Log("_type = "+ _type);
            List<string> onBasePath = _OnBaseType != UIType.NONE ? GetUIPath(_OnBaseType) : new List<string>();

            using (var data = uiList.GetEnumerator())
            {
                while (data.MoveNext())
                {
                    if (_OnBaseType == UIType.NONE || !onBasePath.Contains(data.Current.Key))
                    {
                        if (data.Current.Value != null && data.Current.Value.gameObject.activeSelf && data.Current.Value.MutexInterface)
                        {
                            data.Current.Value.gameObject.SetActive(false);
                        }
                    }
                }
            }
            List<string> needPath = GetUIPath(_type);
            UIBase ui = null;
            for (int i = 0; i < onBasePath.Count; i++)
            {
                if (uiList.ContainsKey(onBasePath[i]))
                {
                    ui = uiList[onBasePath[i]];
                }
                else
                {
                    ui = ObtainUI(onBasePath[i], _type);
                }

                if (ui == null)
                    continue;

                if (!ui.gameObject.activeSelf)
                    ui.gameObject.SetActive(true);
            }

            for (int i = 0; i < needPath.Count; i++)
            {
               
                if (uiList.ContainsKey(needPath[i]))
                {
                    ui = uiList[needPath[i]];
                }
                else
                {
                    ui = ObtainUI(needPath[i], _type);
                }
                if (ui == null)
                    continue;
                if (onBasePath.Count > 0)
                    ui.AddDepth = 10;
                if (!ui.gameObject.activeSelf)
                {
                    ui.gameObject.transform.localScale = Vector3.one;
                    ui.gameObject.SetActive(true);
                }
            }
            curUIType = _type;
        }

        UIBase ObtainUI(string _path, UIType _type)
        {
            GameObject obj = Resources.Load(_path) as GameObject;
            if (obj != null)
            {
                GameObject game = Instantiate(obj) as GameObject;
                if (game != null)
                {
                    //if (_type == UIType.ExitConfirmWnd)
                    //{
                    //    FitScreen(game, true);
                    //}
                    //else
                    //{
                    //    FitScreen(game);
                    //}
                    FitScreen(game);

                    UIBase uibase = game.GetComponent<UIBase>();
                    if (uibase != null)
                    {
                        uibase.gameObject.SetActive(false);
                        uibase.Type = _type;
                        if (!uiList.ContainsKey(_path))
                            uiList[_path] = uibase;
                    }
                    return uibase;
                }
                return null;
            }
            return null;
        }

        List<string> GetUIPath(UIType _type)
        {
            string path = ExResources.GetPlatformPath();
            List<string> openList = new List<string>();
            switch (_type)
            {
                case UIType.LoadingWnd:
                    openList.Add(string.Format("Prefabs/UI/LoadingWnd", path));
                    return openList;
                //case UIType.MainWnd:
                //    openList.Add(string.Format("Prefabs/UI/MainWnd", path));
                //    return openList;
                case UIType.StartWnd:
                    openList.Add(string.Format("Prefabs/UI/StartWnd", path));
                    return openList;
                case UIType.IntroductionWnd :
                    openList.Add(string.Format("Prefabs/UI/IntroductionWnd", path));
                    return openList;
                case UIType.HZHSWnd:
                    openList.Add(string.Format("Prefabs/UI/HZHSWnd", path));
                    return openList;
                case UIType.HTMWnd:
                    openList.Add(string.Format("Prefabs/UI/HTMWnd", path));
                    return openList;
                case UIType.SelectPlotWnd:
                    openList.Add(string.Format("Prefabs/UI/SelectPlotWnd", path));
                    return openList;
                case UIType.SettingWnd:
                    openList.Add(string.Format("Prefabs/UI/SettingWnd", path));
                    return openList;
                case UIType.MainDialogueWnd:
                    openList.Add(string.Format("Prefabs/UI/MainDialogueWnd", path));
                    return openList;
                case UIType.LeftDialogueWnd:
                    openList.Add(string.Format("Prefabs/UI/LeftDialogueWnd", path));
                    return openList;
                case UIType.AnswerWnd:
                    openList.Add(string.Format("Prefabs/UI/AnswerWnd", path));
                    return openList;
                case UIType.FantasyWnd:
                    openList.Add(string.Format("Prefabs/UI/FantasyWnd", path));
                    return openList;
                case UIType.ExperienceWnd:
                    openList.Add(string.Format("Prefabs/UI/ExperienceWnd", path));
                    return openList;
                case UIType.FantasyCompareWnd:
                    openList.Add(string.Format("Prefabs/UI/FantasyCompareWnd", path));
                    return openList;
                case UIType.RenameWnd:
                    openList.Add(string.Format("Prefabs/UI/RenameWnd", path));
                    return openList;
                case UIType.QATalkWnd:
                    openList.Add(string.Format("Prefabs/UI/QATalkWnd", path));
                    return openList;
                case UIType.HZHSEndWnd:
                    openList.Add(string.Format("Prefabs/UI/HZHSEndWnd", path));
                    return openList;
                case UIType.HTMMapPathWnd:
                    openList.Add(string.Format("Prefabs/UI/HTMMapPathWnd", path));
                    return openList;
                case UIType.HTMMapPoint1Wnd:
                    openList.Add(string.Format("Prefabs/UI/HTMMapPoint1Wnd", path));
                    return openList;
                case UIType.HTMMapPoint2Wnd:
                    openList.Add(string.Format("Prefabs/UI/HTMMapPoint2Wnd", path));
                    return openList;
                case UIType.HTMMapPoint3Wnd:
                    openList.Add(string.Format("Prefabs/UI/HTMMapPoint3Wnd", path));
                    return openList;
                case UIType.HTMBoatNameWnd:
                    openList.Add(string.Format("Prefabs/UI/HTMBoatNameWnd", path));
                    return openList;
                case UIType.MemoryWnd:
                    openList.Add(string.Format("Prefabs/UI/MemoryWnd", path));
                    return openList;
                default:
                    return openList;
            }

        }

        /// <summary>
        /// 获取二级界面路径
        /// </summary>
        public string GetSubUIPath(SubUIType _type)
        {
            switch (_type)
            {
                default:
                    return string.Empty;
            }
        }



        public void SetUiTier(UIBase _ts)
        {
            if (uIRoot != null && uIRoot.transform.childCount >= 1)
            {
                _ts.transform.SetSiblingIndex(uIRoot.transform.childCount - 1);
                for (int i = 0; i < uIRoot.transform.childCount; i++)
                {
                    UIBase uibs = uIRoot.transform.GetChild(i).GetComponent<UIBase>();
                    if (uibs != null)
                    {
                        //					MDebug.LogError (_ts.UITier+":"+uibs.UITier);
                        if (_ts.UITier < uibs.UITier)
                        {
                            _ts.transform.SetSiblingIndex(uibs.transform.GetSiblingIndex());
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获得所有当前显示的UIBASE
        /// </summary>
        /// <returns>The all show U.</returns>
        public List<UIBase> GetAllShowUI()
        {
            List<UIBase> list = new List<UIBase>();
            using (var data = uiList.GetEnumerator())
            {
                while (data.MoveNext())
                {
                    if (data.Current.Value != null && data.Current.Value.gameObject.activeSelf)
                    {
                        UIBase ty = data.Current.Value;
                        if (ty != null)
                            list.Add(ty);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 判断UI是否显示
        /// </summary>
        /// <returns><c>true</c>, if U IIS show was gotten, <c>false</c> otherwise.</returns>
        public bool GetUIisShow(UIType _type)
        {
            using (var data = uiList.GetEnumerator())
            {
                while (data.MoveNext())
                {
                    if (data.Current.Value.Type == _type && data.Current.Value.gameObject.activeSelf)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 开关取反。非互斥界面用
        /// </summary>
        /// <param name="_type">Type.</param>
        public void ToggleUI(UIType _type)
        {
            UIBase ui = GetUIBase<UIBase>(_type);
            if (ui == null || !ui.isActiveAndEnabled)
            {
                ActivationUI(_type);
            }
            else
            {
                ConcealUI(_type);
            }
        }

        /// <summary>
        /// 适应屏幕
        /// </summary>
        /// <param name="_go">Go.</param>
        public void FitScreen(GameObject game, bool scale = false)
        {
            //width = GetPath ().Equals ("PC") ? 1920 : 2048;
            //height = GetPath ().Equals ("PC") ? 1080 : 1536;
            //scaleX = Screen.width / width;
            //scaleY = Screen.height / height;
            if (scale && !ExResources.GetPlatformPath().Equals("PC"))
            {
                //只拉伸pad
                scaleX = Screen.width / 2048.0f;
                scaleY = Screen.height / 1536.0f;
                //Debug.Log("========================== " + game.name);
                //Debug.Log("==========Screen.width================ " + Screen.width);

                //Debug.Log("==========Screen.height================ " + Screen.height);
            }
            else
            {
                scaleX = 1f;
                scaleY = 1f;
            }
            //if(game != null)
            //    Debug.Log(game.name + "   fitScreen : " + scaleX + " : " + scaleY);
            if (game != null)
            {
                RectTransform rc = game.transform as RectTransform;
                if (rc != null)
                {
                    Vector3 p = new Vector3(rc.anchoredPosition3D.x * scaleX, rc.anchoredPosition3D.y * scaleY, rc.anchoredPosition3D.z);
                    Vector2 size = new Vector2(rc.sizeDelta.x * scaleX, rc.sizeDelta.y * scaleY);
                    if (uIRoot != null) game.transform.SetParent(uIRoot.transform);
                    game.transform.localScale = Vector3.one;
                    rc.anchoredPosition3D = p;
                    rc.sizeDelta = size;
                    RectTransform[] rcList = game.GetComponentsInChildren<RectTransform>(true);
                    for (int i = 0; i < rcList.Length; i++)
                    {
                        rcList[i].anchoredPosition3D = new Vector3(rcList[i].anchoredPosition3D.x * scaleX, rcList[i].anchoredPosition3D.y * scaleY, rcList[i].anchoredPosition3D.z);
                        rcList[i].sizeDelta = new Vector2(rcList[i].sizeDelta.x * scaleX < 1 && rcList[i].sizeDelta.x * scaleX > 0 ? 1 : rcList[i].sizeDelta.x * scaleX, rcList[i].sizeDelta.y * scaleY < 1 && rcList[i].sizeDelta.y * scaleY > 0 ? 1 : rcList[i].sizeDelta.y * scaleY);
                        GridLayoutGroup gp = rcList[i].GetComponent<GridLayoutGroup>();
                        if (gp != null)
                        {
                            gp.cellSize = new Vector2(gp.cellSize.x * scaleX, gp.cellSize.y * scaleY);
                            gp.spacing = new Vector2(gp.spacing.x * scaleX, gp.spacing.y * scaleY);
                        }
                        Text tx = rcList[i].GetComponent<Text>();
                        if (tx != null)
                            tx.fontSize = (int)(tx.fontSize * ((scaleX + scaleY) / 2));
                    }
                }
            }
        }


        float oldW;
        float oldH;

        void RefreshBoard()
        {
            //scaleX = Screen.width / 1920f;
            //scaleY = Screen.height / 1080f;
            scaleX = 1f;
            scaleY = 1f;

            uIRoot.transform.localScale = Vector3.one * scaleX;
        }

        void Update()
        {
            if (oldW != Screen.width || oldH != Screen.height)
            {
                oldW = Screen.width;
                oldH = Screen.height;
                RefreshBoard();
                FitScreen(null);
            }
        }

        public void SetUIRootVisible()
        {
            if (uIRoot != null)
            {
                uIRoot.SetActive(uiActive);
            }
        }

        bool uiActive = true;
        public bool UIActive
        {
            get
            {
                return uiActive;
            }
            set
            {
                uiActive = value;
            }
        }

        public GameObject GetPrefabInstance(GameObject prefab, Transform parent)
        {
            GameObject gameItem = GameObject.Instantiate(prefab, parent);
            if (gameItem != null)
            {
                Transform trans = gameItem.transform;
                trans.localPosition = Vector3.zero;
                trans.localEulerAngles = Vector3.zero;
                trans.localScale = Vector3.one;
            }
            return gameItem;
        }
    }
}