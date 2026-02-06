using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

public class MenuPanel : BasePanel
{
    static readonly string path = "Panels/MenuPanel";
    public MenuPanel() : base(new UIType(path)) { }
    public override void OnAwake()
    {
        Debug.Log("Menu Panel Working");
        Toggle[] toggles = new Toggle[2];

        toggles[0] = ui_tool.GetOrAddComponentInChildren<Toggle>("BothEyes");
        toggles[1] = ui_tool.GetOrAddComponentInChildren<Toggle>("IndividualEye");

        toggles[1].isOn = SaveAndLoad.LoadDisplayMode();
        toggles[0].isOn = !toggles[1].isOn;

        toggles[0].onValueChanged.AddListener((bool isOn) =>
        {
            if (isOn)
            {
                toggles[1].isOn = false;

                EyesMesh.isIndividual = toggles[1].isOn;
                SaveAndLoad.SaveDisplayMode(toggles[1].isOn);
            }
            else if (!toggles[1].isOn)
            {
                toggles[0].isOn = true;
            }
        });

        toggles[1].onValueChanged.AddListener((bool isOn) =>
        {
            if (isOn)
            {
                toggles[0].isOn = false;

                EyesMesh.isIndividual = toggles[1].isOn;
                SaveAndLoad.SaveDisplayMode(toggles[1].isOn);
            }
            else if (!toggles[0].isOn)
            {
                toggles[1].isOn = true;
            }
        });

        Toggle fishEyes = ui_tool.GetOrAddComponentInChildren<Toggle>("FishEyes");
        fishEyes.isOn = SteamVR_TestTrackedCamera.undistorted;

        fishEyes.onValueChanged.AddListener((bool isOn) =>
        {
            SteamVR_TestTrackedCamera.undistorted = isOn;
        });

        ui_tool.GetOrAddComponentInChildren<Button>("Quit").onClick.AddListener(() =>
        {
            Application.Quit();
        });

        ui_tool.GetOrAddComponentInChildren<Button>("Play").onClick.AddListener(() =>
        {
            GameRoot.Instance.scene_system.SetScene(new EyeTestScene());
        });
    }
}