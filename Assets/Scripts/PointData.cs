using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointData : MonoBehaviour
{
    public int index;
    
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPointSelection()
    {
        audioSource.Play();
    }
}
