using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SockWaveCollider : MonoBehaviour {

    public float force = 5f;

	// Use this for initialization
	void Start () {
		
	}

    private void OnParticleCollision(GameObject other)
    {
        Rigidbody rig = other.transform.GetComponent<Rigidbody>();
        rig.AddExplosionForce(force, other.transform.position, 1f);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
