using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class


    NpcMotion : MonoBehaviour
{
    enum SharkState
    {
        Killing,
        Killed,
        Pounching,
        Searching
    }

    public NavMeshAgent navMeshAgent;
    public Transform jawRotationPoint;
    public float pathEndThreshold = 0.1f;
    public float stepRange = 5f;
    public float pounceSpeed = 10f;
    public float searchingSpeed = 1f;
    public float pounceAngularSpeed = 90f;
    public float searchingAngularSpeed = 20f;
    public float killDuration = 1f;
    public float openJawAngle = 30f;

    private Vector3 killPosition;
    private Quaternion killRotation;
    private float killStartTime;
    private Vector3 playerPosition;
    private Vector3 point;
    private bool hasPath = false;

    SharkState sharkState = SharkState.Searching;

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

    private void Update()
    {
        UnityEngine.AI.NavMeshHit hit;
        float distanceToEdge = 1;

        if (!(sharkState is SharkState.Killing or SharkState.Killed))
        {
            if (UnityEngine.AI.NavMesh.FindClosestEdge(transform.position, out hit, UnityEngine.AI.NavMesh.AllAreas))
            {
                distanceToEdge = hit.distance;
            }

            if (AtEndOfPath() || !hasPath)
            {
                if (distanceToEdge < 1f)
                {
                    RandomPoint(transform.position, -stepRange, out point);
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

            Quaternion moveRotation = Quaternion.Lerp(transform.rotation, killRotation, t);
            transform.rotation = moveRotation;

            transform.Find("Head").RotateAround(jawRotationPoint.position, jawRotationPoint.right, openJawAngle * Time.deltaTime/killDuration);
            transform.Find("Jaw").RotateAround(jawRotationPoint.position, jawRotationPoint.right, -openJawAngle * Time.deltaTime / killDuration);

            sharkState = Vector3.Distance(transform.position, killPosition) <= 0.01f ? SharkState.Killed : SharkState.Killing;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "PounceTrigger")
        {
            sharkState = SharkState.Pounching;
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            navMeshAgent.destination = playerPosition;
            navMeshAgent.speed = pounceSpeed;
            navMeshAgent.angularSpeed = pounceAngularSpeed;
        }

        if (collider.gameObject.tag == "KillTrigger")
        {
            sharkState = SharkState.Killing;
            float sharkHeadBodyDistance = Vector3.Distance(transform.Find("HeadPosition").position, transform.position);
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

            killRotation = Quaternion.LookRotation(playerPosition - transform.position);
            killPosition = playerPosition - sharkHeadBodyDistance*(playerPosition - transform.position).normalized;
            killStartTime = Time.time;
            navMeshAgent.enabled = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "PounceTrigger")
        {
            navMeshAgent.speed = searchingSpeed;
            navMeshAgent.angularSpeed = searchingAngularSpeed;
            sharkState = SharkState.Searching;
        }
    }
}

//public Rigidbody rigidBody;
//public Vector3 StartVelocity;
//public Component playerContrllerScript;
//float timePassed = 0f;
//Vector3 acceleration = new Vector3(0, 0, 0);
//bool isNearWall;
//bool isPouncing;
//bool isKilling;
//Vector3 sharkHeadBodyDisplacement;
//Vector3 playerPosition;
//Vector3 sharkPlayerDisplacementHorizontal;
//Vector3 pouncingVelocity;
//float killStartTime;
//float killTime;
//Vector3 lerpingEndPos;

//// Start is called before the first frame update
//void Start()
//{
//    // set starting veloicity
//    rigidBody.velocity = StartVelocity;
//    rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
//}

//void FixedUpdate()
//{
//    timePassed += Time.fixedDeltaTime;

//    if (rigidBody.velocity.magnitude > 0.01f)
//    {
//        gameObject.transform.parent.forward = rigidBody.velocity.normalized;
//    }

//    rigidBody.AddForce(acceleration, ForceMode.Acceleration);

//    if (isKilling)
//    {
//        float t = (Time.time - killStartTime) / killTime;
//        Vector3 movePosition = Vector3.Lerp(transform.position, lerpingEndPos, t);
//        rigidBody.MovePosition(movePosition);
//        isKilling = Vector3.Distance(transform.position, lerpingEndPos) <= 0.01f ? false : true;
//    }

//    if (isPouncing || isNearWall)
//    {
//        return;
//    }

//    if (rigidBody.velocity.magnitude > 0.5f)
//    {
//        rigidBody.velocity = rigidBody.velocity.normalized * 0.5f;
//    }


//    //if (sharkPlayerDisplacementHorizontal.magnitude < 15)
//    //{
//    //    acceleration = 0.05f * sharkPlayerDisplacementHorizontal.normalized;
//    //} else
//    //{
//        if (timePassed > 5f)
//        {
//            acceleration.Set(Random.Range(-0.01f, 0.01f), 0, Random.Range(-0.01f, 0.01f));
//            timePassed = 0f;
//        }
//    //}
//}

//private void OnTriggerEnter(Collider collider)
//{
//    if (collider.gameObject.tag == "PounceTrigger")
//    {
//        isPouncing = true;
//        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
//        sharkPlayerDisplacementHorizontal = new Vector3(-(rigidBody.position - playerPosition).x, 0, -(rigidBody.position - playerPosition).z);
//        pouncingVelocity = 1f*sharkPlayerDisplacementHorizontal.normalized;

//        rigidBody.velocity = pouncingVelocity;
//        acceleration = Vector3.zero;
//    }

//    if (collider.gameObject.tag == "KillTrigger")
//    {
//        isKilling = true;
//        sharkHeadBodyDisplacement = transform.Find("HeadPosition").position - transform.position;
//        lerpingEndPos = GameObject.FindGameObjectWithTag("Player").transform.position - sharkHeadBodyDisplacement;
//        killStartTime = Time.time;
//        killTime = 1f;
//        rigidBody.velocity = Vector3.zero;
//        GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
//    }

//    if (collider.gameObject.tag == "WallTrigger")
//    {
//        isNearWall = true;
//    }
//}

//private void OnTriggerExit(Collider collider)
//{
//    if (collider.gameObject.tag == "WallTrigger")
//    {
//        isNearWall = false;
//    }

//    if (collider.gameObject.tag == "PounceTrigger")
//    {
//        isPouncing = false;
//        timePassed = 5f;
//    }
//}

//private void OnTriggerStay(Collider collider)
//{
//    if (collider.gameObject.tag == "WallTrigger") {
//        acceleration = 0.01f * collider.gameObject.transform.forward;
//    }
//}
