﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushSpellController : MonoBehaviour
{

    #region inspector variables
    [SerializeField] private float _Duration = 5;
    [SerializeField] private float _EndSize = 5;
    #endregion

    #region variables
    private float _Time = 0.0f;
    #endregion

    void Start()
    {
        GetComponent<ParticleSystem>().Play();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
	
	void Update () {
	    if (_Time > _Duration)
	    {
	        Destroy(gameObject);
	    }
	    else
	    {
	        _Time += Time.deltaTime;
	        transform.localScale = Vector3.one * (Mathf.Lerp(0.0f, _EndSize, _Time/_Duration));

//            GetComponent<SphereCollider>().radius = Mathf.Lerp(0.0f, _EndSize, _Time/_Duration);
	    }
	}
}