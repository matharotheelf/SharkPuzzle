using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    [SerializeField] float wigglePauseMinimum = 1f;
    [SerializeField] float wigglePauseMaximum = 5f;
    [SerializeField] float wiggleDuration = 6f;
    [SerializeField] float wiggleAngle = 15f;

    private bool isWiggling = false;
    private float wiggleStartTime;

    void Start()
    {
        // Start intermittent wiggling
        StartCoroutine(Wiggle());
    }

    void Update()
    {
        if(isWiggling) {

            // Rotate branch forward and backward to wiggle
            float angle = 2*wiggleAngle*Time.deltaTime / wiggleDuration;

            if (2 * (Time.time - wiggleStartTime) < wiggleDuration)
            {
                gameObject.transform.Rotate(angle * Vector3.forward);
            }
            else
            {
                gameObject.transform.Rotate(-angle * Vector3.forward);
            }

            // Complete wiggle and trigger coroutine
            if(Time.time >= wiggleDuration + wiggleStartTime)
            {
                isWiggling = false;
                StartCoroutine(Wiggle());
            }
        }
    }

    IEnumerator Wiggle()
    {
        // Waits a random number of second so branch is still
        yield return new WaitForSeconds(Random.Range(wigglePauseMinimum, wigglePauseMaximum));

        // Starts a wiggle
        isWiggling = true;
        wiggleStartTime = Time.time;
    }
}
