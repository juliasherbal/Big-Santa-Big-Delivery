using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum FSMStates
    {
        Patrol,
        Chase,
        Capture
    }

    public float patrolSpeed;
    public float chaseSpeed;

    public List<GameObject> checkpoints;
    public GameObject currentGoal;
    public NavMeshAgent agent;

    public GameObject player;
    public int maxFOV;
    public int sightDistance;
    public float captureDistance;

    public FSMStates currentState;
    public float chaseTime;
    public float maxChaseTime;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        FindNextGoal();
        agent = GetComponent<NavMeshAgent>();
        currentState = FSMStates.Patrol;
        chaseTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case FSMStates.Patrol:
                Patrol();
                break;
            case FSMStates.Chase:
                Chase();
                break;
            case FSMStates.Capture:
                Capture();
                break;
        }
    }

    void Patrol()
    {
        agent.stoppingDistance = 0;

        agent.speed = patrolSpeed;

        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (Vector3.Distance(transform.position, currentGoal.transform.position) < 1.5)
        {
            FindNextGoal();
        }
        if (distanceFromPlayer <= sightDistance && PlayerInFov())
        {
            Debug.Log("Spotted OMG!");
            currentState = FSMStates.Chase;
        }

        FaceTarget(currentGoal.transform.position);

        agent.SetDestination(currentGoal.transform.position);
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
            FindNextGoal();
            currentState = FSMStates.Patrol;
        }
    }

    void Capture()
    {
        agent.SetDestination(transform.position);
        transform.Rotate(0, 10f, 0);
        player.transform.position = GameObject.FindGameObjectWithTag("startingPos").transform.position;
        GameObject.Find("LevelManager").GetComponent<LevelManager>().countDown -= 30;
        currentState = FSMStates.Patrol;
    }

    void FindNextGoal()
    {
        int rand = Random.Range(0, checkpoints.Count);
        currentGoal = checkpoints[rand];
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
