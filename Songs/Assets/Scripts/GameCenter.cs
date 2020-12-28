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
        //[SerializeField]
        //ThirdPersonUserControl ThirdPerson = null;
        public static Transform AssetsParent;

        public string TestSceneName = "《我好似一朵孤独的流云》意境";

        private void Awake()
        {
            DontDestroyOnLoad(this);
#if UNITY_WEBGL
            Application.targetFrameRate = 30;
#else
            Application.targetFrameRate = 120;
#endif
            //TaskXml();
            //Xml();
            //ModelXml();
            if (!enabled) return;

            AssetsParent = new GameObject("AssetsParent").transform;
            AssetsParent.SetParent(transform);
            AssetsParent.position = Vector3.zero;
            AssetsParent.rotation = Quaternion.identity;
            AssetsParent.localScale = Vector3.one;

            GameDataManager.GetInstance().Startup(transform, delegate () {

            });
            SongsDataMng.GetInstance().Init();
            CameraMng.GetInstance().Start(transform);
            //CameraMng.GetInstance().UserControl = ThirdPerson;
            GameDataLoader.GetInstance().Startup();

#if SONGS_DEBUG
            SongsDataMng.GetInstance().OnDataLoaded = delegate (string val)
            {
                if (!string.IsNullOrEmpty(TestSceneName))
                {
                    SceneTaskData sceneTask = SongsDataMng.GetInstance().GetSceneTaskDataFromName(TestSceneName);
                    SongsDataMng.GetInstance().SetSceneTaskData(sceneTask);
                    UIMng.Instance.OpenUI(UIType.LoadingWnd);
                }
            };
#endif
        }

        // Use this for initialization
        void Start() {
#if SONGS_DEBUG
            if (string.IsNullOrEmpty(TestSceneName))
            {
                UIMng.Instance.OpenUI(UIType.StartWnd);
            }
#else
            UIMng.Instance.OpenUI(UIType.StartWnd);
#endif
        }

        // Update is called once per frame
        void Update() {
            GameDataLoader.GetInstance().FrameUpdate();
            GameDataManager.GetInstance().FrameUpdate();
            SceneController.GetInstance().FrameUpdate();
            SongsDataMng.GetInstance().LoadUpdate();
        }

        public void OnApplicationQuit()
        {
            GameDataLoader.GetInstance().Terminate();
            GameDataManager.GetInstance().Terminate();
        }

        //void TaskXml()
        //{
        //    TasksConfig tasksConfig = new TasksConfig();

        //    tasksConfig.name = "strg";
        //    SceneTaskData sceneTaskData = new SceneTaskData();
        //    sceneTaskData.name = "000";
        //    tasksConfig.datas.Add(sceneTaskData) ;
        //    using (FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/TasksConfig111.xml", FileMode.Create))
        //    {
        //        XmlSerializer xmlSerializer = new XmlSerializer(typeof(TasksConfig));
        //        xmlSerializer.Serialize(fileStream, tasksConfig);
        //    }
        //}

        //void ModelXml()
        //{
        //    ModelConfig modelConfig = new ModelConfig();
        //    SceneData data = new SceneData();
        //    data.name = "22";
        //    data.sceneName = "33";

        //    ModelData modelData = new ModelData();
        //    modelData.name = "111";
        //    modelData.assetName = "3333";
        //    modelData.position = new Vector3(0,0,0);
        //    modelData.eulerAngle = new Vector3(0, 0, 0);

        //    data.datas.Add(modelData);

        //    modelConfig.datas.Add(data);

        //    using (FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/TestModel.xml", FileMode.Create))
        //    {
        //        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModelConfig));
        //        xmlSerializer.Serialize(fileStream, modelConfig);
        //    }
        //}

        //void Xml()
        //{
        //    QuestionBankConfig questionBankConfig = new QuestionBankConfig();
        //    QuestionBankData question = new QuestionBankData();
        //    question.Id = 1;
        //    question.icon = "111";
        //    question.errorTip = "111";
        //    question.questions.Add("1");
        //    question.questions.Add("2");
        //    question.answers.Add(2);
        //    question.answers.Add(3);
        //    question.des = "11";
        //    question.head = "11";
        //    question.startParsing = "11";
        //    question.endParsing = "11";

        //    questionBankConfig.datas.Add(question);

        //    using (FileStream fileStream = new FileStream(SongsDataMng.QuestionBankPath, FileMode.Create))
        //    {
        //        XmlSerializer xmlSerializer = new XmlSerializer(typeof(QuestionBankConfig));
        //        xmlSerializer.Serialize(fileStream, questionBankConfig);
        //    }
        //}
    }
}