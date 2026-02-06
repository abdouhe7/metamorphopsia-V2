/* SceneHandler.cs*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;

public class Interaction : MonoBehaviour
{
    public SteamVR_LaserPointer LaserPointer;

    void Start()
    {
        LaserPointer.PointerIn += PointerInside;
        LaserPointer.PointerOut += PointerOutside;
        LaserPointer.PointerClick += PointerClick;
    }

    private void OnDestroy()
    {
        LaserPointer.PointerClick -= PointerClick;
        LaserPointer.PointerOut -= PointerOutside;
        LaserPointer.PointerIn -= PointerInside;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        if (e.target.tag == "Buttons")
        {
            e.target.GetComponent<Image>().color = Color.cyan;
            e.target.GetComponent<Button>().onClick.Invoke();
        }
        if (e.target.tag == "Toggles")
        {
            e.target.GetComponent<Toggle>().isOn = !e.target.GetComponent<Toggle>().isOn;
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        if (e.target.tag == "Buttons")
        {
            e.target.GetComponent<Image>().color = Color.gray;
        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.tag == "Buttons")
        {
            e.target.GetComponent<Image>().color = Color.white;
        }
        
    }
}