using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    List<Transform> _playerList;
    public Transform target;
    public float force;
    public ParticleSystem ee;

    private float nextExplosion;
    private bool isExploding = false;
    private int explosionCount;

    // Use this for initialization
    void Start () {

        nextExplosion = 0f;
        explosionCount = 0;

        /*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            _playerList.Add(player.gameObject.transform);
        }*/
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            /*foreach(Transform player in _playerList)
            {
                GetSucked(player);
            }*/

            //GetSucked(target);
            isExploding = !isExploding;
        }

        if (isExploding && explosionCount < 3)
        {
            if (Time.time >= nextExplosion)
            {
                ee.Play(true);
                nextExplosion = Time.time + 2f;
                explosionCount++;
            }
        }
        else
        {
            isExploding = false;
            explosionCount = 0;
        }
    }

    void GetSucked(Transform player)
    {
        Vector3 direction = Vector3.zero;
        if(Vector3.Distance(transform.position, player.position) < 20f)
        {
            direction = player.position - transform.position;
            player.position = Vector3.MoveTowards(player.position, transform.position, force);
        }
    }
}
