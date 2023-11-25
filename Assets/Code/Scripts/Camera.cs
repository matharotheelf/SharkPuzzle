using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Vector3 CameraPlayerDisplacement;
    [SerializeField] GameObject player;
    [SerializeField] float rotationSpeed = 50f;

    void Start()
    {
        // move camera to start position behind fish
        transform.position = player.transform.position + CameraPlayerDisplacement;
    }

    void Update()
    {
        // rotates camera if the rotate button is pressed
        if (Input.GetButton("RotateCamera"))
        {
            CameraPlayerDisplacement = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up) * CameraPlayerDisplacement;
        }

        // moves camera to rotates position and faces at fish
        transform.position = player.transform.position + CameraPlayerDisplacement;
        transform.LookAt(player.transform);
    }
}
