using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour {

    float speed;

	// Use this for initialization
	void Start () {
        speed = GetComponent<Mover>().speed;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += speed * transform.forward;
    }
}
