using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMover : MonoBehaviour {

    private Vector3 _WizardPosition;
    private NavMeshAgent _Nav;

    void Awake () {
        _WizardPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        _Nav = GetComponent<NavMeshAgent>();
    }
	
	void Update () {
        _Nav.SetDestination(_WizardPosition);
    }
}
