using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushSpellMessage
{
    public Component Sender;
}

public class FireSpellMessage
{
    public Component Sender;
    public float Power;
}

public class WaterSpellMessage
{
    public Component Sender;
    public float Power;
}

public class HealingSpellMessage
{
    public Component Sender;
    public float Value;
}

public class SpellCaster : MonoBehaviour
{

    [SerializeField] private GameObject _PushSpell;
    [SerializeField] private GameObject _FireSpell;
    [SerializeField] private GameObject _WaterSpell;
    [SerializeField] private GameObject _HealingSpell;
    [SerializeField] private float _Radius = 5.0f;
    [SerializeField] private float _FirePower = 30.0f;
    [SerializeField] private float _WaterPower = 30.0f;
    [SerializeField] private float _HealingPower = 10.0f;

    void Awake()
    {
        if (_PushSpell == null || _FireSpell == null || _WaterSpell == null || _HealingSpell == null)
        {
            throw new Exception("Spell not inserted.");
        }
    }

    void Update () {
        if (Input.GetKeyDown("a"))
        {
            var message = new PushSpellMessage()
            {
                Sender = this
            };
            CastSpell(_PushSpell, message);
        }
        if (Input.GetKeyDown("s"))
        {
            var message = new FireSpellMessage()
            {
                Sender = this,
                Power = _FirePower
            };
            CastSpell(_FireSpell, message);
        }
        if (Input.GetKeyDown("d"))
        {
            var message = new WaterSpellMessage()
            {
                Sender = this,
                Power = _WaterPower
            };
            CastSpell(_WaterSpell, message);
        }
        if (Input.GetKeyDown("f"))
        {
            var message = new HealingSpellMessage()
            {
                Sender = this,
                Value = _HealingPower
            };
            CastSpell(_HealingSpell, message);
        }	
	}

    void CastSpell<T>(GameObject spell, T message) where T: class
    {
        Instantiate(spell, transform.position, Quaternion.Euler(90f, 0f, 0f));
        MessageDispatcher.Send(message, transform.position, _Radius);
    }
}
