using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCVision : MonoBehaviour
{
    public GameObject player;
    public int maxFOV;
    public int sightDistance;

    public bool seen = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInFov())
        {
            if (!seen)
            {
                Debug.Log("Fart!");
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("NPC Spawner"))
                {
                    obj.GetComponent<NPCSpawner>().Spawn();
                }
            }
        }
        OnDrawGizmos();
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
        Vector3 frontView = (transform.forward * sightDistance);
        Vector3 left = Quaternion.Euler(0, maxFOV * 0.5f, 0) * frontView;
        Vector3 right = Quaternion.Euler(0, -maxFOV * 0.5f, 0) * frontView;

        Debug.DrawLine(transform.position, transform.position + frontView, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + left, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + right, Color.yellow);
    }
}
