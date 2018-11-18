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

    public List<Transform> RHoles = new List<Transform>();
    public List<Transform> LHoles = new List<Transform>();
    public Transform selectedHole;
    public bool left;
    public bool goingIn;

    List<Transform> holes = new List<Transform>();
    private bool isHighSpeed = false;
    int nextHole;
    Vector3 direction;

    // Use this for initialization
    void Start () {

        nextExplosion = 0f;
        explosionCount = 0;
        eelSpeed = 15f;

        //Look for the holes and Add'em to the lsit
        /*GameObject holeSystem = GameObject.Find("Holes");

        foreach (Transform child in holeSystem.transform)
        {
            holes.Add(child);
        }
        nextHole = Random.Range(0, holes.Count - 1);*/

        /*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            _playerList.Add(player.gameObject.transform);
        }*/

        AddHoles();
        if(Random.Range(0,2) == 1)
        {
            nextHole = Random.Range(0, LHoles.Count - 1);
            selectedHole = LHoles[nextHole];
            left = true;
        }
        else
        {
            nextHole = Random.Range(0, RHoles.Count - 1);
            selectedHole = RHoles[nextHole];
            left = false;
        }
        goingIn = false;
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
            HighSpeed(selectedHole);
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

    void HighSpeed(Transform hole)
    {
        if (Vector3.Distance(transform.position, hole.position) < 5f)
        {
            Debug.Log("Point Reached");
            //Look for another hole
            if (left && !goingIn)
            {
                nextHole = Random.Range(0, LHoles.Count-1);
                goingIn = true;
            }
            if(!left && !goingIn)
            {
                nextHole = Random.Range(0, RHoles.Count-1);
                goingIn = true;
            }
            if(left && goingIn)
            {
                nextHole = Random.Range(0, RHoles.Count-1);
                goingIn = false;
            }
            if (!left && goingIn)
            {
                nextHole = Random.Range(0, LHoles.Count-1);
                goingIn = false;
            }
        }

        transform.position += transform.forward *( Time.deltaTime * eelSpeed);
        //transform.position = Vector3.MoveTowards(transform.position, holes[hole].position, Time.deltaTime * eelSpeed);

        direction = transform.position - hole.position;
        Quaternion targetRot = Quaternion.LookRotation(-direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.04f);
        //transform.forward = -direction;
    }

    void AddHoles()
    {
        GameObject rholes = GameObject.Find("Holes").transform.Find("RightHoles").gameObject;
        GameObject lholes = GameObject.Find("Holes").transform.Find("LeftHoles").gameObject;

        foreach (Transform child in rholes.transform)
        {
            RHoles.Add(child);
        }
        foreach (Transform child in lholes.transform)
        {
            LHoles.Add(child);
        }
    }
}
