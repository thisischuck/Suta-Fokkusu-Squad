using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : BaseStats
{
	public SkinnedMeshRenderer mesh;
	Color originalColor;

	private void Start()
	{
		//GenerateVariables(health, 0);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Bullet")
		{
			FlashOnHit();

			CheckWhatWepon(other.gameObject);

			GameObject stats = GameObject.Find("Stats");

			float enemyCurrenhp = GetComponent<HealthEnemy>().health;
			float enemyMaxhp = GetComponent<HealthEnemy>().base_maxHealth;
			string enemyName = "";
			if (GetComponent<Enemy>())
			{
				enemyName = GetComponent<Enemy>().enemyName;
				stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
			}
			else if (GetComponentInChildren<SpawnerBehaviour>())
			{
				enemyName = GetComponentInChildren<SpawnerBehaviour>().spawnerName;
				stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
			}
		}
	}

	private void CheckWhatWepon(GameObject other)
	{
		if (other.name == "missile(Clone)" || other.name == "miniMissile(Clone)")
		{
			TakeDamage(10);
		}
		if (other.name == "missileBig(Clone)")
		{
			TakeDamage(13);
		}
		if (other.name == "EnergyBullet(Clone)")
		{
			float damage = 2.5f * other.transform.localScale.y;
			TakeDamage(damage);
		}
		Destroy(other.gameObject);
	}

	public void FlashOnHit()
	{
		StartCoroutine(IenumFlashOnHit());
	}

	IEnumerator IenumFlashOnHit()
	{
		mesh.material.color = new Color(Color.red.r * 100, Color.red.g, Color.red.b * 100);
		yield return new WaitForSeconds(0.2f);
		mesh.material.color = originalColor;
	}
}
