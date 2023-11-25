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
        Pouncing,
        Searching
    }

    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] GameObject navMeshSurface;
    [SerializeField] GameObject player;
    [SerializeField] Transform jawRotationPoint;
    [SerializeField] GameScreen gameOverScreen;
    [SerializeField] float pathEndThreshold = 0.1f;
    [SerializeField] float stepRange = 5f;
    [SerializeField] float pounceSpeed = 10f;
    [SerializeField] float pounceAcceleration = 8f;
    [SerializeField] float searchingAcceleration = 0.5f;
    [SerializeField] float searchingSpeed = 1f;
    [SerializeField] float pounceAngularSpeed = 90f;
    [SerializeField] float searchingAngularSpeed = 20f;
    [SerializeField] float killDuration = 1f;
    [SerializeField] float openJawAngle = 30f;
    [SerializeField] AudioSource Audio;
    [SerializeField] AudioClip KillSoundClip;

    private Vector3 killPosition;
    private Quaternion killRotation;
    private float killStartTime;
    private Vector3 playerPosition;
    private Vector3 point;
    private bool hasPath = false;

    private SharkState sharkState = SharkState.Searching;

    // Random point generator within the range of the shark for shark random path
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

    // Bool which returns whether a shark has reached the end of its path
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

    // Bool which returns whether shark is next to the navmesh edge
    bool AtEdgeOfMesh()
    {
        UnityEngine.AI.NavMeshHit hit;
        float distanceToEdge = 1;

        if (UnityEngine.AI.NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
        {
            distanceToEdge = hit.distance;
        }

        return distanceToEdge < 1f;
    }

    // Generates next destination for shark
    void GenerateDestination()
    {
        if (AtEdgeOfMesh())
        {
            point = navMeshSurface.transform.position;
        }
        else
        {
            // If the shark is at the adge of the navme
            RandomPoint(transform.position, stepRange, out point);
        }

        navMeshAgent.destination = point;
        hasPath = true;
    }

    // Lerps shark position and rotation for action of killing the fish
    void KillLerp()
    {
        // Parameter for lerping
        float t = (Time.time - killStartTime) / killDuration;

        // Moves the shark to the next position in lerp
        Vector3 movePosition = Vector3.Lerp(transform.position, killPosition, t);
        transform.position = movePosition;

        // Rotates shark in lerp
        Quaternion moveRotation = Quaternion.Slerp(transform.rotation, killRotation, t);
        transform.rotation = moveRotation;

        // Opens Jaw of shark
        transform.Find("Head").RotateAround(jawRotationPoint.position, jawRotationPoint.right, openJawAngle * Time.deltaTime / killDuration);
        transform.Find("JawHolder").Find("Jaw").RotateAround(jawRotationPoint.position, jawRotationPoint.right, -openJawAngle * Time.deltaTime / killDuration);

        // Transition shark to final Killed state if lerp over
        sharkState = Vector3.Distance(transform.position, killPosition) <= 0.01f ? SharkState.Killed : SharkState.Killing;

        // Game over after kill
        if (sharkState == SharkState.Killed)
        {
            GameOver();
        }
    }

    // Triggers Game over screen
    private void GameOver()
    {
        gameOverScreen.Setup("GameOverScreen");
    }

    public void StartPounce()
    {
        sharkState = SharkState.Pouncing;
        // Shark moves towards player
        playerPosition = player.transform.position;
        navMeshAgent.destination = playerPosition;
        // Shark speeds up and becomes more agile
        navMeshAgent.speed = pounceSpeed;
        navMeshAgent.acceleration= pounceAcceleration;
        navMeshAgent.angularSpeed = pounceAngularSpeed;
    }


    public void EndPounce()
    {
        // Shark returns to usual behaviour
        navMeshAgent.speed = searchingSpeed;
        navMeshAgent.angularSpeed = searchingAngularSpeed;
        navMeshAgent.acceleration = searchingAcceleration;
        sharkState = SharkState.Searching;
    }

    // When a shark kills it lerps towards the fish
    public void Kill()
    {
        sharkState = SharkState.Killing;
        float sharkHeadBodyDistance = Vector3.Distance(transform.Find("HeadPosition").position, transform.position);
        playerPosition = player.transform.position;

        // shark target rotation to face toward fish
        killRotation = Quaternion.LookRotation(playerPosition - transform.position);

        // shark target position is so that the head of the shark is on the position of the fish
        killPosition = playerPosition - sharkHeadBodyDistance * (playerPosition - transform.position).normalized;

        // Time to start lerp towards fish
        killStartTime = Time.time;

        // Stops shark nav mesh movement
        navMeshAgent.enabled = false;

        // Plays kill audio
        Audio.PlayOneShot(KillSoundClip);

        // Disable fish movement as the user has died
        player.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
    }

    private void Update()
    {
        switch (sharkState)
        {
            case SharkState.Searching:
                if (AtEndOfPath() || !hasPath)
                {
                    GenerateDestination();
                }
                break;
            case SharkState.Pouncing:
                if (AtEndOfPath() || !hasPath)
                {
                    GenerateDestination();
                }
                break;
            case SharkState.Killing:
                KillLerp();
                break;
        }
    }
}
