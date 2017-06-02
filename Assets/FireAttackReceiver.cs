using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttackReceiver : MonoBehaviour {

    private void Receive(FireSpellMessage message)
    {
        Debug.Log("fire attact receiver received message");
        var damageMessage = new DamageEnemyMessage()
        {
            _Value = message.Power
        };
        MessageDispatcher.Send(damageMessage, gameObject);
    }
}
