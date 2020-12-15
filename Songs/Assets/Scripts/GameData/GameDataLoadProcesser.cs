using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MREngine
{
    public class GameDataLoadProcesser
    {
        /// <summary>
        /// 加载请求
        /// </summary>
        private UnityWebRequest request = null;
        

        private bool completed = false;
        public bool Completed
        {
            get { return completed; }
            set { completed = value; }
        }

        protected ILoaderItem Item
        {
            get;
            private set;
        }

        public GameDataLoadProcesser()
        {

        }

        public GameDataLoadProcesser(ILoaderItem item)
        {
            Item = item;
        }

        public ILoaderItem GetItem()
        {
            return Item;
        }

        public void Start()
        {
            if(null != request)
            {
                request.Dispose();
                request = null;
            }

            string url = FileUtil.GetDataURL(Item.URL);     
            try
            {              
                request = UnityWebRequestAssetBundle.GetAssetBundle(url);
                request.SendWebRequest();
                Completed = false;
                
            }
            catch (System.Exception ex)
            {
            	if(null != request)
                {
                    request.Dispose();
                    request = null;
                }

                Debug.LogError("Load Failed URL: " + url + " " + ex.ToString());
                Item.LoadErrorHandler();
                Completed = true;
                Item = null;
            }          
        }

        public bool Update()
        {
            if (Completed)
            {
                return true;
            }

            if (null == request)
            {
                return false;
            }          

            if(request.isDone)
            {
                try
                {
                    if(null == request.error)
                    {
                        AssetBundle assetBundle = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                        if (null != assetBundle)
                        {
                            Item.LoadCompleteHandler(assetBundle);
                        }
                        else
                        {
                            Item.LoadErrorHandler();
                            Debug.LogError("Load Error -- URL: " + Item.URL + " webrequest error : assetBundle is Null");
                        }
                    }
                    else
                    {
                        Item.LoadErrorHandler();
                        Debug.LogError("Load Error -- URL: " + Item.URL + " webrequest error : " + request.error.ToString());
                    }
                }
                catch (System.Exception ex)
                {
                    Item.LoadErrorHandler();
                    Debug.LogError("Load Data Exception: ---- " + ex.ToString());
                }
                finally
                {
                    request.Dispose();
                    request = null;
                    Item = null;
                }
                Completed = true;
            }
            else
            {
                //未完成，可更新加载进度
                Completed = false;
                //Debug.Log("----- webrequest progress is: " + request.downloadProgress);
            }
            return Completed;
        }

        public void Reset()
        {
           if(null != request)
            {
                request.Dispose();
                request = null;
            }
            Item = null;
            Completed = false;
        }
    }
}

