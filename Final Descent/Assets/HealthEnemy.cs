using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : MonoBehaviour {

    public float Health = 100f;
    public int Shield;

	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            float damage = col.gameObject.GetComponent<Mover>().Damage;
            ApplyDamage(damage);
        }
    }

    private void ApplyDamage(float damage)
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Hit");
            Health -= damage;
        }
    }
}
