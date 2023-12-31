using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    [SerializeField] float killRadius = 4f;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Shark")
        {
            // this condition is necessary because the trigger activates unpredicatably
            // this ensures the shark only kills when it is close to the fish
            if (Vector3.Distance(collider.transform.position, gameObject.transform.position) < killRadius)
            {
                NpcMotion shark = collider.gameObject.GetComponent<NpcMotion>();
                // initiates the shark to kill the target
                shark.Kill();
            }
        }

    }
}
