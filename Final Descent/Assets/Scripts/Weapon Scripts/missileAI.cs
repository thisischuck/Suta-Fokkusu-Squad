using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileAI : MonoBehaviour {

	private Transform target;

	public GameObject miniMissiles;

	public float speed = 0, rotSpeed = 0;
	public Vector3 Velocity;
	public float lifeTime;
	public bool explosive = false;

	private float timer = 0.0f; 

	void Start () {

		if (!explosive)
			lifeTime = 5f;
		else lifeTime = 4f;

		transform.forward = Velocity;
	}

	void Update()
	{
		timer += Time.deltaTime;

		if (LookForEnemy())
		{
			Velocity = EnemyBehaviours.Pursuit(transform, Velocity, target, 1);
			transform.forward = Velocity.normalized;
		}
		transform.position += Velocity * speed * Time.deltaTime;

		if (timer >= lifeTime && !explosive)
			Destroy(this.gameObject);
		else if(timer >= lifeTime && explosive)
		{
			SpawnMiniMissiles(10);
			Destroy(this.gameObject);
		}
	}

	private void SpawnMiniMissiles(int count)
	{
		for( int i = 0; i <= count; i++)
		{
			Vector3 spherePoint = Random.insideUnitSphere * 3 + this.transform.position;
			GameObject minimissile = Instantiate(miniMissiles, spherePoint, this.transform.rotation);
		}
	}

	private bool LookForEnemy()
	{
		float distance = 0.0f;
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject e in enemies)
		{
			float d = Vector3.Distance(this.transform.position, e.transform.position);
			if (distance > d || distance == 0)
			{
				distance = d;
				target = e.transform;
			}
		}
		if (distance > 30f)
			return false;
		else return true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Enemy")
		{
			Debug.Log("hit");
			//collision.gameObject.SendMessage(takeDamage(damage));
			Destroy(this.gameObject);
		}
	}
}
