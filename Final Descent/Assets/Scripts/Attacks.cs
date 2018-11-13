using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    List<GameObject> _playerList;

	// Use this for initialization
	void Start () {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            _playerList.Add(player);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach(GameObject player in _playerList)
            {
                GetSucked(player);
            }
        }
	}

    void GetSucked(GameObject player)
    {
        Vector3 direction = Vector3.zero;
        if(Vector3.Distance(transform.position, player.transform.position) < 20f)
        {
            direction = player.transform.position - transform.position;
            player.transform.position = Vector3.MoveTowards(player.transform.position, transform.position, 20);
        }
    }
}
