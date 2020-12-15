using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MREngine
{
    public interface ILoaderItem
    {
        string URL
        {
            get;
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        void LoadCompleteHandler(Object _content_);

        /// <summary>
        /// 加载失败
        /// </summary>
        void LoadErrorHandler();

        /// <summary>
        /// 加载进度
        /// </summary>
        /// <param name="_progress_"></param>
        void LoadProgress(float _progress_);

    }

}
