using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle
{
    public static string OutPutPrefabsPath = Application.dataPath + "/BuildUtil/Resources";
    public static string PrefabsPath = "Assets/BuildUtil/Resources";

    //[MenuItem("Tools/BuildAssetBundle", false, 2)]
    //public static void BuildAb()
    //{
    //    if (!Directory.Exists(Application.streamingAssetsPath))
    //    {
    //        Directory.CreateDirectory(Application.streamingAssetsPath);
    //    }
    //    string output = Application.streamingAssetsPath + "/WebGL";
    //    if(!Directory.Exists(output))
    //    {
    //        Directory.CreateDirectory(output);
    //    }
    //    BuildPipeline.BuildAssetBundles(output, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.WebGL);
    //    Debug.Log("------ Build Completed ---- ");
    //    AssetDatabase.Refresh();
    //}

    //[MenuItem("Tools/BuildScene", false, 4)]
    //public static void BuildScene()
    //{
    //    if (!Directory.Exists(Application.streamingAssetsPath))
    //    {
    //        Directory.CreateDirectory(Application.streamingAssetsPath);
    //    }
    //    string output = "F:/SongsBiuld/WebGL";
    //    if (!Directory.Exists(output))
    //    {
    //        Directory.CreateDirectory(output);
    //    }

    //    string[] levels = { "Assets/Scenes/华兹华斯赏花.unity" };
    //    BuildPipeline.BuildPlayer(levels, output + "/华兹华斯赏花", BuildTarget.WebGL, BuildOptions.BuildAdditionalStreamedScenes);
    //    AssetDatabase.Refresh();
    //    levels[0] = "Assets/Scenes/陶渊明赏花.unity";
    //    BuildPipeline.BuildPlayer(levels, output + "/陶渊明赏花", BuildTarget.WebGL, BuildOptions.BuildAdditionalStreamedScenes);
    //    AssetDatabase.Refresh();
    //    levels[0] = "Assets/Scenes/华兹华斯书房.unity";
    //    BuildPipeline.BuildPlayer(levels, output + "/华兹华斯书房", BuildTarget.WebGL, BuildOptions.BuildAdditionalStreamedScenes);
    //}

    [MenuItem("Tools/ClearABMark", false, 3)]
    public static void ClearAllMarks()
    {
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (string name in abNames)
        {
            AssetDatabase.RemoveAssetBundleName(name, true);
        }
        AssetDatabase.Refresh();
        Debug.Log("Clear mark finished ---- ");
    }

    [MenuItem("Tools/MarkAllAssetBundle", false, 1)]
    public static void MarkAllAb()
    {
        List<string> gamePaths = new List<string>();
        List<GameObject> games = GetAllPrefabs(ref gamePaths);
        List<string> curNamePaths = new List<string>();
        for (int i = 0; i < games.Count; i++)
        {
            AssetImporter importer = AssetImporter.GetAtPath(gamePaths[i]);
            if (importer != null) importer.assetBundleName = games[i].name;
            EditorUtility.DisplayProgressBar("AB预制体赋值……", "AB预制体赋值中……", (float)i / games.Count);
            List<string> names = new List<string>();
            List<string> abs = GetPrefabDepePaths(games[i], ref names);
            for (int j = 0; j < abs.Count; j++)
            {
                if (!curNamePaths.Contains(abs[j]))
                {
                    importer = AssetImporter.GetAtPath(abs[j]);
                    if (importer != null && string.IsNullOrEmpty(importer.assetBundleName))
                    {
                        curNamePaths.Add(abs[j]);
                        string name = names[j].Replace(' ', '_');
                        if (CheckString(name))
                        {
                            name = name.GetHashCode().ToString();
                        }
                        name = Regex.Replace(name, "[^0-9A-Za-z]", "");
                        Debug.Log(name + "  :  " + abs[j]);
                        if (importer != null) importer.assetBundleName = name;
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }

    public static List<GameObject> GetAllPrefabs(ref List<string> gamePaths)
    {
        List<GameObject> prefabs = new List<GameObject>();
        var resourcesPath = OutPutPrefabsPath;
        var absolutePaths = System.IO.Directory.GetFiles(resourcesPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < absolutePaths.Length; i++)
        {
            EditorUtility.DisplayProgressBar("获取预制体……", "获取预制体中……", (float)i / absolutePaths.Length);

            string path = PrefabsPath + absolutePaths[i].Remove(0, resourcesPath.Length);
            path = path.Replace("\\", "/");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (prefab != null)
            {
                gamePaths.Add(path);
                prefabs.Add(prefab);
            }
            else
            {
                Debug.LogError("预制体不存在！ " + path);
            }
        }
        EditorUtility.ClearProgressBar();
        return prefabs;
    }

    /// 获取预制件依赖 <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">欲获取的类型</typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static List<string> GetPrefabDepePaths(GameObject go, ref List<string> names)
    {
        List<string> results = new List<string>();
        Object[] roots = new Object[] { go };
        Object[] dependObjs = EditorUtility.CollectDependencies(roots);
        foreach (Object dependObj in dependObjs)
        {
            if (dependObj != null && (dependObj.GetType() == typeof(Cubemap) || dependObj.GetType() == typeof(Texture2D) ||
                dependObj.GetType() == typeof(Material) || dependObj.GetType() == typeof(Shader) || dependObj.GetType() == typeof(Mesh)))
                //if (dependObj != null)
            {
                string path = AssetDatabase.GetAssetPath(dependObj);
                if (!results.Contains(path))
                {
                    names.Add(dependObj.name);
                    results.Add(path);
                }
            }
        } 
        return results;
    }

    public static List<string> GetPrefabDepePaths(GameObject go,string extension = ".fbx")
    {
        List<string> results = new List<string>();
        Object[] roots = new Object[] { go };
        Object[] dependObjs = EditorUtility.CollectDependencies(roots);
        for (int i = 0; i < dependObjs.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(dependObjs[i]);
            if (dependObjs[i] != null && (path.Contains(extension) || path.Contains(extension.ToUpper()) || path.Contains(extension.ToLower())))
            {
                if(dependObjs[i].GetType() != typeof(AnimationClip) && !results.Contains(path))
                {
                    results.Add(path);
                    //Debug.LogWarning(go.name + " : " + path);
                }
            }
        }
        if(results.Count == 0)
        {
            Debug.LogWarning(go.name + " : " + AssetDatabase.GetAssetPath(go) + " 没有找到FBX引用，请检查引用");
        }
        if(results.Count > 1)
        {
            Debug.LogError(go.name + " : " + AssetDatabase.GetAssetPath(go) + " 存在多个FBX引用，请排查问题");
        }
        return results;
    }

    //要单独打AssetBundle的
    private static void MarkSingleAB(List<string> dirList, List<string> fileExtList, string defaultFileName = null)
    {
        List<string> fileList = new List<string>();
        foreach(string dir in dirList)
        {
            ScanFile(dir, fileExtList, ref fileList);
        }
        
        foreach(string file in fileList)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            int index = file.IndexOf("Assets");
            string filePath = file.Substring(index);

            AssetImporter importer = AssetImporter.GetAtPath(filePath);
            if(string.IsNullOrEmpty((defaultFileName)))
            {
                importer.assetBundleName = fileName;
            }
            else
            {
                importer.assetBundleName = defaultFileName;
            }              
                  
        }
    }

    private static void MarkShader()
    {
        string prefix = Application.dataPath + "/ZhongKeYuan";
        string shaderDir = prefix + "/Shader";

        MarkSingleAB(new List<string> { shaderDir }, new List<string> { ".mat", ".shadergraph" });

        List<string> matList = new List<string>();
        ScanFile(shaderDir, new List<string> { ".mat" }, ref matList);
        MarkInternal(matList, "mat");

        List<string> shaderList = new List<string>();
        ScanFile(shaderDir, new List<string> { ".shader", ".shadergraph" }, ref shaderList);
        MarkInternal(shaderList, "shader");
    }

    private static void MarkInternal(List<string> fileList, string ext)
    {
        foreach (string file in fileList)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            int index = file.IndexOf("Assets");
            string filePath = file.Substring(index);

            AssetImporter importer = AssetImporter.GetAtPath(filePath);
            importer.assetBundleName = fileName;
        }
    }


    public static void ScanFile(string dir,  List<string> extList, ref List<string> fileList)
    {
        if (!Directory.Exists(dir))
        {
            Debug.LogError("Directory not exist -----");
            return;
        }

        string[] fileArray = Directory.GetFiles(dir);
        foreach (string file in fileArray)
        {
            FileInfo fileInfo = new FileInfo(file);
            if ((fileInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            {               
                foreach(string ext in extList)
                {                  
                    if(fileInfo.Extension.Equals(ext))
                    {
                        fileList.Add(file);
                    }
                }
            }
        }

        //得到目录
        List<string> tmpDirList = new List<string>(Directory.GetDirectories(dir));
        foreach (string tmpDir in tmpDirList)
        {
            ScanFile(tmpDir, extList, ref fileList);
        }
    }


    [MenuItem("Tools/ApplyPrefab", false, 5)]
    public static void ApplyPrefab()
    {
        string assetPrefixPath = "Assets/ZhongKeYuan/Prefab";

        GameObject[] objs = Selection.gameObjects;
        bool success = false;
        foreach (GameObject obj in objs)
        {
            string name = obj.name;  
            PrefabUtility.SaveAsPrefabAsset(obj, assetPrefixPath + "/" + name + ".prefab", out success);
            if (success)
            {
                Debug.Log("--prefab name is: " + obj.name);
            }
            else
            {
                Debug.LogError("--prefab Error: " + obj.name);
            }
        }
    }


    public static bool IsChinese(char c)
    {
        return c >= 0x4E00 && c <= 0x9FA5;
    }

    public static bool CheckString(string str)
    {
        char[] ch = str.ToCharArray();
        if (str != null)
        {
            for (int i = 0; i < ch.Length; i++)
            {
                if (IsChinese(ch[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

}
