using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastWizard
{
    public class TimeController : MonoBehaviour
    {

        private float _TimeOfGame = 0.0f;
        [SerializeField]
        private int _BonusPoints = 10;
        [SerializeField]
        private float _BonusInterval = 10.0f;
        private float _TimeToBonus;

        void Awake()
        {
            _TimeToBonus = _BonusInterval;
        }

        // Update is called once per frame
        void Update()
        {
            _TimeOfGame += Time.deltaTime;
            _TimeToBonus -= Time.deltaTime;
            if (_TimeToBonus < 0.0f)
            {
                var message = new TimeBonusMessage
                {
                    Points = _BonusPoints
                };
                MessageDispatcher.Send(message, gameObject);
                _TimeToBonus = _BonusInterval - (_TimeOfGame % _BonusInterval);
            }
        }
    }
}
