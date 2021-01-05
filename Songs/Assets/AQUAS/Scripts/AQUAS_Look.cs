using System.Collections.Generic;
using UnityEngine;

namespace AQUAS
{
    /// <summary>
    /// A simple mouse look script for the demos, provided for convenience to avoid having to import standard assets.
    /// Copied with thanks from here in unity forums : https://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/#post-3101292
    /// </summary>
    public class AQUAS_Look : MonoBehaviour
    {
        [Header("Info")]
        private List<float> _rotArrayX = new List<float>();
        private List<float> _rotArrayY = new List<float>();
        private float rotAverageX;
        private float rotAverageY;
        private float mouseDeltaX;
        private float mouseDeltaY;

        [Header("Settings")]
        public bool _isLocked = false;
        public float _sensitivityX = 1.5f;
        public float _sensitivityY = 1.5f;
        [Tooltip("The more steps, the smoother it will be.")]
        public int _averageFromThisManySteps = 3;

        [Header("References")]
        [Tooltip("Object to be rotated when mouse moves left/right.")]
        public Transform _playerRootT;
        [Tooltip("Object to be rotated when mouse moves up/down.")]
        public Transform _cameraT;

        private Transform parent;

        private void Awake()
        {
            _playerRootT = transform;
            _cameraT = transform;
            //if (parent == null) parent = transform.parent;
        }

        void Update()
        {
            MouseLookAveraged();
        }

        //============================================
        // FUNCTIONS (CUSTOM)
        //============================================

        void MouseLookAveraged()
        {
            rotAverageX = 0f;
            rotAverageY = 0f;
            mouseDeltaX = 0f;
            mouseDeltaY = 0f;


            if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                mouseDeltaX += Input.GetAxis("Mouse X") * _sensitivityX;
                mouseDeltaY += Input.GetAxis("Mouse Y") * _sensitivityY;
            }
            if (Input.GetMouseButtonUp(0))
            {
                rotAverageX = 0f;
                rotAverageY = 0f;
                mouseDeltaX = 0f;
                mouseDeltaY = 0f;
            }
            if (!_isLocked)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    offset += Vector3.forward * 0.1f;
                    if(offset.magnitude < 0.5f)
                    {
                        offset.z = -0.5f;
                    }
                }
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    offset -= Vector3.forward * 0.1f;
                    if(offset.magnitude > 20)
                    {
                        offset.z = -20;
                    }
                }
            }
            
            // Add current rot to list, at end
            _rotArrayX.Add(mouseDeltaX);
            _rotArrayY.Add(mouseDeltaY);

            // Reached max number of steps? Remove oldest from list
            if (_rotArrayX.Count >= _averageFromThisManySteps)
                _rotArrayX.RemoveAt(0);

            if (_rotArrayY.Count >= _averageFromThisManySteps)
                _rotArrayY.RemoveAt(0);

            // Add all of these rotations together
            for (int i_counterX = 0; i_counterX < _rotArrayX.Count; i_counterX++)
                rotAverageX += _rotArrayX[i_counterX];

            for (int i_counterY = 0; i_counterY < _rotArrayY.Count; i_counterY++)
                rotAverageY += _rotArrayY[i_counterY];

            // Get average
            rotAverageX /= _rotArrayX.Count;
            rotAverageY /= _rotArrayY.Count;

            // Apply
            if (parent != null)
            {
                parent.Rotate(0f, rotAverageX, 0f, Space.World);
                float x = _playerRootT.localEulerAngles.x > 180 ? _playerRootT.localEulerAngles.x - 360 : _playerRootT.localEulerAngles.x;
                if (rotAverageY > 0 && x < 0) return;
                if (rotAverageY < 0 && x > 70) return;
                _playerRootT.Rotate(-rotAverageY, 0f, 0f, Space.Self);
                ProcessMode();
            }
            else
            {
                _playerRootT.Rotate(0f, rotAverageX, 0f, Space.World);
                _cameraT.Rotate(-rotAverageY, 0f, 0f, Space.Self);
            }
        }
        [HideInInspector]
        public Vector3 offset = new Vector3(0, 0, 0.08f);
        Vector3 m_CamPos = Vector3.zero;
        Vector3 thirdPos = Vector3.zero;
        Vector3 direction = new Vector3();
        private void ProcessMode()
        {
            m_CamPos = _playerRootT.position;
            thirdPos = parent.position;
            direction = (thirdPos - m_CamPos).normalized;
            RaycastHit hit;
            Debug.DrawLine(thirdPos, m_CamPos,Color.red);
            if (Physics.Raycast(thirdPos, m_CamPos, out hit, offset.magnitude))
            {
                _playerRootT.localPosition = hit.distance * offset;
            }
            else
            {
                _playerRootT.localPosition = offset;
            }
            //Debug.Log(m_Cam.forward);
        }

        public void SetParent(Transform tran)
        {
            parent = tran;
        }
    }
}