using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcMotion : MonoBehaviour
{
    enum SharkState
    {
        Killing,
        Killed,
        Pounching,
        Searching
    }

    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] GameObject navMeshSurface;
    [SerializeField] Transform jawRotationPoint;
    [SerializeField] GameScreen gameOverScreen;
    [SerializeField] float pathEndThreshold = 0.1f;
    [SerializeField] float stepRange = 5f;
    [SerializeField] float pounceSpeed = 10f;
    [SerializeField] float searchingSpeed = 1f;
    [SerializeField] float pounceAngularSpeed = 90f;
    [SerializeField] float searchingAngularSpeed = 20f;
    [SerializeField] float killDuration = 1f;
    [SerializeField] float openJawAngle = 30f;

    private Vector3 killPosition;
    private Quaternion killRotation;
    private float killStartTime;
    private Vector3 playerPosition;
    private Vector3 point;
    private bool hasPath = false;

    private SharkState sharkState = SharkState.Searching;

    // code from: https://docs.unity3d.com/560/Documentation/Manual/nav-MoveToDestination.html
    bool RandomPoint(Vector3 center, float sRange, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.Range(sRange / 2, sRange) * transform.forward + Random.Range(-sRange, sRange) * transform.right;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    bool AtEndOfPath()
    {
        hasPath |= navMeshAgent.hasPath;
        if (hasPath && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + pathEndThreshold)
        {
            // Arrived
            hasPath = false;
            return true;
        }

        return false;
    }

    private void GameOver()
    {
        gameOverScreen.Setup("GameOverScreen");
    }

    public void StartPounce()
    {
        sharkState = SharkState.Pounching;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        navMeshAgent.destination = playerPosition;
        navMeshAgent.speed = pounceSpeed;
        navMeshAgent.angularSpeed = pounceAngularSpeed;
    }

    public void EndPounce()
    {
        navMeshAgent.speed = searchingSpeed;
        navMeshAgent.angularSpeed = searchingAngularSpeed;
        sharkState = SharkState.Searching;
    }

    public void Kill()
    {
        sharkState = SharkState.Killing;
        float sharkHeadBodyDistance = Vector3.Distance(transform.Find("HeadPosition").position, transform.position);
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        killRotation = Quaternion.LookRotation(playerPosition - transform.position);
        killPosition = playerPosition - sharkHeadBodyDistance * (playerPosition - transform.position).normalized;
        killStartTime = Time.time;
        navMeshAgent.enabled = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
    }

    private void Update()
    {
        UnityEngine.AI.NavMeshHit hit;
        float distanceToEdge = 1;

        if (!(sharkState is SharkState.Killing or SharkState.Killed))
        {
            if (UnityEngine.AI.NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
            {
                distanceToEdge = hit.distance;
            }


            if (AtEndOfPath() || !hasPath)
            {
                if (distanceToEdge < 1f)
                {
                    point = navMeshSurface.transform.position;
                }
                else
                {
                    RandomPoint(transform.position, stepRange, out point);
                }

                navMeshAgent.destination = point;
                hasPath = true;
            }
        }

        if (sharkState == SharkState.Killing)
        {
            float t = (Time.time - killStartTime) / killDuration;
            Vector3 movePosition = Vector3.Lerp(transform.position, killPosition, t);
            transform.position = movePosition;

            Quaternion moveRotation = Quaternion.Slerp(transform.rotation, killRotation, t);
            transform.rotation = moveRotation;

            transform.Find("Head").RotateAround(jawRotationPoint.position, jawRotationPoint.right, openJawAngle * Time.deltaTime/killDuration);
            transform.Find("Jaw").RotateAround(jawRotationPoint.position, jawRotationPoint.right, -openJawAngle * Time.deltaTime / killDuration);

            sharkState = Vector3.Distance(transform.position, killPosition) <= 0.01f ? SharkState.Killed : SharkState.Killing;

            if (sharkState == SharkState.Killed)
            {
                GameOver();
            }
        }
    }
}
