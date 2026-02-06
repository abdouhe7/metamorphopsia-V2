using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSystem
{
    SceneState scene_state;
    public void SetScene(SceneState state)
    {
        if (scene_state != null)
            scene_state.OnSleep();
        scene_state = state;
        if (scene_state != null)
            scene_state.OnAwake();
    }
}
