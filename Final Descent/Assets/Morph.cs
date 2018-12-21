using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morph : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Morpher(GameObject g, out GameObject slot)
    {
        GameObject newGun = Instantiate(g, transform.position, transform.rotation);
        newGun.transform.parent = transform.parent;
        slot = newGun;
        Destroy(gameObject);
    }
}
