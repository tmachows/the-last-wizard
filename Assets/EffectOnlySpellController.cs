using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOnlySpellController : MonoBehaviour {

    #region inspector variables
    [SerializeField]
    protected float _Duration = 5;
    [SerializeField]
    protected float _EndSize = 5;
    #endregion

    #region variables
    protected float _Time = 0.0f;
    #endregion

    void Start()
    {
        GetComponent<ParticleSystem>().Play();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    void Update()
    {
        if (_Time > _Duration)
        {
            Destroy(gameObject);
        }
        else
        {
            _Time += Time.deltaTime;
            OtherEffect();
        }
    }

    void OtherEffect()
    {
    }
}
