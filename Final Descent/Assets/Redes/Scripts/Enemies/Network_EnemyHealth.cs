using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Network_EnemyHealth : Network_BaseHealth
{

    public string name;
    public SkinnedMeshRenderer mesh;
    Color originalColor;

    // Use this for initialization
    void Start()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        originalColor = mesh.material.color;
        currentHealth = maxHeatlh;
        currentShield = maxShield;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        if (isServer)
        {
            if (currentHealth <= 0)
            {
                NetworkServer.Destroy(gameObject);
            }
            else
            {
                Debug.Log("Hit");
                currentHealth -= damage;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            if (col.gameObject.GetComponent<Mover>())
            {
                float damage = col.gameObject.GetComponent<Mover>().Damage;
                TakeDamage(damage);

            }
            if (col.gameObject.GetComponent<Network_LaserForward>())
            {
                float damage = col.gameObject.GetComponent<Network_LaserForward>().damage;
                TakeDamage(damage);
            }
            if (!isClient)
                return; 

            FlashOnHit();
            GameObject stats = GameObject.Find("Stats");

            var healthEnemy = GetComponent<Network_EnemyHealth>() as Network_EnemyHealth;

            float enemyCurrenhp = healthEnemy.currentHealth;
            float enemyMaxhp = healthEnemy.maxHeatlh;
            string enemyName = "";

            if (GetComponent<Network_Enemy>())
            {
                enemyName = GetComponent<Network_Enemy>().enemyName;
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
}
