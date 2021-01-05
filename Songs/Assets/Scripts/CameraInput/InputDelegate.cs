using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaoZiCloudSDK.CameraHelper
{
    public delegate void SimpleInputSignal(GameObject obj);
    public delegate void DoubleInputSignal(int _instanceId_);
    public delegate void DragInputSignal(Vector3 _pos_);
    public delegate void HoldDownInputSignal(Vector2 _pos_);
    public delegate void TwoFingerZoomSignal(float zoom);


    /// <summary>
    /// 相机操作系统模式
    /// </summary>
    public enum CameraMode
    {
        None = 0,
        /// <summary>
        /// 全局模型，与Unity的scene视图中一样
        /// </summary>
        Global = 1,
        /// <summary>
        /// 看向目标模式
        /// </summary>
        Target = 2,
        /// <summary>
        /// 第一人称
        /// </summary>
        First = 3,
        /// <summary>
        /// 第三人称
        /// </summary>
        Third = 4,
        /// <summary>
        /// 顶视图
        /// </summary>
        Top = 5,
        /// <summary>
        /// 基于目标的看向目标全局模型
        /// </summary>
        TargetGlobal = 6,

    }

    public abstract class CameraBase
    {
        public static Vector3 DEFAULT_LIMIT_POSITION = Vector3.one * float.MaxValue;
        [HideInInspector]
        public Transform fps;
        [HideInInspector]
        public Transform cameraTran;
        [HideInInspector]
        public Vector3 deltaPosition = Vector3.zero;
        [HideInInspector]
        public Camera camera;

        [SerializeField]
        private float rotaSpeed = 5;
        [SerializeField]
        private float moveSpeed = 8;

        [SerializeField]
        protected Vector3 limitPosition = CameraBase.DEFAULT_LIMIT_POSITION;

        public float RotaSpeed { get { return rotaSpeed; } set { rotaSpeed = value; } }
        public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
        
        public Camera MainCamera
        {
            get
            {
                if(camera == null && cameraTran != null)
                {
                    camera = cameraTran.GetComponentInChildren<Camera>();
                }
                return camera;
            }
            protected set
            {
                camera = value;
            }
        }

        public abstract bool Start(Transform transform);

        public abstract void Update();

        public abstract void CameraRotate();

        public abstract void CameraMove();

        public abstract void CameraZoom(float touchDistance);

        public abstract void Close();
    } 
}

