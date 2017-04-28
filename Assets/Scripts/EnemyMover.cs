using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMover : MonoBehaviour {

    private Vector3 _WizardPosition;
    private NavMeshAgent _Nav;
    private Animator _Animator;

    void Awake () {
        _Nav = GetComponent<NavMeshAgent>();
        _Animator = GetComponent<Animator>();
    }
	
	void Update () {
	    if (Vector3.Distance(transform.position, _WizardPosition) > 1.2)
	    {
	        _Nav.SetDestination(_WizardPosition);
	    }
	    else
	    {
	        _Animator.SetBool("CanReachHero", true);
	    }
    }

    public void SetWizardPosition(Vector3 wizardPosition)
    {
        _WizardPosition = wizardPosition;
    }
}
