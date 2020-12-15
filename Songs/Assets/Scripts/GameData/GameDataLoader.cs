using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MREngine
{
    public class GameDataLoader
    {
        private const int MAXPROCESSER = 1;

        private static GameDataLoader instance = null;
        public static GameDataLoader GetInstance()
        {
            if(null == instance)
            {
                instance = new GameDataLoader();
            }
            return instance;
        }

        /// <summary>
        /// 空闲处理器队列
        /// </summary>
        private Queue<GameDataLoadFileProcesser> idleQueue = new Queue<GameDataLoadFileProcesser>();
        /// <summary>
        /// 运行处理器列表
        /// </summary>
        private List<GameDataLoadFileProcesser> runningList = new List<GameDataLoadFileProcesser>();

        /// <summary>
        /// 加载列表
        /// </summary>
        private List<GameDataLoadFileProcesser> loadList = new List<GameDataLoadFileProcesser>();

        public void Startup()
        {
            GameDataLoadFileProcesser processer;
            for(int i = 0;  i < MAXPROCESSER; i ++)
            {
                processer = new GameDataLoadFileProcesser();
                idleQueue.Enqueue(processer);
            }

            runningList.Clear();
        }

        public void AddLoadItem(ILoaderItem item)
        {
            if(!Contain(item))
            {
                GameDataLoadFileProcesser processer = new GameDataLoadFileProcesser(item);
                loadList.Add(processer);
            }        
        }

        private bool Contain(ILoaderItem item)
        {
            for(int i  = 0; i < loadList.Count; i++)
            {
                if(loadList[i].GetItem().URL.Equals(item.URL))
                {
                    return true;
                }
            }

            for (int j = 0; j < runningList.Count; j++)
            {
                if (runningList[j].GetItem().URL.Equals(item.URL))
                {
                    return true;
                }
            }

            return false;
        }


        private GameDataLoadFileProcesser processer;
        public void FrameUpdate()
        {
            for(int i = 0;  i < runningList.Count;)
            {
                processer = runningList[i];
                if(processer.Update())
                {
                    runningList.RemoveAt(i);
                    processer.Reset();
                    idleQueue.Enqueue(processer);
                }
                else
                {
                    ++i;
                }
            }

            while(loadList.Count > 0 && idleQueue.Count > 0 && runningList.Count < MAXPROCESSER)
            {
                GameDataLoadFileProcesser tmpProcesser = loadList[0];
                loadList.RemoveAt(0);

                tmpProcesser.Start();
                runningList.Add(tmpProcesser);
            }
        }
        
        public void Terminate()
        {
            if(runningList != null) runningList.Clear();
            if (idleQueue != null) idleQueue.Clear();
            if (loadList != null) loadList.Clear();

            runningList = null;
            idleQueue = null;        
            processer = null;
        }   
              
    }

}
