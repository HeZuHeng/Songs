using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BuildUtil
{
    public class BuildSceneConfig
    {
        public static string OutPutConfigPath = Path.GetDirectoryName(Application.dataPath) + "/ScenesConfig";

        [MenuItem("Tools/BuildSceneConfig", false, 2)]
        public static void CreateSceneConfig()
        {
            string output = OutPutConfigPath;

            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }

            List<string> gamePaths = new List<string>();
            List<GameObject> prefabs = BuildAssetBundle.GetAllPrefabs(ref gamePaths);
            List<List<string>> prefabsFbxs = new List<List<string>>();
            List<string> prefabsABName = new List<string>();

            AssetImporter assetImporter = null;
            for (int j = 0; j < prefabs.Count; j++)
            {
                List<string> fbxs = BuildAssetBundle.GetPrefabDepePaths(prefabs[j]);
                if (fbxs.Count != 0)
                {
                    assetImporter = AssetImporter.GetAtPath(gamePaths[j]);
                    if (string.IsNullOrEmpty(assetImporter.assetBundleName))
                    {
                        Debug.LogError("预制体没有生成资源名 ：请使用MarkAllAssetBundle");
                        return;
                    }
                    prefabsABName.Add(assetImporter.assetBundleName);
                    prefabsFbxs.Add(fbxs);
                }
            }

            ModelConfig config = new ModelConfig();
            int id = 10000;
            int index = 0;
            foreach (UnityEditor.EditorBuildSettingsScene eScene in UnityEditor.EditorBuildSettings.scenes)
            {
                //在built setting中是否已经开启
                if (eScene.enabled)
                {
                    UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(eScene.path);
                    EditorUtility.DisplayProgressBar("整理场景……", scene.name + " 整理场景中……", (float)index / UnityEditor.EditorBuildSettings.scenes.Length);
                    Debug.Log(scene.name);
                    SceneData sceneData = new SceneData();
                    sceneData.name = scene.name;
                    sceneData.sceneName = scene.name;

                    List<GameObject> newPrefabs = new List<GameObject>();
                    List<string> newPrefabsPath = new List<string>();
                    List<GameObject> curPrefabs = new List<GameObject>();
                    List<GameObject> deletePrefabs = new List<GameObject>();

                    GameObject[] gameObjects = scene.GetRootGameObjects();
                    for (int i = 0; i < gameObjects.Length; i++)
                    {
                        Transform[] childs = gameObjects[i].GetComponentsInChildren<Transform>(true);
                        for (int j = 0; j < childs.Length; j++)
                        {
                            GameObject game = childs[j].gameObject;
                            if (game.isStatic) continue;
                            string assetPath = GetPrefabAssetPath(game, PrefabAssetType.Model);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                newPrefabs.Add(game);
                                newPrefabsPath.Add(assetPath);
                                //Debug.Log("整理场景 : " + game.gameObject.name + " : " + assetPath);
                            }
                            else
                            {
                                assetPath = GetPrefabAssetPath(game, PrefabAssetType.Regular);
                                if (!string.IsNullOrEmpty(assetPath) && gamePaths.Contains(assetPath))
                                {
                                    curPrefabs.Add(game);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < newPrefabs.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("检查预制体……", "检查预制体中……", (float)i / newPrefabs.Count);
                        bool eixtPrefab = false;
                        for (int j = 0; j < prefabsFbxs.Count; j++)
                        {
                            if (prefabsFbxs[j].Contains(newPrefabsPath[i]))
                            {
                                //Debug.Log(newPrefabs[i].name);
                                Transform tran = newPrefabs[i].transform;
                                ModelData modelData = new ModelData(tran.gameObject.name, prefabsABName[j], GetParentPath(tran.parent).Replace("Root/", ""));
                                modelData.Id = id++;
                                modelData.position = tran.position;
                                modelData.eulerAngle = tran.eulerAngles;
                                modelData.scale = tran.lossyScale;
                                sceneData.datas.Add(modelData);
                                eixtPrefab = true;
                                deletePrefabs.Add(tran.gameObject);
                                break;
                            }
                        }
                        if (!eixtPrefab)
                        {
                            Debug.LogError(scene.name + " 没有找到对应的预制体 ：" + newPrefabsPath[i]);
                        }
                    }

                    for (int i = 0; i < curPrefabs.Count; i++)
                    {
                        var prefabAsset = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(curPrefabs[i]);
                        string curPrefabsPath = AssetDatabase.GetAssetPath(prefabAsset);
                        assetImporter = AssetImporter.GetAtPath(curPrefabsPath);
                        if (string.IsNullOrEmpty(assetImporter.assetBundleName))
                        {
                            Debug.LogError("预制体没有生成资源名 ：请使用MarkAllAssetBundle");
                            return;
                        }
                        Transform tran = curPrefabs[i].transform;
                        ModelData modelData = new ModelData(tran.gameObject.name, assetImporter.assetBundleName, GetParentPath(tran.parent).Replace("Root/",""));
                        modelData.Id = id++;
                        modelData.position = tran.position;
                        modelData.eulerAngle = tran.eulerAngles;
                        modelData.scale = tran.lossyScale;
                        sceneData.datas.Add(modelData);
                    }

                    config.datas.Add(sceneData);
                    SaveConfig(output + "/ModelConfig.xml", config);

                    //删除已经做为预制体配置的物体
                    for (int i = 0; i < deletePrefabs.Count; i++)
                    {
                        GameObject.DestroyImmediate(deletePrefabs[i]);
                    }

                    for (int i = 0; i < curPrefabs.Count; i++)
                    {
                        GameObject.DestroyImmediate(curPrefabs[i]);
                    }
                    EditorSceneManager.SaveScene(scene);
                    AssetDatabase.Refresh();
                    index++;
                }
            }

            EditorUtility.ClearProgressBar();
            Debug.Log("------ Build Completed ---- ");
            AssetDatabase.Refresh();
        }

        public static List<Vector3> GetGamePositions(Transform tran)
        {
            List<Vector3> points = new List<Vector3>();  
            points.Add(tran.localPosition);
            return points;
        }

        public static void SaveConfig(string path, object config)
        {
            //创建文件流
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            //创建写入流
            StreamWriter sw = new StreamWriter(fileStream, Encoding.UTF8);
            //获取类型
            XmlSerializer xml = new XmlSerializer(config.GetType());
            //序列化至文件
            xml.Serialize(sw, config);
            //释放
            sw.Dispose();
            fileStream.Dispose();
        }
        
        public static string GetParentPath(Transform parent)
        {
            if(parent != null)
            {
                if (parent.parent != null)
                {
                    return GetParentPath(parent.parent) + "/" + parent.gameObject.name;
                }
                else
                {
                    return parent.gameObject.name;
                }
            }
            return string.Empty;
        }

#if UNITY_2018_3_OR_NEWER
        /// <summary>
        /// 获取预制体资源路径。
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static string GetPrefabAssetPath(GameObject gameObject, PrefabAssetType assetType)
        {
            // Project中的Prefab是Asset不是Instance
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                // 预制体资源就是自身
                return UnityEditor.AssetDatabase.GetAssetPath(gameObject);
            }

            // Scene中的Prefab Instance是Instance不是Asset
            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject) && PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject) && PrefabUtility.GetPrefabAssetType(gameObject) == assetType)
            {
                // 获取预制体资源
                var prefabAsset = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
                return UnityEditor.AssetDatabase.GetAssetPath(prefabAsset);
            }

            // PrefabMode中的GameObject既不是Instance也不是Asset
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage != null)
            {
                // 预制体资源：prefabAsset = prefabStage.prefabContentsRoot
                return prefabStage.prefabAssetPath;
            }
            // 不是预制体
            return null;
        }
#endif
    }
}
