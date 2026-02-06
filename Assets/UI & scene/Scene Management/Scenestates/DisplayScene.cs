using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayScene : SceneState
{
    readonly string scene_name = "DisplayScene";
    static public PanelManager panelManager;
    public override void OnAwake()
    {
        if (SceneManager.GetActiveScene().name != scene_name)
        {
            SceneManager.LoadScene(scene_name);
            SceneManager.sceneLoaded += SceneLoaded;
        }
        panelManager = new PanelManager();
    }
    public override void OnSleep()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        if (!panelManager.Empty())
            panelManager.Pop();
    }

    private void SceneLoaded(Scene scene, LoadSceneMode load)
    {
        Debug.Log("Display scene loading complete.");
    }
}