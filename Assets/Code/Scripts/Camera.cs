using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Vector3 CameraPlayerDisplacement;
    [SerializeField] GameObject player;
    [SerializeField] float rotationSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.transform.position + CameraPlayerDisplacement;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("RotateCamera"))
        {
            CameraPlayerDisplacement = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up) * CameraPlayerDisplacement;
        }
        transform.position = player.transform.position + CameraPlayerDisplacement;
        transform.LookAt(player.transform);
    }
}
