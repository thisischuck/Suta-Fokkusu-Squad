using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserForward : MonoBehaviour
{
    public float secondsToDeath = 5.0f;
    public Vector3 Velocity;
    public float Speed = 30.0f;
    private bool hasStarted;
    public int damage = 5;

    void Start()
    {
        transform.forward = Velocity;
        StartCour();
    }

    void Update()
    {
        transform.position += Velocity * Time.deltaTime * Speed;
    }

    IEnumerator DeathTime()
    {
        hasStarted = true;

        yield return new WaitForSeconds(secondsToDeath);

        Destroy(this.gameObject);
    }

    public void StartCour()
    {
        if (!hasStarted)
        {
            StartCoroutine(DeathTime());
        }
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<HealthEnemy>())
        {
            other.GetComponent<HealthEnemy>().TakeDamage(15);
            GameObject stats = GameObject.Find("Stats");

            float enemyCurrenhp = other.GetComponent<HealthEnemy>().health;
            float enemyMaxhp = other.GetComponent<HealthEnemy>().base_maxHealth;
            string enemyName = "";
            if (other.GetComponent<Enemy>())
            {
                enemyName = other.GetComponent<Enemy>().enemyName;
                stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
            }
            else if (other.GetComponentInChildren<SpawnerBehaviour>())
            {
                enemyName = other.GetComponentInChildren<SpawnerBehaviour>().spawnerName;
                stats.GetComponent<DynamicHud>().SetEnemyStats(enemyName, enemyMaxhp, enemyCurrenhp);
            }

        }
    }
}
