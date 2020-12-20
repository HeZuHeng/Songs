using UnityEngine;
using System.Collections;

public class HighlightableObjectHelper : MonoBehaviour
{

    //持有当前外发光需要的组件
    public HighlightableObject[] m_hos;
    private  bool isInit = false;
    void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (m_hos == null || m_hos.Length == 0)
        {
            m_hos = this.GetComponentsInChildren<HighlightableObject>();
        }
        isInit = true;
    }

    private void OnEnable()
    {
        if (isInit == false) Init();
        if (m_hos != null && m_hos.Length != 0)
        {
            for (int i = 0; i < m_hos.Length; i++)
            {
                HighlightableObject m_ho = m_hos[i];
                if(m_ho!=null) m_ho.FlashingOn(Color.green, Color.gray, 1f);
            }
        }
    }
    private void OnDisable()
    {
        if (m_hos != null && m_hos.Length != 0)
        {
            for (int i = 0; i < m_hos.Length; i++)
            {
                HighlightableObject m_ho = m_hos[i];
                if (m_ho != null) m_ho.FlashingOff();
            }
        }
    }
}