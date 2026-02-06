using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class DebugUI : MonoBehaviour
{
    public TMP_Text resolution;
    void Start()
    {
        resolution.text = Screen.currentResolution.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
