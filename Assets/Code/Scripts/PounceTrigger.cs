using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PounceTrigger : MonoBehaviour
{
    public float pounceRadius = 8f;

    void OnTriggerEnter(Collider collider)
    {
        if (Vector3.Distance(collider.transform.position, gameObject.transform.position) < pounceRadius)
        {
            if (collider.gameObject.tag == "Shark")
            {
                NpcMotion shark = collider.gameObject.GetComponent<NpcMotion>();
                shark.StartPounce();
            }
        }
        
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Shark")
        {
            NpcMotion shark = collider.gameObject.GetComponent<NpcMotion>();
            shark.EndPounce();
        }

    }
}
