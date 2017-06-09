using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushSpellController : EffectOnlySpellController
{
    void OtherEffect()
    {
        transform.localScale = Vector3.one * (Mathf.Lerp(0.0f, _EndSize, _Time / _Duration));
    }
}
