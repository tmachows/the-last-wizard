using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utility
{
    public static IEnumerable<GameObject> OverlapSphere(Vector3 position, float radius,
        bool attachedRigidbody = true)
    {
        return Physics.OverlapSphere(position, radius).Select(x =>
        {
            if (attachedRigidbody && x.attachedRigidbody != null)
            {
                return x.attachedRigidbody.gameObject;
            }
            return x.gameObject;
        }
        );
    }
}
