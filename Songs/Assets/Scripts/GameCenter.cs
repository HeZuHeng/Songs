using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MREngine;
using System.Text;
using UnityEngine.Networking;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;
using Songs;
using UnityStandardAssets.Characters.ThirdPerson;

namespace SpaceSimulation
{
    public class GameCenter : MonoBehaviour {
        [SerializeField]
        ThirdPersonUserControl ThirdPerson = null;
        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (!enabled) return;
            UIMng.Instance.OpenUI(UIType.StartWnd);
            SongsDataMng.GetInstance().Init();
            CameraMng.GetInstance().UserControl = ThirdPerson;
            GameDataManager.GetInstance().Startup(transform,delegate() {
            });
#if UNITY_WEBGL
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 120;
#endif

#if SONGS_DEBUG && UNITY_EDITOR
            if (!File.Exists(SongsDataMng.TaskPath))WriteTask();
            if (!File.Exists(SongsDataMng.ModelPath)) WriteModel();
#endif
        }

        // Use this for initialization
        void Start() {

            GameDataLoader.GetInstance().Startup();

        }

        // Update is called once per frame
        void Update() {
            GameDataLoader.GetInstance().FrameUpdate();
            GameDataManager.GetInstance().FrameUpdate();
            SongsDataMng.GetInstance().LoadUpdate();

            if (Input.GetKeyDown(KeyCode.R))
            {
                SongsDataMng.GetInstance().NextSceneData();
                //UnLoadScene();
                UIMng.Instance.OpenUI(UIType.LoadingWnd);
            }
        }

        public void OnApplicationQuit()
        {
            GameDataLoader.GetInstance().Terminate();
            GameDataManager.GetInstance().Terminate();
        }
#if SONGS_DEBUG
        void WriteTask()
        {
            TasksConfig tasksConfig = new TasksConfig();
            tasksConfig.datas = new List<TaskData>();
            TaskData fan1 = new TaskData("张三", 12);
            TaskData fan2 = new TaskData("李四", 14);
            TaskData fan3 = new TaskData("吴亦凡", 18);

            tasksConfig.datas.Add(fan1);
            tasksConfig.datas.Add(fan2);
            tasksConfig.datas.Add(fan3);
            using (FileStream fileStream = new FileStream(SongsDataMng.TaskPath, FileMode.Create))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(TasksConfig));
                xmlSerializer.Serialize(fileStream, tasksConfig);
            }
        }
        void WriteModel()
        {
            ModelConfig modelConfig = new ModelConfig();
            SceneData sceneData = new SceneData();
            sceneData.name = "hzhs";
            ModelData fan1 = new ModelData("张三", "12");
            ModelData fan2 = new ModelData("李四", "14");
            ModelData fan3 = new ModelData("吴亦凡", "18");
            sceneData.datas.Add(fan1);
            sceneData.datas.Add(fan2);
            sceneData.datas.Add(fan3);
            modelConfig.datas.Add(sceneData);

            using (FileStream fileStream = new FileStream(SongsDataMng.ModelPath, FileMode.Create))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModelConfig));
                xmlSerializer.Serialize(fileStream, modelConfig);
            }
        }
#endif
    }
}