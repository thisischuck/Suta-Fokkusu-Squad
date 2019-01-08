using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_BaseHealth : NetworkBehaviour {
    public float maxHeatlh;
    public float maxShield;

    [SyncVar]
    public float currentHealth;
    [SyncVar]
    public float currentShield;

    [SyncVar]
    public bool isAlive;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
