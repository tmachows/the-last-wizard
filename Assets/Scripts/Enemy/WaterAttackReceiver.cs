using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard
{
    public class WaterAttackReceiver : MonoBehaviour
    {

        private void Receive(WaterSpellMessage message)
        {
            var damageMessage = new DamageEnemyMessage()
            {
                _Value = message.Power
            };
            MessageDispatcher.Send(damageMessage, gameObject);
        }
    }
}