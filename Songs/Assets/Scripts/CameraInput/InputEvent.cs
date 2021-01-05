
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LaoZiCloudSDK.CameraHelper
{
    /// <summary>
    /// 相机事件发起方
    /// </summary>
    public class InputEvent : MonoBehaviour 
    {
        //共用
        public Transform mainCameraTrans;
        public float mouseMultiple = 0.01f;

        public Camera mainCamera;
        protected List<Touch> touchs = new List<Touch>();

        protected static Vector3 inputHitPos;
        protected static Vector3 currentNormal;
        protected bool dragging;

        protected GameObject lastGo;
        protected Vector2 deltaPosition;
        protected Vector2 pressedPosition = Vector2.zero;
        protected Vector2 currentPos = Vector2.zero;
        protected Vector2 lastPos = Vector2.zero;

        protected float pressedTime = 0;
        protected float holdDownTime = 0;
        protected float oldDistance = 0f;
        protected float touchDistance = 0f;
        private Vector3 dragOffset;
        private int hotControl;
        private float frameCount = 15f;

        protected virtual void Awake()
        {
            mainCamera = GetComponentInChildren<Camera>();
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            mainCameraTrans = mainCamera.transform;

            mainCamera.tag = "MainCamera";
            if(Application.targetFrameRate == -1)
            {
                frameCount = 15;
            }
            else
            {
                frameCount = Application.targetFrameRate / 3f;
            }
        }

        protected virtual void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ToTouch();
            }
            else
            {
                ToMouse();
            }
            touchDistance = 0;
            lastPos = currentPos;
        }


        protected virtual void ToMouse()
        {
            if (!dragging && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            this.currentPos.x = Mathf.Clamp(Input.mousePosition.x, 0, Screen.width);
            this.currentPos.y = Mathf.Clamp(Input.mousePosition.y, 0, Screen.height);
            deltaPosition = (currentPos - lastPos) * mouseMultiple;
            //Debug.Log(deltaPosition.x + ":: " + deltaPosition.y);
            if (Input.GetMouseButtonDown(0))
            {
                Press(this.currentPos);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Release(this.currentPos);
            }
            else if (Input.GetMouseButton(0))
            {
                Drag();
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                touchDistance = 1f;
                InputManager.GetInstance().TwoFingerZoomEventDispatch(touchDistance);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                touchDistance = -1f;
                InputManager.GetInstance().TwoFingerZoomEventDispatch(touchDistance);
            }
        }

        protected virtual void ToTouch()
        {
            touchs.Clear();
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    continue;
                }
                if (touchs.Count < 2)
                {
                    touchs.Add(Input.GetTouch(i));
                }
                else
                {
                    continue;
                }
            }

            if (touchs.Count == 1)
            {
                this.currentPos.x = Mathf.Clamp(touchs[0].position.x, 0, Screen.width);
                this.currentPos.y = Mathf.Clamp(touchs[0].position.y, 0, Screen.height);
                deltaPosition = touchs[0].deltaPosition;
                if (touchs[0].phase == TouchPhase.Began)
                {
                    Press(this.currentPos);
                }
                else if (touchs[0].phase == TouchPhase.Ended)
                {
                    Release(this.currentPos, touchs[0].deltaPosition);
                }
                else if (touchs[0].phase == TouchPhase.Moved)
                {
                    Drag();
                }
            }

            if (touchs.Count >= 2)
            {
                Touch touchA = touchs[0];
                Touch touchB = touchs[1];
                if (touchA.phase == TouchPhase.Began || touchB.phase == TouchPhase.Began)
                {
                    oldDistance = Vector2.Distance(touchA.position, touchB.position);
                }
                else if(touchA.phase == TouchPhase.Moved || touchB.phase == TouchPhase.Moved)
                {
                    touchDistance = Vector2.Distance(touchA.position, touchB.position) - oldDistance;
                    oldDistance = Vector2.Distance(touchA.position, touchB.position);
                    InputManager.GetInstance().TwoFingerZoomEventDispatch(touchDistance);
                }
            }
        }

        #region 输入坐标处理

        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="screenPos">屏幕点坐标</param>
        protected virtual void Press(Vector2 screenPos)
        {
			this.pressedTime = Time.frameCount;
            this.holdDownTime = pressedTime;
            this.pressedPosition = screenPos;
            this.dragging = true;

            if(GUIUtility.hotControl != 0)
            {
                hotControl = GUIUtility.hotControl;
                return;
            }

            this.lastGo = RaycastObject(screenPos);
            if (this.lastGo != null)
            {
                //this.lastGo.SendMessage("OnPress", screenPos, SendMessageOptions.DontRequireReceiver);
                InputManager.GetInstance().PressEventDispatch(this.lastGo);

                Vector3 screen = mainCamera.WorldToScreenPoint(lastGo.transform.position);
                dragOffset = screen - new Vector3(screenPos.x, screenPos.y, 0);
            }
        }


        protected virtual bool Drag()
        {
            if (currentPos != pressedPosition)
            {
                if (lastGo != null)
                {
                    Vector3 pos = new Vector3(currentPos.x, currentPos.y, 0);
                    pos = pos + dragOffset;
                    InputManager.GetInstance().DragEventDispatch(mainCamera.ScreenToWorldPoint(pos));
                }
                return true;
            }

            if (currentPos != Vector2.zero && currentPos == pressedPosition && Time.frameCount - holdDownTime > frameCount * 2f)
            {
                holdDownTime = Time.frameCount;
                InputManager.GetInstance().HoldDownEventDispatch(currentPos);
            }
            return false;
        }

        /// <summary>
        /// 松开
        /// </summary>
        /// <param name="screenPos">屏幕点坐标</param>
        protected virtual void Release(Vector2 screenPos)
        {
            if(hotControl != 0)
            {
                hotControl = GUIUtility.hotControl;
                return;
            }
            if (this.lastGo != null)
            {
                GameObject currentGo = RaycastObject(screenPos);
                //Debug.Log((screenPos - pressedPosition).magnitude + "  // " + currentGo.name + "  " + Time.frameCount + "  : " + (Time.frameCount - pressedTime));
                if (currentGo == this.lastGo && (screenPos - pressedPosition).magnitude <= 10 && Time.frameCount - pressedTime <= frameCount)
                {
                    //this.lastGo.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
                    InputManager.GetInstance().ClickEventDispatch(this.lastGo);
                }
                

                //this.lastGo.SendMessage("OnRelease", SendMessageOptions.DontRequireReceiver);
                InputManager.GetInstance().ReleaseEventDispatch(this.lastGo);
                this.lastGo = null;
            }
            else if ((screenPos - pressedPosition).magnitude <= 10 && Time.frameCount - pressedTime <= frameCount)
            {
                InputManager.GetInstance().NoTouchEventDispatch(screenPos);
            }

            this.pressedTime = Time.frameCount;
            this.holdDownTime = pressedTime;
            inputHitPos = Vector3.zero;
            this.pressedPosition = Vector2.zero;
            this.dragging = false;
        }
        
        /// <summary>
        /// 松开
        /// </summary>
        /// <param name="screenPos">屏幕点坐标</param>
        public void Release(Vector2 screenPos, Vector2 detalPos)
        {
            if (this.lastGo != null)
            {
                GameObject currentGo = RaycastObject(screenPos);
				//Debug.Log(detalPos.magnitude + "  // " + currentGo.name + "  " + Time.frameCount + "  : " + (Time.frameCount - pressedTime));
                if (currentGo == this.lastGo && detalPos.magnitude <= 10f && Time.frameCount - pressedTime <= frameCount)
                {
                    //this.lastGo.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
                    InputManager.GetInstance().ClickEventDispatch(this.lastGo);
                }

                //this.lastGo.SendMessage("OnRelease", SendMessageOptions.DontRequireReceiver);
                InputManager.GetInstance().ReleaseEventDispatch(this.lastGo);
                this.lastGo = null;
            }
            else if ((screenPos - pressedPosition).magnitude <= 10 && Time.frameCount - pressedTime <= frameCount)
            {
                InputManager.GetInstance().NoTouchEventDispatch(screenPos);
            }
            this.pressedTime = Time.frameCount;
            inputHitPos = Vector3.zero;
            this.pressedPosition = Vector2.zero;
            this.dragging = false;
        }


        /// <summary>
        /// 发射线
        /// </summary>
        /// <param name="screenPos">屏幕点坐标</param>
        /// <returns>返回被碰撞到的GameObject</returns>
        protected virtual GameObject RaycastObject(Vector2 screenPos)
        {
            RaycastHit hit;            
            if (Physics.Raycast(this.mainCamera.ScreenPointToRay(screenPos), out hit, 1000000f))
            {
                inputHitPos = hit.point;
                currentNormal = hit.normal;
                return hit.collider.gameObject;                
            }

            return null;
        }

        #endregion

        
    }
}