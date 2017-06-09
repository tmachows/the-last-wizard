using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard
{
    public class AttackMessage
    { }

    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        private float _Attack = 20.0f;
        [SerializeField]
        private float _Range = 4.0f;

        private void Receive(AttackMessage message)
        {
            var damageMessage = new DamagePlayerMessage
            {
                Value = _Attack
            };
            MessageDispatcher.Send(damageMessage, transform.position, _Range);
        }
    }
}
