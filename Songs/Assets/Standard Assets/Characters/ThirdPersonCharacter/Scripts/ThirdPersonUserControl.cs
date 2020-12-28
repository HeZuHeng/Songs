using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        private Vector3 off;
        private bool _isLocked = false;
        private bool walk = true;

        public Vector3 offset = new Vector3(0, 2, 0);
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                if(m_Cam == null) m_Cam = Camera.main.transform;
            }
            else
            {
                //Debug.LogWarning(
                //    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }

        private void OnEnable()
        {
           //if(m_Character != null) m_Character.enabled = true;
        }

        private void OnDisable()
        {
            _isLocked = false;
           //if(m_Character != null) m_Character.enabled = false;
        }

        public void State(bool walk = true)
        {
            this.walk = walk;
            if (m_Character != null) m_Character.State(walk);
        }

        public void SetMainCamera(Transform tran, bool isLocked = false)
        {
            m_Cam = tran;
            _isLocked = isLocked;
        }

        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = Input.GetButtonDown("Jump");
            }
        }


        private void LateUpdate()
        {
            if (!_isLocked && m_Cam != null) m_Cam.position = transform.position + offset;
        }

        //Vector3 m_CamPos = Vector3.zero;
        //Vector3 thirdPos = Vector3.zero;
        //Vector3 direction = new Vector3();
        //private void ProcessMode()
        //{
        //    m_CamPos = transform.position + offset;
        //    thirdPos = transform.position + Vector3.up * offset.y;
        //    direction = (m_CamPos - thirdPos).normalized;
        //    RaycastHit hit;
        //    if (Physics.Raycast(thirdPos, direction, out hit, Vector3.Distance(thirdPos,m_CamPos)))
        //    {
        //        if(hit.transform.GetHashCode() != transform.GetHashCode())
        //        {
        //            m_Cam.position = hit.point - direction * offset.z;
        //        }
        //    }
        //    else
        //    {
        //        m_Cam.position = m_CamPos - direction * offset.z;
        //    }
        //    //Debug.Log(m_Cam.forward);
        //}

        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            if (!walk) return;
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }
            //Debug.Log(m_CamForward + " : "+ m_Move);
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
