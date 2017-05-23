﻿using System;
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

    void Awake () {
        _Nav = GetComponent<NavMeshAgent>();
        _Animator = GetComponent<Animator>();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        _MovingAudioSource = audioSources[0];
        _BattleAudioSource = audioSources[1];
        _IsEnemyMoving = true;
    }
	
	void Update () {
	    if (Vector3.Distance(transform.position, _WizardPosition) > 1.2)
	    {
	        _Nav.SetDestination(_WizardPosition);
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
	    }
    }

    public void SetWizardPosition(Vector3 wizardPosition)
    {
        _WizardPosition = wizardPosition;
    }
}
