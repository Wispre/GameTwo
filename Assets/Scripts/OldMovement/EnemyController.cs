using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Transform target;
    private Vector3 spawnSpot;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        spawnSpot = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
            agent.stoppingDistance = 2f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            target = null;
            agent.stoppingDistance = 0f;
            agent.SetDestination(spawnSpot);
        }
    }

    private void Update()
    {
        if(target != null)
        {
            agent.SetDestination(target.position);
        }
    }
}
