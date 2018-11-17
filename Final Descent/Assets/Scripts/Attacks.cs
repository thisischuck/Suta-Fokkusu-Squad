using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    List<Transform> _playerList;
    public Transform target;
    public float force;
    public float eelSpeed;
    public ParticleSystem ee;

    private float nextExplosion;
    private bool isExploding = false;
    private int explosionCount;

    List<Transform> holes = new List<Transform>();
    private bool isHighSpeed = false;
    private bool isOnHole = false;
    int nextHole;
    Vector3 direction;

    // Use this for initialization
    void Start () {

        nextExplosion = 0f;
        explosionCount = 0;
        eelSpeed = 15f;

        //Look for the holes and Add'em to the lsit
        GameObject holeSystem = GameObject.Find("Holes");

        foreach (Transform child in holeSystem.transform)
        {
            holes.Add(child);
        }
        nextHole = Random.Range(0, holes.Count - 1);

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
            //isExploding = !isExploding;
            isHighSpeed = !isHighSpeed;
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

        if (isHighSpeed)
        {
            HighSpeed(nextHole);
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

    void HighSpeed(int hole)
    {
        if (Vector3.Distance(transform.position, holes[hole].position) < 1f)
        {
            //Look for another hole
            nextHole = Random.Range(0, holes.Count);
        }

        direction = transform.position - holes[hole].position;
        Quaternion targetRot = Quaternion.LookRotation(-direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 8f);
        //transform.forward = -direction;
        transform.position = Vector3.MoveTowards(transform.position, holes[hole].position, Time.deltaTime * eelSpeed);
        
    }
}
