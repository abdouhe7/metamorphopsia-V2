using UnityEngine;
using TMPro;

/// <summary>
/// Lesson 1: Demonstrates eye selection for VR testing
/// Covers Question 9: How to block one eye when testing the other
/// 
/// MEDICAL CONTEXT:
/// Metamorphopsia affects each eye differently, so we need to test
/// and correct each eye separately. This script blocks one eye's
/// view while testing the other.
/// </summary>
public class EyeBlocker : MonoBehaviour
{
    /// <summary>
    /// Which eye(s) can see the content
    /// </summary>
    public enum EyeMode
    {
        BothEyes,      // Normal - both eyes see
        LeftEyeOnly,   // Block RIGHT eye, test LEFT
        RightEyeOnly   // Block LEFT eye, test RIGHT
    }
    
    [Header("=== Current Mode ===")]
    public EyeMode currentMode = EyeMode.BothEyes;
    
    [Header("=== Blocker Objects ===")]
    [Tooltip("Black plane positioned in front of left eye")]
    public GameObject leftEyeBlocker;
    
    [Tooltip("Black plane positioned in front of right eye")]
    public GameObject rightEyeBlocker;
    
    [Header("=== UI Feedback ===")]
    public TMP_Text statusText;
    
    void Start()
    {
        // Apply initial mode
        ApplyMode();
        
        Debug.Log("[EyeBlocker] Ready. Press 1/2/3 to change eye mode.");
    }
    
    void Update()
    {
        // Keyboard shortcuts for quick testing
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetMode(EyeMode.BothEyes);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetMode(EyeMode.LeftEyeOnly);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetMode(EyeMode.RightEyeOnly);
        }
    }
    
    /// <summary>
    /// Set which eye(s) can see the content
    /// </summary>
    public void SetMode(EyeMode mode)
    {
        currentMode = mode;
        ApplyMode();
    }
    
    /// <summary>
    /// Apply the current eye mode by showing/hiding blockers
    /// </summary>
    void ApplyMode()
    {
        switch (currentMode)
        {
            case EyeMode.BothEyes:
                // Normal view - no blockers
                SetBlocker(leftEyeBlocker, false);
                SetBlocker(rightEyeBlocker, false);
                UpdateStatus("Both Eyes Active", "Press 2 for Left only, 3 for Right only");
                break;
                
            case EyeMode.LeftEyeOnly:
                // Testing LEFT eye - block RIGHT eye
                SetBlocker(leftEyeBlocker, false);
                SetBlocker(rightEyeBlocker, true);
                UpdateStatus("Left Eye Only", "Right eye is blocked");
                break;
                
            case EyeMode.RightEyeOnly:
                // Testing RIGHT eye - block LEFT eye
                SetBlocker(leftEyeBlocker, true);
                SetBlocker(rightEyeBlocker, false);
                UpdateStatus("Right Eye Only", "Left eye is blocked");
                break;
        }
        
        Debug.Log($"[EyeBlocker] Mode changed to: {currentMode}");
    }
    
    /// <summary>
    /// Show or hide a blocker object
    /// </summary>
    void SetBlocker(GameObject blocker, bool active)
    {
        if (blocker != null)
        {
            blocker.SetActive(active);
        }
    }
    
    /// <summary>
    /// Update UI status text
    /// </summary>
    void UpdateStatus(string title, string detail)
    {
        if (statusText != null)
        {
            statusText.text = $"Eye Mode: {title}\n{detail}";
        }
    }
    
    // ==========================================
    // PUBLIC METHODS FOR UI BUTTONS
    // ==========================================
    
    public void SetBothEyes()
    {
        SetMode(EyeMode.BothEyes);
    }
    
    public void SetLeftEyeOnly()
    {
        SetMode(EyeMode.LeftEyeOnly);
    }
    
    public void SetRightEyeOnly()
    {
        SetMode(EyeMode.RightEyeOnly);
    }
    
    /*
    ==========================================
    ALTERNATIVE METHOD FOR VIVE PRO EYE
    ==========================================
    
    Instead of blocker planes, student can use Camera.stereoTargetEye:
    
    // Render to both eyes (normal)
    Camera.main.stereoTargetEye = StereoTargetEyeMask.Both;
    
    // Render only to left eye
    Camera.main.stereoTargetEye = StereoTargetEyeMask.Left;
    
    // Render only to right eye
    Camera.main.stereoTargetEye = StereoTargetEyeMask.Right;
    
    This is cleaner but only works in actual VR mode.
    The blocker plane method works in both Editor and VR.
    */
}
