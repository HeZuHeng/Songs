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
        private void Awake()
        {
            DontDestroyOnLoad(this);
#if UNITY_WEBGL
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 120;
#endif
            //Xml();
            if (!enabled) return;
            SongsDataMng.GetInstance().Init();
            CameraMng.GetInstance().Init(transform);
            //CameraMng.GetInstance().UserControl = ThirdPerson;
            GameDataLoader.GetInstance().Startup();
            GameDataManager.GetInstance().Startup(transform, delegate () {
                SceneAssetObject assetObject = SceneMng.GetInstance().AddSpaceAsset(1, "nvyk", "女游客", delegate (float pro) {
                    if (pro >= 1)
                    {
                        SceneAssetObject sceneAsset = SceneMng.GetInstance().GetSceneAssetObject(1);
                        sceneAsset.Tran.SetParent(transform);
                        CameraMng.GetInstance().InitPlayer(sceneAsset.Tran);
                        sceneAsset.Tran.gameObject.SetActive(false);
                    }
                });
            });
        }

        // Use this for initialization
        void Start() {
            UIMng.Instance.OpenUI(UIType.StartWnd);
        }

        // Update is called once per frame
        void Update() {
            GameDataLoader.GetInstance().FrameUpdate();
            GameDataManager.GetInstance().FrameUpdate();
            SongsDataMng.GetInstance().LoadUpdate();
        }

        public void OnApplicationQuit()
        {
            GameDataLoader.GetInstance().Terminate();
            GameDataManager.GetInstance().Terminate();
        }

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