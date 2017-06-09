using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard
{
    public class FireAttackReceiver : MonoBehaviour
    {

        private void Receive(FireSpellMessage message)
        {
            var damageMessage = new DamageEnemyMessage()
            {
                _Value = message.Power
            };
            MessageDispatcher.Send(damageMessage, gameObject);
        }
    }
}
