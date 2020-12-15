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
#if UNITY_WEBGL
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 120;
#endif
            if (!enabled) return;
            UIMng.Instance.OpenUI(UIType.StartWnd);
            SongsDataMng.GetInstance().Init();
            CameraMng.GetInstance().UserControl = ThirdPerson;
            GameDataManager.GetInstance().Startup(transform,delegate() {
            });


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
        }

        public void OnApplicationQuit()
        {
            GameDataLoader.GetInstance().Terminate();
            GameDataManager.GetInstance().Terminate();
        }
    }
}