using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    public float killRadius = 2.5f;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Shark")
        {
            if(Vector3.Distance(collider.transform.position, gameObject.transform.position) < killRadius)
            {
                NpcMotion shark = collider.gameObject.GetComponent<NpcMotion>();
                shark.Kill();
            }
        }

    }
}
