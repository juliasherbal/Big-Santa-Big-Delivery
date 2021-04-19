using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSantaOver : MonoBehaviour
{
    public float speed;
    Vector3 stoppingPoint;
    public float stoppingDistance = 50f;
    

    void Start()
    {
        speed = 0;
        stoppingPoint = transform.position - transform.forward * stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position - transform.forward * 7 + new Vector3(0, 1.5f, 0), -transform.forward + new Vector3(-50, 0, 0));
        if (Physics.Raycast(transform.position - transform.forward * 7 + new Vector3(0, 1.5f, 0), -transform.forward, out hit, 50))
        {
            if (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("partOfPlayer"))
            {
                speed = 20f;
            }
        }

        if (speed > 0)
        {
            transform.position = Vector3.Lerp(transform.position, stoppingPoint, Time.deltaTime); 
        }
    }
}
