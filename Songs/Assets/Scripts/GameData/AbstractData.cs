using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MREngine
{
    public class AbstractData : ILoaderItem
    {
        private Dictionary<string, ObjectPool> objectPool = new Dictionary<string, ObjectPool>();

        public event SimpleDataSignal OnDataReady;

        public void AddDataHandler(SimpleDataSignal handler)
        {
            if(null == handler)
            {
                return;
            }

            if(ReadyToUse)
            {
                handler.Invoke(URL);
            }
            else
            {
                OnDataReady += handler;
            }
        }

        public void RemoveDataHandler(SimpleDataSignal handler)
        {
            if(null == handler)
            {
                return;
            }

            OnDataReady -= handler;
        }
        
        public string URL
        {
            get;private set;
        }

        public AbstractData(string url)
        {
            this.URL = url;
        }

        private int refCount = 1;
        public int RefCount
        {
            get { return refCount; }
            private set { refCount = value; }
        }

        public void AddRefCount()
        {
            RefCount++;
        }

        public void RemoveRefCount()
        {
            RefCount--;
        }

        /// <summary>
        /// 自身AB 加载完
        /// </summary>
        private bool loaded;
        public bool Loaded
        {
            get { return loaded; }
            set { loaded = value; }
        }

        /// <summary>
        /// 自身AB和依赖AB都加载完
        /// </summary>
        private bool readyToUse;
        public bool ReadyToUse
        {
            get { return readyToUse; }
            set
            {
                readyToUse = value;
                if(readyToUse &&  (null != OnDataReady))
                {
                    OnDataReady.Invoke(URL);
                }
            }          
        }

        private AssetBundle assetBundle;

        public float Progress { get; set; }

        public void LoadCompleteHandler(UnityEngine.Object content)
        {
            assetBundle = content as AssetBundle;          
            Loaded = true;
            Progress = 1f;
        }

        public void LoadErrorHandler()
        {
            Loaded = true;
            Progress = 1f;
        }

        public void LoadProgress(float progress)
        {
            Progress = progress;
        }


        public Object LoadAsset(string assetName)
        {
            ObjectPool pool = GetOrCreateObjectPool(assetName);
            if (pool == null) return null;
            return pool.GetAsset();
        }     

        public GameObject LoadGameObject(string assetName)
        {          
            ObjectPool pool = GetOrCreateObjectPool(assetName);
            if (pool == null) return null;
            GameObject obj = pool.GetGameObject();
            return obj;
        }

        public string GetSceneName(string assetName)
        {
            if (assetBundle == null || !assetBundle.isStreamedSceneAssetBundle) return null;
            
            return assetBundle.GetAllScenePaths()[0];
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            string assetName = gameObject.name;
            ObjectPool pool = GetObjectPool(assetName);
            if(null == pool)
            {
                Debug.Log("------- GetObjectPool " + gameObject.name + " not exist !!! ");
                Object.Destroy(gameObject);
                return;
            }

            pool.Destroy(gameObject);        
        }

        private ObjectPool GetObjectPool(string assetName)
        {
            ObjectPool pool = null;
            if (objectPool.TryGetValue(assetName, out pool))
            {
                return pool;
            }
            return pool;
        }
        private ObjectPool GetOrCreateObjectPool(string assetName)
        {
            ObjectPool pool = null;
            if (objectPool.TryGetValue(assetName, out pool))
            {
                return pool;
            }
            if (assetBundle == null) return null;
            Object asset = assetBundle.LoadAsset(assetName);
            pool = new ObjectPool(asset);
            objectPool.Add(assetName, pool);
            return pool;
        }

        public void Reset()
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(false);
            }
            assetBundle = null;
            Loaded = false;
            ReadyToUse = false;
            URL = null;
            RefCount = 0;
            OnDataReady = null;

            if(null != objectPool)
            {
                var enumerator = objectPool.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.Reset();
                }
                objectPool.Clear();                         
            }
        }
    }

}
