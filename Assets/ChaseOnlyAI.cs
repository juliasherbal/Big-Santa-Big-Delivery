using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseOnlyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Idle,
        Chase,
        Capture
    }

    public float chaseSpeed;

    public GameObject currentGoal;
    public NavMeshAgent agent;

    public GameObject player;
    public int maxFOV;
    public int sightDistance;
    public float captureDistance;

    public FSMStates currentState;
    public float chaseTime;
    public float maxChaseTime;
    public bool isIdle;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentGoal = player;
        agent = GetComponent<NavMeshAgent>();
        currentState = FSMStates.Chase;
        if (isIdle)
        {
            currentState = FSMStates.Idle;
        }
        chaseTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case FSMStates.Idle:
                Idle();
                break;
            case FSMStates.Chase:
                Chase();
                break;
            case FSMStates.Capture:
                Capture();
                break;
        }
    }

    void Idle()
    {
        agent.SetDestination(transform.position);

        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceFromPlayer <= sightDistance && PlayerInFov())
        {
            Debug.Log("Spotted OMG!");
            currentState = FSMStates.Chase;
        }
    }

    void Chase()
    {
        chaseTime += Time.deltaTime;

        float distanceFromPlayer = Vector3.Distance(transform.position, currentGoal.transform.position);

        currentGoal = player;
        agent.stoppingDistance = 0;
        agent.speed = chaseSpeed;
        FaceTarget(currentGoal.transform.position);
        agent.SetDestination(currentGoal.transform.position);
        if (distanceFromPlayer <= captureDistance)
        {
            Debug.Log("Caught!");
            currentState = FSMStates.Capture;
        }

        if (chaseTime >= maxChaseTime)
        {
            chaseTime = 0;
            currentState = FSMStates.Idle;
        }
    }

    void Capture()
    {
        agent.SetDestination(transform.position);
        transform.Rotate(0, 10f, 0);
        player.transform.position = GameObject.FindGameObjectWithTag("startingPos").transform.position;
        GameObject.Find("LevelManager").GetComponent<LevelManager>().countDown -= 45;
        currentState = FSMStates.Idle;
    }


    bool PlayerInFov()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        if (Vector3.Angle(directionToPlayer, transform.forward) <= maxFOV)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        /*Vector3 frontView = (transform.forward * sightDistance);
        Vector3 left = Quaternion.Euler(0, maxFOV * 0.5f, 0) * frontView;
        Vector3 right = Quaternion.Euler(0, -maxFOV * 0.5f, 0) * frontView;

        Debug.DrawLine(transform.position, transform.position + frontView, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + left, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + right, Color.yellow);*/
    }

    void FaceTarget(Vector3 destination)
    {
        Vector3 directionToTarget = (destination - transform.position).normalized;
        directionToTarget.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
    }
}
