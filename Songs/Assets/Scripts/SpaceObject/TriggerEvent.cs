using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public TriggerEnterEvent enterEvent = new TriggerEnterEvent();

    void OnTriggerEnter(Collider other)
    {
        enterEvent?.Invoke(other.gameObject.name);
    }
}

public class TriggerEnterEvent : UnityEvent<string>
{
    public TriggerEnterEvent()
    {

    }
}