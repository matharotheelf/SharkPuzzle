using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Vector3 CameraPlayerDisplacement;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = GameObject.FindWithTag("Player").transform.position + CameraPlayerDisplacement;

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = GameObject.FindWithTag("Player").transform.position + CameraPlayerDisplacement;
    }
}
