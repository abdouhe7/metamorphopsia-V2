using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasFollow : MonoBehaviour
{
    public Camera camera;
    public float offset;
    public float smoothSpeed = 0.125f; // Adjust the speed for smoother or faster following

    void Update()
    {
        Vector3 headset_position = camera.transform.position;
        Quaternion headset_rotation = camera.transform.rotation;

        Vector3 look = camera.transform.TransformDirection(Vector3.forward);

        Vector3 player_pos_offset = headset_position + look * offset;

        //transform.SetPositionAndRotation(player_pos_offset, headset_rotation);

        // Smoothly interpolate the position and rotation
        transform.position = Vector3.Lerp(transform.position, player_pos_offset, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, headset_rotation, smoothSpeed * Time.deltaTime);
    }
}
