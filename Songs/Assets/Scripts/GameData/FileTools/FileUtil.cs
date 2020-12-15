using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MREngine
{
    public class FileUtil
    {
        public static string GetManifestPath()
        {
            string _platform_ = GetPlatform();
            string _path_ = Application.streamingAssetsPath + "/" + _platform_ + "/" + _platform_;
            return _path_;
        }

        public static string GetPlatform()
        {   
            switch(Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:               
                    return "Windows";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "Mac";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                default:
                    return string.Empty;
            }         
        }

        public static string GetDataURL(string _url_)
        {
            return Application.streamingAssetsPath + "/" + GetPlatform() + "/"  + _url_;
        }
    }
}

