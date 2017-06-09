using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard
{
    #region spell messages
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
    #endregion


    public class SpellCaster : MonoBehaviour
    {
        #region inspector variables
        [SerializeField]
        private GameObject _PushSpell;
        [SerializeField]
        private GameObject _FireSpell;
        [SerializeField]
        private GameObject _WaterSpell;
        [SerializeField]
        private GameObject _HealingSpell;
        [SerializeField]
        private float _Radius = 5.0f;
        [SerializeField]
        private float _FirePower = 30.0f;
        [SerializeField]
        private float _WaterPower = 30.0f;
        [SerializeField]
        private float _HealingPower = 10.0f;
        [SerializeField]
        private float _BaseCooldown = 3.0f;
        [SerializeField]
        private float _PushSpellCooldown = 0.0f;
        [SerializeField]
        private float _FireSpellCooldown = 0.0f;
        [SerializeField]
        private float _WaterSpellCooldown = 0.0f;
        [SerializeField]
        private float _HealingSpellCooldown = 0.0f;
        [SerializeField]
        private Animator _PushSpellAnimator;
        [SerializeField]
        private Animator _FireSpellAnimator;
        [SerializeField]
        private Animator _WaterSpellAnimator;
        [SerializeField]
        private Animator _HealingSpellAnimator;
        #endregion


        void Awake()
        {
            if (_PushSpell == null || _FireSpell == null || _WaterSpell == null || _HealingSpell == null)
            {
                throw new Exception("Spell not inserted.");
            }
        }

        public void CastSpell(Result result)
        {
            if (!result.matched)
            {
                return;
            }
            if (result.name.Equals("v"))
            {
                if (_PushSpellCooldown <= 0)
                {
                    var message = new PushSpellMessage()
                    {
                        Sender = this
                    };
                    CastSpell(_PushSpell, message);
                    _PushSpellCooldown = _BaseCooldown;
                    _PushSpellAnimator.SetTrigger("SpellCasted");
                }
            }
            else if (result.name.Equals("caret"))
            {
                if (_FireSpellCooldown <= 0)
                {
                    var message = new FireSpellMessage()
                    {
                        Sender = this,
                        Power = _FirePower
                    };
                    CastSpell(_FireSpell, message);
                    _FireSpellCooldown = _BaseCooldown;
                    _FireSpellAnimator.SetTrigger("SpellCasted");
                }
            }
            else if (result.name.Equals("circle"))
            {
                if (_WaterSpellCooldown <= 0)
                {
                    var message = new WaterSpellMessage()
                    {
                        Sender = this,
                        Power = _WaterPower
                    };
                    CastSpell(_WaterSpell, message);
                    _WaterSpellCooldown = _BaseCooldown;
                    _WaterSpellAnimator.SetTrigger("SpellCasted");
                }
            }
            else if (result.name.Equals("rectangle"))
            {
                if (_HealingSpellCooldown <= 0)
                {
                    var message = new HealingSpellMessage()
                    {
                        Sender = this,
                        Value = _HealingPower
                    };
                    CastSpell(_HealingSpell, message);
                    _HealingSpellCooldown = _BaseCooldown;
                    _HealingSpellAnimator.SetTrigger("SpellCasted");
                }
            }
        }

        void CastSpell<T>(GameObject spell, T message) where T : class
        {
            Instantiate(spell, transform.position, Quaternion.Euler(90f, 0f, 0f));
            MessageDispatcher.Send(message, transform.position, _Radius);
        }

        public void Update()
        {
            if (Input.GetKeyDown("a"))
            {
                var message = new FireSpellMessage()
                {
                    Sender = this
                };
                CastSpell(_FireSpell, message);
            }

            if (_PushSpellCooldown > 0)
            {
                _PushSpellCooldown -= Time.deltaTime;
            }
            if (_FireSpellCooldown > 0)
            {
                _FireSpellCooldown -= Time.deltaTime;
            }
            if (_WaterSpellCooldown > 0)
            {
                _WaterSpellCooldown -= Time.deltaTime;
            }
            if (_HealingSpellCooldown > 0)
            {
                _HealingSpellCooldown -= Time.deltaTime;
            }
        }
    }
}