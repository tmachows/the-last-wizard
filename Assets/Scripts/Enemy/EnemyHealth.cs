using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard
{
    public class DamageEnemyMessage
    {
        public float _Value;
    }

    public class EnemyHealth : MonoBehaviour
    {

        [SerializeField]
        private float _Health = 100f;

        private void Receive(DamageEnemyMessage message)
        {
            _Health -= message._Value;
            if (_Health < 0)
            {
                var enemyDeathMessage = new KilledEnemyMessage
                {
                    Points = 10
                };
                MessageDispatcher.Send(enemyDeathMessage, transform.position, 20.0f);

                Destroy(gameObject);
            }
        }
    }
}