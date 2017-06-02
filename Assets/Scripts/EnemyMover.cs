using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMover : MonoBehaviour {

    private Vector3 _WizardPosition;
    private NavMeshAgent _Nav;
    private Animator _Animator;
    private AudioSource _MovingAudioSource;
    private AudioSource _BattleAudioSource;
    private bool _IsEnemyMoving;

    [SerializeField] private float _AttackInterval = 1.5f;
    private float _TimeToNextAttack;

    void Awake () {
        _Nav = GetComponent<NavMeshAgent>();
        _Animator = GetComponent<Animator>();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        _MovingAudioSource = audioSources[0];
        _BattleAudioSource = audioSources[1];
        _IsEnemyMoving = true;
        _TimeToNextAttack = _AttackInterval;
    }
	
	void Update () {
	    if (Vector3.Distance(transform.position, _WizardPosition) > 1.2)
	    {
	        _Nav.SetDestination(_WizardPosition);
	        _IsEnemyMoving = true;
	    }
	    else
        {
            if (_IsEnemyMoving)
            {
                _MovingAudioSource.Stop();
                _BattleAudioSource.Play();
            }
            _IsEnemyMoving = false;
            _Animator.SetBool("CanReachHero", true);

            _TimeToNextAttack -= Time.deltaTime;
            if (_TimeToNextAttack < 0)
            {
                var message = new AttackMessage();
                MessageDispatcher.Send(message, gameObject);
                _TimeToNextAttack = _AttackInterval;
            }
        }
    }

    public void SetWizardPosition(Vector3 wizardPosition)
    {
        _WizardPosition = wizardPosition;
    }
}
