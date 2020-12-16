
using UnityEngine;
using System.Collections;
/// <summary>
/// 一级界面枚举
/// </summary>
namespace Songs
{
    /// <summary>
    /// 加载类型
    ///  
    /// </summary>
    public class ExResources
    {

        public enum PathType : byte
        {
            NoNe = 0,
            Atlas = 1,
            Font = 2,
            UI = 3,
            Util = 4,
        }

        public static string GetPlatformPath()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return string.Empty;
            }
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                return "PC";
            }
            else
            {
                return "Pad";
            }
        }

        public static Object GetResources(string path, PathType type)
        {
            switch (type)
            {
                case PathType.Atlas:
                    return Resources.Load("Sprites/" + path);
                case PathType.Font:
                    return Resources.Load("Fonts/" + path);
                case PathType.UI:
                    return Resources.Load(string.Format("Prefabs/UI/{0}/{1}", GetPlatformPath(), path));
                case PathType.Util:
                    return Resources.Load("Prefabs/UI/" + path);
                default:
                    return null;
            }
        }
    }
}
