using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCollider : MonoBehaviour {

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Collision");
        other.GetComponent<HealthPlayer>().TakeDamage(10);
    }
}
