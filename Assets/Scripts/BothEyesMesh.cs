using UnityEngine;

public class BothEyesMesh : MonoBehaviour
{
    void Awake()
    {
        GetComponent<MeshFilter>().mesh = PlayerMesh.DisplayMesh();
    }
}
