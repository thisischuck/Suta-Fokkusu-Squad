using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    private List<Transform> _playerList;
    public float eelSpeed;

    //Tornado
    private bool isTornado = false;
    private Vector3 centerPosition;
    public float radius;
    public float force;

    //EletricExplosion
    private float nextExplosion;
    private bool isExploding = false;
    private int explosionCount;
    public ParticleSystem ee;

    //highspeed
    private List<Transform> RHoles = new List<Transform>();
    private List<Transform> LHoles = new List<Transform>();
    public Transform selectedHole;
    private bool left;
    private bool goingIn = false;
    private bool isHighSpeed = false;
    int nextHole;
    Vector3 direction;

    //calling
    public GameObject enemyBabies;
    private int enemiesPerHole = 3;

    //Bite
    public Transform BottomMouth;
    private bool isBiting = false;
    Quaternion openMouth;
    Quaternion closeMouth;
    public bool mouthClosed;

    // Use this for initialization
    void Start () {

        nextExplosion = 0f;
        explosionCount = 0;
        eelSpeed = 15f;
        radius = 20f;

        _playerList = new List<Transform>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            _playerList.Add(player.gameObject.transform);
        }

        AddHoles();
        int rand = Random.Range(0, 2);

        if ( rand == 1)
        {
            nextHole = Random.Range(0, LHoles.Count - 1);
            selectedHole = LHoles[nextHole];
            left = true;
        }
        else if(rand == 0)
        {
            nextHole = Random.Range(0, RHoles.Count - 1);
            selectedHole = RHoles[nextHole];
            left = false;
        }

        openMouth = Quaternion.Euler(BottomMouth.rotation.y + 20.0f, BottomMouth.rotation.y, BottomMouth.rotation.z);
        closeMouth = BottomMouth.rotation;
        mouthClosed = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            /*isTornado = !isTornado;
            if (isTornado)
            {
                centerPosition = transform.position + Vector3.forward * 10f;
            }
            else transform.position = Vector3.MoveTowards(transform.position, centerPosition, eelSpeed * Time.deltaTime);*/
            isExploding = !isExploding;
            //isHighSpeed = !isHighSpeed;
            //Calling();
            /*isBiting = !isBiting;
            if (openMouth == BottomMouth.rotation)
                mouthClosed = false;
            else if (closeMouth == BottomMouth.rotation)
                mouthClosed = true;*/
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

        if (isTornado)
        {
            CircularMotion();
            GetSucked();
        }

        if (isHighSpeed)
        {
            HighSpeed();
        }

        if (isBiting)
        {
            Bite();
        }
    }

    void CircularMotion() //Still needs adjusting
    {
        float rotSpeed = 80f;

        transform.RotateAround(centerPosition, Vector3.up, Time.deltaTime * rotSpeed);
        Vector3 desiredPosition = (transform.position - centerPosition).normalized * radius + centerPosition;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 5f * Time.deltaTime);
    }

    void GetSucked()
    {
        foreach(Transform player in _playerList)
        {
            Vector3 direction = Vector3.zero;
            if (Vector3.Distance(centerPosition, player.position) < 50f)
            {
                direction = player.position - transform.position;
                player.position = Vector3.MoveTowards(player.position, centerPosition, force);
            }
        }
    }

    void HighSpeed()
    {
        Transform previousHole = selectedHole; 
        if (Vector3.Distance(transform.position, selectedHole.position) < 5f)
        {
            Debug.Log("Point Reached");
            //Look for another hole
            if (left && !goingIn)
            {
                left = true;
                goingIn = true;
                while(selectedHole == previousHole)
                {
                    nextHole = Random.Range(0, LHoles.Count - 1);
                    selectedHole = LHoles[nextHole];
                }
            }
            else if(!left && !goingIn)
            {
                left = false;
                goingIn = true;
                while (selectedHole == previousHole)
                {
                    nextHole = Random.Range(0, RHoles.Count - 1);
                    selectedHole = RHoles[nextHole];
                }
            }
            else if(left && goingIn)
            {
                goingIn = false;
                left = false;
                while (selectedHole == previousHole)
                {
                    nextHole = Random.Range(0, RHoles.Count - 1);
                    selectedHole = RHoles[nextHole];
                }
            }
            else if (!left && goingIn)
            {
                goingIn = false;
                left = true;
                while (selectedHole == previousHole)
                {
                    nextHole = Random.Range(0, LHoles.Count - 1);
                    selectedHole = LHoles[nextHole];
                }
            }
        }

        transform.position += transform.forward *( Time.deltaTime * eelSpeed);
        //transform.position = Vector3.MoveTowards(transform.position, holes[hole].position, Time.deltaTime * eelSpeed);

        direction = transform.position - selectedHole.position;
        Quaternion targetRot = Quaternion.LookRotation(-direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.04f);
        //transform.forward = -direction;
    }

    void Calling() // change position to spawnEnemyPosition
    {
        foreach(Transform hole in RHoles)
        {
            for(int i=0;i<=enemiesPerHole;i++)
                Instantiate(enemyBabies,hole.position,Quaternion.identity);
        }

        foreach (Transform hole in LHoles)
        {
            for (int i = 0; i <= enemiesPerHole; i++)
                Instantiate(enemyBabies, hole.position, Quaternion.identity);
        }
    }

    void Bite()
    {
        if (mouthClosed)
        {
            BottomMouth.localRotation = Quaternion.RotateTowards(BottomMouth.rotation, openMouth , Time.deltaTime * 6f);
        }
        else
            BottomMouth.localRotation = Quaternion.RotateTowards(BottomMouth.rotation, closeMouth, Time.deltaTime * 6f);
    }

    void AddHoles()
    {
        GameObject rholes = GameObject.Find("2Room").transform.Find("Room").transform.Find("RightHoles").gameObject;
        GameObject lholes = GameObject.Find("2Room").transform.Find("Room").transform.Find("LeftHoles").gameObject;

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
