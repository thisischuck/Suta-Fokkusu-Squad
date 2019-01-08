using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_LaserForward : NetworkBehaviour {
    public float secondsToDeath = 5.0f;
    [SyncVar]
    public Vector3 Velocity;
    [SyncVar]
    public Vector3 scale = new Vector3(1,1,1);
    public float Speed = 30.0f;
    private bool hasStarted;
    public int damage = 5;

    void Start()
    {
        transform.forward = Velocity;
        transform.localScale = scale;
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

        NetworkServer.Destroy(this.gameObject);
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