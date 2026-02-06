using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance { get; private set; }
    public SceneSystem scene_system { get; private set; }

    void Configuration()
    {
        Screen.SetResolution(1920, 1200, true);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 90;
    }

    private void Awake()
    {
        Configuration();
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
            Instance = this;
        scene_system = new SceneSystem();

        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        scene_system.SetScene(new DisplayScene());
        //scene_system.SetScene(new EyeTestScene());
    }
}