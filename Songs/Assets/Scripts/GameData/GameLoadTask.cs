using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MREngine
{
    public class GameLoadTask
    {
        public event SimpleProgressSignal OnTaskProgress;

        private AbstractData mainData;
        public AbstractData MainData
        {
            get { return mainData; }
            set { mainData = value; }
        }

        private List<AbstractData> dpDataList = new List<AbstractData>();

        public void AddDpData(AbstractData data)
        {
            dpDataList.Add(data);
        }

        public void Start()
        {
            for(int i = 0;  i < dpDataList.Count;  i++)
            {
                AbstractData data = dpDataList[i];
                if(!data.Loaded)
                {
                    GameDataLoader.GetInstance().AddLoadItem(data);
                }
            }

            if(!MainData.Loaded)
            {
                GameDataLoader.GetInstance().AddLoadItem(MainData);
            }
        }

        public float Progress { get; set; }

        public bool Update()
        {
            if (dpDataList == null || MainData == null) return true;
            bool dpDone = true;
            Progress = 0f;
            for (int i = 0; i < dpDataList.Count; i++)
            {
                if(!dpDataList[i].Loaded)
                {
                    dpDone = false;                   
                }
                Progress += dpDataList[i].Progress;
            }
           
            Progress += MainData.Progress;
            Progress = Progress / (dpDataList.Count + 1);          

            if(dpDone && MainData.Loaded)
            {
                MainData.ReadyToUse = true;
                Progress = 1f;
                if(null != OnTaskProgress)
                {
                    OnTaskProgress.Invoke(Progress);
                }
                return true;
            }

            if(null != OnTaskProgress)
            {
                OnTaskProgress.Invoke(Progress);
            }

            return false;
        }

        public void Reset()
        {
            MainData = null;
            if(null != dpDataList)
            {
                dpDataList.Clear();
                dpDataList = null;
            }
            OnTaskProgress = null;           
        }
    }
}

