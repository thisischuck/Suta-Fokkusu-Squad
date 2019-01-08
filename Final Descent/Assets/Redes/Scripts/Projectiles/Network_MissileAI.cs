using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_MissileAI : NetworkBehaviour {

    private Transform target;

    public GameObject miniMissiles;

    public float speed = 0, rotSpeed = 0;
    [SyncVar]
    public Vector3 Velocity;
    public float lifeTime;
    public bool explosive = false;

    private float timer = 0.0f;

    void Start()
    {

        if (!explosive)
            lifeTime = 5f;
        else lifeTime = 4f;

        transform.forward = Velocity;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (target != null)
        {
            if (LookForEnemy())
            {
                Velocity = EnemyBehaviours.Pursuit(transform, Velocity, target, 1);
                transform.forward = Velocity.normalized;
            }
        }
        transform.position += Velocity * speed * Time.deltaTime;

        if (timer >= lifeTime && !explosive)
            NetworkServer.Destroy(this.gameObject);
        else if (timer >= lifeTime && explosive)
        {
            SpawnMiniMissiles(10);
            NetworkServer.Destroy(this.gameObject);
        }
    }

    private void SpawnMiniMissiles(int count)
    {
        for (int i = 0; i <= count; i++)
        {
            Vector3 spherePoint = Random.insideUnitSphere * 3 + this.transform.position;
            CmdSpawnMiniMissile(spherePoint);
        }
    }

    [Command]
    private void CmdSpawnMiniMissile(Vector3 spherePoint)
    {
        GameObject minimissile = Instantiate(miniMissiles, spherePoint, this.transform.rotation);
        NetworkServer.Spawn(minimissile);
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

        if (other.GetComponent<Network_EnemyHealth>())
        {
            if (isServer)
            {
                other.GetComponent<Network_EnemyHealth>().TakeDamage(15);
                NetworkServer.Destroy(this.gameObject);
            }
            if (isLocalPlayer)
            {
                GameObject stats = GameObject.Find("Stats");

                float enemyCurrenhp = other.GetComponent<Network_EnemyHealth>().currentHealth;
                float enemyMaxhp = other.GetComponent<Network_EnemyHealth>().maxHeatlh;
                string enemyName = "";
                if (other.GetComponent<Network_Enemy>())
                {
                    enemyName = other.GetComponent<Network_Enemy>().enemyName;
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
}
