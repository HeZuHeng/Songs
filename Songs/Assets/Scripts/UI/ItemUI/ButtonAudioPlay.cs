using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudioPlay : MonoBehaviour
{
    // Start is called before the first frame update
    public Button btn;

    void Start()
    {
        if (btn == null) btn = GetComponent<Button>();
        btn.onClick.AddListener(OnAudioPlay);
    }

    void OnAudioPlay()
    {
        AudioManager.Instance.PlaySound(1, 0.8f);
    }
}
