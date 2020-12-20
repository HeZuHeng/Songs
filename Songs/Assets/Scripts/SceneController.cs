using MREngine;
using Slate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController 
{

    private static SceneController _instance = null;
    public static SceneController GetInstance()
    {
        if (null == _instance)
        {
            _instance = new SceneController();
        }
        return _instance;
    }

    SceneData CurSceneData;
    GameObject playCutsceneObj;
    GameObject cameraObj;
    GameObject initTranObj;

    public void Init()
    {
        Close();

        SceneData sceneData = SongsDataMng.GetInstance().GetSceneData;
        CurSceneData = sceneData;
        if (sceneData == null) return;

        GameLoadTask loadCutscenen = GameDataManager.GetInstance().GetGameTask(CurSceneData.sceneCutscenen);
        loadCutscenen.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                playCutsceneObj = loadCutscenen.MainData.LoadGameObject(CurSceneData.sceneCutscenen);
                if (playCutsceneObj != null)
                {
                    Cutscene playCutscene = playCutsceneObj.GetComponent<Cutscene>();
                    CameraMng.GetInstance().InitScene(playCutscene);
                }
            }
        };

        GameLoadTask loadCamera = GameDataManager.GetInstance().GetGameTask(CurSceneData.sceneCamera);
        loadCamera.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                cameraObj = loadCamera.MainData.LoadGameObject(CurSceneData.sceneCamera);
                if (cameraObj != null)
                {
                    cameraObj.AddComponent<HighlightingEffect>();
                    Camera camera = cameraObj.GetComponent<Camera>();
                    CameraMng.GetInstance().InitScene(camera);
                }
            }
        };
        GameLoadTask loadPosition = GameDataManager.GetInstance().GetGameTask(CurSceneData.scenePosition);
        loadPosition.OnTaskProgress += delegate (float progress)
        {
            if (progress >= 1)
            {
                initTranObj = loadPosition.MainData.LoadGameObject(CurSceneData.scenePosition);
                if (initTranObj != null)
                {
                    Vector3 initPosition = initTranObj.transform.position;
                    Vector3 initRotation = initTranObj.transform.eulerAngles;
                    CameraMng.GetInstance().InitScene(initPosition, initRotation);
                }
            }
        };

        ModelData modelData = null;
        for (int i = 0; i < CurSceneData.datas.Count; i++)
        {
            modelData = CurSceneData.datas[i];
            SceneMng.GetInstance().AddSpaceAsset(modelData);
        }
    }

    public void Close()
    {
        CameraMng.GetInstance().ResetMove();
        if(playCutsceneObj != null) GameObject.DestroyImmediate(playCutsceneObj);
        if (cameraObj != null) GameObject.DestroyImmediate(cameraObj);
        if (initTranObj != null) GameObject.DestroyImmediate(initTranObj);
        if (CurSceneData == null) return;
        for (int i = 0; i < CurSceneData.datas.Count; i++)
        {
            SceneMng.GetInstance().RemoveSpaceObject(CurSceneData.datas[i].Id);
        }
    }
}
