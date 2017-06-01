using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{

    [SerializeField] private GameObject _SpellToCast;

    void Awake()
    {
        if (_SpellToCast == null)
        {
            throw new Exception("Spell To Cast not inserted.");
        }
    }

    void Update () {
        if (Input.GetKeyDown("a"))
        {
            Instantiate(_SpellToCast, transform.position, Quaternion.Euler(90f, 0f, 0f));
        }    		
	}



}
