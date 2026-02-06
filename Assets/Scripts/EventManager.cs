using System;
using UnityEngine;

public static class EventManager
{
    public static event Action<GazeTarget> OnPrimaryTargetDeactivated;

    public static void TriggerPrimaryTargetDeactivated(GazeTarget deactivatedTarget)
    {
        OnPrimaryTargetDeactivated?.Invoke(deactivatedTarget);
    }
}
