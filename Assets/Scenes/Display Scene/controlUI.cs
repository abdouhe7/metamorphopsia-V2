using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlUI : MonoBehaviour
{
    bool OnUI = false;

    public Camera UIcamera;
    public Camera LeftEyeCamera;
    public Camera RightEyeCamera;
    public Camera LeftCamera;
    public Camera RightCamera;

    private void Start()
    {
        LeftCamera.enabled = true;
        RightCamera.enabled = true;
        LeftEyeCamera.enabled = true;
        RightEyeCamera.enabled = true;
        UIcamera.enabled = false;
    }
    void Update()
    {
        if (ControllerOutput.pressMenuButton)
        {
            OnUI = !OnUI;
            if (!OnUI)
            {
                LeftEyeCamera.enabled = true;
                RightEyeCamera.enabled = true;
                LeftCamera.enabled = true;
                RightCamera.enabled = true;
                UIcamera.enabled = false;
                DisplayScene.panelManager.Pop();
            }
            else
            {
                LeftEyeCamera.enabled = false;
                RightEyeCamera.enabled = false;
                LeftCamera.enabled = false;
                RightCamera.enabled = false;
                UIcamera.enabled = true;
                DisplayScene.panelManager.Push(new MenuPanel());
            }
        }
    }
}
