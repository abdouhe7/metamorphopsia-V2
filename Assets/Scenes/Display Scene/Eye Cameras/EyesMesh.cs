using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EyesMesh : MonoBehaviour
{
    public GameObject Follower;

    public static bool isIndividual = false;

    Texture2D uvMapLeft;
    Texture2D uvMapRight;
    Texture2D uvMapBoth;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = PlayerMesh.DisplayMesh();

        uvMapLeft = SaveAndLoad.ReadUV("LeftEyeSample");
        uvMapRight = SaveAndLoad.ReadUV("RightEyeSample");
        uvMapBoth = SaveAndLoad.ReadUV("BothEyesSample");

        isIndividual = SaveAndLoad.LoadDisplayMode();

        gameObject.transform.SetParent(Follower.transform);
    }

    private void Update()
    {
        SetUVTexture();
    }

    void SetUVTexture()
    {
        if (isIndividual)
        {
            if (uvMapBoth != null)
            {
                GetComponent<Renderer>().material.SetTexture("_UVTex", uvMapBoth);
                GetComponent<Renderer>().material.SetFloat("exist", 1.0f);
            }
            else
                GetComponent<Renderer>().material.SetFloat("exist", 0.0f);
        }

        else
        {
            if (this.name == "LeftContent")
            {
                if (uvMapLeft != null)
                {
                    GetComponent<Renderer>().material.SetTexture("_UVTex", uvMapLeft);
                    GetComponent<Renderer>().material.SetFloat("exist", 1.0f);
                }
                else
                    GetComponent<Renderer>().material.SetFloat("exist", 0.0f);
            }

            else if (this.name == "RightContent")
            {
                if (uvMapRight != null)
                {
                    GetComponent<Renderer>().material.SetTexture("_UVTex", uvMapRight);
                    GetComponent<Renderer>().material.SetFloat("exist", 1.0f);
                }
                else
                    GetComponent<Renderer>().material.SetFloat("exist", 0.0f);
            }
        }
    }
}
