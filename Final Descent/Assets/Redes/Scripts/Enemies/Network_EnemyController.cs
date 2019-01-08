using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_EnemyController : NetworkBehaviour {
    public List<GameObject> enemiesList;
    [SyncVar]
    public int enemyCount = 0;
    public int enemyMax = 20;
    public List<GameObject> activeEnemies = new List<GameObject>();

    private void Update()
    {
        foreach (GameObject g in activeEnemies)
        {
            if (g == null)
            {
                activeEnemies.Remove(g);
                enemyCount--;
            }
        }
    }

    public void ChooseAnEnemyToSpawn(Vector3 pos)
    {
        if (!isServer)
            return;
        int n = Random.Range(0, enemiesList.Count - 1);
        SpawnEnemy(n, pos);
    }

    public void SpawnEnemy(int n, Vector3 pos)
    {
        if (enemyCount < enemyMax)
        {
            GameObject g = Instantiate(enemiesList[n], pos, Quaternion.identity);
            activeEnemies.Add(g);
            enemyCount++;
            NetworkServer.Spawn(g);
        }
    }
}
