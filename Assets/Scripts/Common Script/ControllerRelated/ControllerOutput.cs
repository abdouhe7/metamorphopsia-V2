using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;
using Valve.VR;
using UnityEngine.InputSystem;

public class ControllerOutput : MonoBehaviour
{
    static public Vector2 rightaxisDirection;

    static public bool pressPrimaryButton = false;
    static public bool pressMenuButton = false;

    static public bool holdPrimaryButton = false;
    static public bool pressDownPrimaryButton = false;
    static public bool pressUpPrimaryButton = false;

    static public GameObject hitObjectRight;
    static public Ray controllerRay;

    public Camera rigCamera;
    public float raycastDistance = Mathf.Infinity;

    void Start()
    {
        SteamVR.Initialize();
    }

    void Update()
    {
        rightaxisDirection = Vector2.zero;
        hitObjectRight = null;
        UpdateRay();
        RaycastFromSource(controllerRay, ref hitObjectRight);
        ExtraButtons();

#if UNITY_EDITOR
        var newRotation = rigCamera.transform.eulerAngles;

        if (Keyboard.current.upArrowKey.isPressed)
            newRotation.x -= 0.1f; // Decrease x for upward tilt (pitch)
        if (Keyboard.current.downArrowKey.isPressed)
            newRotation.x += 0.1f; // Increase x for downward tilt (pitch)
        if (Keyboard.current.leftArrowKey.isPressed)
            newRotation.y -= 0.1f; // Decrease y for left turn (yaw)
        if (Keyboard.current.rightArrowKey.isPressed)
            newRotation.y += 0.1f; // Increase y for right turn (yaw)

        rigCamera.transform.eulerAngles = newRotation;


        pressMenuButton = Keyboard.current.enterKey.wasReleasedThisFrame;
        pressPrimaryButton = Mouse.current.leftButton.isPressed;
        pressDownPrimaryButton = Mouse.current.leftButton.wasPressedThisFrame;
        pressUpPrimaryButton = Mouse.current.leftButton.wasReleasedThisFrame;
#else
        if (SteamVR_Actions.mixedreality_RightPadTrackerPressed.stateUp)
            rightaxisDirection = SteamVR_Actions.mixedreality_RightPadTracker.GetAxis(SteamVR_Input_Sources.RightHand);

        pressMenuButton = SteamVR_Actions.mixedreality_PressMenu.stateUp;
        pressPrimaryButton = SteamVR_Actions.mixedreality_PressTrigger.state;
        pressDownPrimaryButton = SteamVR_Actions.mixedreality_PressTrigger.stateDown;
        pressUpPrimaryButton = SteamVR_Actions.mixedreality_PressTrigger.stateUp;
#endif

//         // For continuous press state
//         pressPrimaryButton = SteamVR_Actions.default_InteractUI.GetState(SteamVR_Input_Sources.RightHand);
//
// // For button down (just pressed)
//         pressDownPrimaryButton = SteamVR_Actions.default_InteractUI.GetStateDown(SteamVR_Input_Sources.RightHand);
//
// // For button up (just released)
//         pressUpPrimaryButton = SteamVR_Actions.default_InteractUI.GetStateUp(SteamVR_Input_Sources.RightHand);
//
//         pressMenuButton = Keyboard.current.enterKey.isPressed;

    }

    void ExtraButtons()
    {
        if (Keyboard.current.equalsKey.wasReleasedThisFrame)
        {
            SimpleGameManager.Instance.IncreaseResponsiveness();
        }
        if (Keyboard.current.minusKey.wasReleasedThisFrame)
        {
            SimpleGameManager.Instance.DecreaseResponsiveness();
        }
        if (Keyboard.current.sKey.wasReleasedThisFrame)
        {
            SimpleGameManager.Instance.SaveDebugInformation();
        }
        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            SimpleGameManager.Instance.ToggleFollowEyes();
        }
    }

    void UpdateRay()
    {
#if UNITY_EDITOR
        controllerRay = rigCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
        controllerRay = new Ray(transform.position, transform.forward);
#endif
    }

    void RaycastFromSource(Ray ray, ref GameObject hitObject)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            hitObject = hit.collider.gameObject;
        }
        else
        {
            hitObject = null;
        }
    }
}
