using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Collision");
        int damage = 100;
        other.GetComponent<HealthPlayer>().TakeDamage(damage);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
