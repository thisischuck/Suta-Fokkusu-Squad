using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawningController : MonoBehaviour {
    public List<GameObject> enemiesList;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject ChooseAnEnemy()
    {
        int n = Random.Range(0, enemiesList.Count);
        return enemiesList[n];
    }
}
