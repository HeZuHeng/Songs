using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add by heshuai
namespace LaoZiCloudSDK.CameraHelper
{
    public class InputManager
    {
        private volatile static InputManager _instance = null;
        private static readonly Object _locker = new Object();

        public event SimpleInputSignal onPress;
        public event SimpleInputSignal onClick;
        public event SimpleInputSignal onRelease;
        public event DragInputSignal onDrag;
        /// <summary>
        /// 没有触点，射线没有碰到碰撞
        /// </summary>
        public event HoldDownInputSignal onNoTouch;
        /// <summary>
        /// 按下不动
        /// </summary>
        public event HoldDownInputSignal onHoldDown;
        /// <summary>
        /// 双指缩放
        /// </summary>
        public event TwoFingerZoomSignal onTwoFingerZoom;

        public static InputManager GetInstance()
        {
            if (null == _instance)
            {
                lock (_locker)
                {
                    if (null == _instance)
                    {
                        _instance = new InputManager();
                    }
                }
            }
            return _instance;
        }

        public void AddTwoFingerZoomEventListener(TwoFingerZoomSignal onTwoFinger)
        {
            this.onTwoFingerZoom += onTwoFinger;
        }

        public void RemoveTwoFingerZoomEventListener(TwoFingerZoomSignal onTwoFinger)
        {
            this.onTwoFingerZoom -= onTwoFinger;
        }

        public void AddNoTouchEventListener(HoldDownInputSignal onNoTouch)
        {
            this.onNoTouch += onNoTouch;
        }

        public void RemoveNoTouchEventListener(HoldDownInputSignal onNoTouch)
        {
            this.onNoTouch -= onNoTouch;
        }

        public void AddHoldDownEventListener(HoldDownInputSignal onHoldDown)
        {
            this.onHoldDown += onHoldDown;
        }

        public void RemoveHoldDownEventListener(HoldDownInputSignal onHoldDown)
        {
            this.onHoldDown -= onHoldDown;
        }

        public void AddReleaseEventListener(SimpleInputSignal onRelease)
        {
            this.onRelease += onRelease;
        }

        public void RemoveReleaseEventListener(SimpleInputSignal onRelease)
        {
            this.onRelease -= onRelease;
        }

        public void AddClickEventListener(SimpleInputSignal onClick)
        {
            this.onClick += onClick;
        }

        public void RemoveClickEventListener(SimpleInputSignal onClick)
        {
            this.onClick -= onClick;
        }

        public void AddDragEventListener(DragInputSignal _drag_)
        {
            onDrag += _drag_;
        }

        public void RemoveDragEventListener(DragInputSignal _drag_)
        {
            onDrag -= _drag_;
        }

        public void AddPressEventListener(SimpleInputSignal _press_)
        {
            onPress += _press_;
        }

        public void RemovePressEventListener(SimpleInputSignal _press_)
        {
            onPress -= _press_;
        }

        public void TwoFingerZoomEventDispatch(float zoom)
        {
            if (null != onTwoFingerZoom)
            {
                onTwoFingerZoom.Invoke(zoom);
            }
        }

        public void HoldDownEventDispatch(Vector2 _pos_)
        {
            if (null != onDrag)
            {
                onHoldDown.Invoke(_pos_);
            }
        }

        public void NoTouchEventDispatch(Vector2 _pos_)
        {
            if (null != onNoTouch)
            {
                onNoTouch.Invoke(_pos_);
            }
        }

        public void DragEventDispatch(Vector3 _pos_)
        {
            if (null != onDrag)
            {
                onDrag.Invoke(_pos_);
            }
        }


        public void PressEventDispatch(int _instaceId_)
        {
            if (null != onPress)
            {
                onPress.Invoke(_instaceId_);
            }
        }

        public void ClickEventDispatch(int _instaceId_)
        {
            if (null != onClick)
            {
                onClick.Invoke(_instaceId_);
            }
        }

        public void ReleaseEventDispatch(int _instaceId_)
        {
            if (null != onRelease)
            {
                onRelease.Invoke(_instaceId_);
            }
        }

    }
}
