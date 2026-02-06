using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GazeTarget : MonoBehaviour
{
    public Material primaryMat, dummyMat;
    public float interactableTime = 1f;

    private bool isOnPrimary;
    private float timer;

    private bool isPrimary;
    public bool IsPrimary
    {
        get => isPrimary;
        set
        {
            isPrimary = value;
            SetMaterial();
        }
    }

    public void SetMaterial()
    {
        if (IsPrimary)
        {
            transform.GetComponent<MeshRenderer>().material = primaryMat;
        }
        else
        {
            transform.GetComponent<MeshRenderer>().material = dummyMat;
        }
    }

    public void StartInteraction()
    {
        if (!isOnPrimary)
        {
            // Start the timer if this is the first time entering the primary target area
            isOnPrimary = true;
            timer = 0.0f; // Reset the timer
        }

        // Increment the timer
        timer += Time.deltaTime;

        // Check if the timer has reached the interactable time
        if (timer >= interactableTime)
        {
            Debug.Log($"Objects are overlapping for more than {interactableTime} seconds");
            DeactivatePrimary();

            // Reset the interaction state (optional, depending on your logic)
            isOnPrimary = false;
            timer = 0.0f;
        }
    }

    public void StopInteraction()
    {
        // Reset the timer if the player is not on the primary target anymore
        if (isOnPrimary)
        {
            isOnPrimary = false;
            timer = 0.0f;
        }
    }

    private void DeactivatePrimary()
    {
        IsPrimary = false;

        // Trigger the event via EventManager
        EventManager.TriggerPrimaryTargetDeactivated(this);
    }

}
