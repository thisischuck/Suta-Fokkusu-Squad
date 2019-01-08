using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : BaseStats {
    public string name;
    public SkinnedMeshRenderer mesh;
    Color originalColor;
    private void Start()
    {
        GenerateVariables(100, 0);
        base_maxHealth = 100;
        //GetComponent<HBController>().StartHPBar(health);
        //mat = GetComponentInChildren<Renderer>().material;
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        originalColor = mesh.material.color;
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
            FlashOnHit();
            GameObject stats = GameObject.Find("Stats");

            var healthEnemy = GetComponent<HealthEnemy>() as HealthEnemy;

            float enemyCurrenhp = healthEnemy.health;
            float enemyMaxhp = healthEnemy.base_maxHealth;
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
