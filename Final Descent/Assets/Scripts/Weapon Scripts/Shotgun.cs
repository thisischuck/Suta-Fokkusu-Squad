using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour {

	List<Transform> addForceObj;
	// Use this for initialization
	void Start () {
		addForceObj = new List<Transform>();
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			Rigidbody rb = other.GetComponent<Rigidbody>();

			rb.AddForce(transform.forward * 60f);
			Destroy(this.gameObject);
		}
	}
}
