using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemyMessage
{
    public float _Value;
}

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] private float _Health = 100f;

    private void Receive(DamageEnemyMessage message)
    {
        _Health -= message._Value;
        if (_Health < 0)
        {
            Destroy(gameObject);
        }
    }
}
