using TMPro;
using UnityEngine;
public class ButtonHandler : MonoBehaviour
{
    public TMP_Text labelText;
    public GridFunctions gridFunctions;
    public Color pressedColor;

    private Material material;
    private Color originalColor;
    private AudioSource audioSource;

    private void Start()
    {
        labelText.text = gridFunctions.ToString();
        material = GetComponent<Renderer>().material;
        originalColor = material.color;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        ChoosePointWithRaycast();
    }

    void ChoosePointWithRaycast()
    {
        var pointObject = ControllerOutput.hitObjectRight;

        if (pointObject != null)
        {
            var buttonHandler = pointObject.GetComponent<ButtonHandler>();
            if(buttonHandler != null && buttonHandler.Equals(this))
            {
                if (ControllerOutput.pressUpPrimaryButton)
                {
                    ButtonPressed();
                }
                else if (ControllerOutput.pressDownPrimaryButton)
                {
                    ButtonPressedDown();
                }
            }
        }
        else
        {
            material.color = originalColor;
        }
    }

    void ButtonPressedDown()
    {
        material.color = pressedColor;
    }

    void ButtonPressed()
    {
        audioSource.Play();
        material.color = originalColor;
        switch (gridFunctions)
        {
            case GridFunctions.SPLIT:
                SimpleGameManager.Instance.Subdivide();
                break;
            case GridFunctions.SAVE:
                SimpleGameManager.Instance.Save();
                break;
            case GridFunctions.READ:
                SimpleGameManager.Instance.Read();
                break;
            case GridFunctions.RESET:
                SimpleGameManager.Instance.ResetMesh();
                break;
            case GridFunctions.TOGGLE_EYES:
                SimpleGameManager.Instance.ToggleFollowEyes();
                break;
            default:
                break;
        }
    }
}
