using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifepodCollider : MonoBehaviour {

	public Transform player;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
			player.GetComponent<HealthPlayer>().TakeDamage(100);
	}
}
