using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PounceTrigger : MonoBehaviour
{
    [SerializeField] float pounceRadius = 8f;

    void OnTriggerEnter(Collider collider)
    {
        // this condition is necessary because the trigger activates unpredicatably
        // this ensures the shark only pounces when it is close to the fish
        if (Vector3.Distance(collider.transform.position, gameObject.transform.position) < pounceRadius)
        {
            // initiates the shark to pounce at the target
            if (collider.gameObject.tag == "Shark")
            {
                NpcMotion shark = collider.gameObject.GetComponent<NpcMotion>();
                shark.StartPounce();
            }
        }
        
    }

    void OnTriggerExit(Collider collider)
    {
        // if the shark leaves the pounce area they stop pouncing and return to normal behaviour
        if (collider.gameObject.tag == "Shark")
        {
            NpcMotion shark = collider.gameObject.GetComponent<NpcMotion>();
            shark.EndPounce();
        }

    }
}
