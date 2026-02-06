using UnityEngine;

public class ApplyDistortion : MonoBehaviour
{
    public Material distortionMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (distortionMaterial != null)
        {
            Graphics.Blit(src, dest, distortionMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
