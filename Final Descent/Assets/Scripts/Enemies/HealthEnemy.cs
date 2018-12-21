using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : BaseStats
{
    public string name;
    private void Start()
    {
        GenerateVariables(100, 0);
        base_maxHealth = 100;
        //GetComponent<HBController>().StartHPBar(health);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<HBController>() != null)
            GetComponent<HBController>().SetCurrentHealth(health);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            if (col.gameObject.GetComponent<Mover>())
            {
                float damage = col.gameObject.GetComponent<Mover>().Damage;
                ApplyDamage(damage);

            }
            if (col.gameObject.GetComponent<LaserForward>())
            {
                float damage = col.gameObject.GetComponent<LaserForward>().damage;
                ApplyDamage(damage);
            }
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

    public void ApplyDamage(float damage)
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Hit");
            health -= damage;
        }
    }
}
