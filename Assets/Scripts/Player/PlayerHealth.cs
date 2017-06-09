using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard
{
    public class DamagePlayerMessage
    {
        public float Value;
    }

    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        private float _Value = 100.0f;
        [SerializeField]
        private Canvas _Canvas;
        private HudView _HUD;

        private float _MaxValue;

        void Awake()
        {
            _MaxValue = _Value;
            _HUD = _Canvas.GetComponent<HudView>();
        }

        private void Receive(HealingSpellMessage message)
        {
            if (_Value + message.Value > _MaxValue)
            {
                _Value = _MaxValue;
            }
            else
            {
                _Value += message.Value;
            }
            _HUD.Health = _Value;
        }

        private void Receive(DamagePlayerMessage message)
        {

            _Value -= message.Value;
            _HUD.Health = _Value;
            if (_Value < 0.0f)
            {
                Debug.Log("health < 0");
                var gameOverMessage = new GameOverMessage()
                {
                    Sender = this
                };
                MessageDispatcher.Send(gameOverMessage, transform.position, 3.0f);
            }
        }
    }
}
