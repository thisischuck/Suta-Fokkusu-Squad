using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
	float forwardAxis;
	float sideAxis;
	// Use this for initialization
	void Start () 
	{
		Cursor.lockState = CursorLockMode.Locked;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		forwardAxis = Input.GetAxis("Vertical");
		sideAxis = Input.GetAxis("Horizontal");

		transform.position += transform.forward * forwardAxis * Time.deltaTime * 20.0f;
		transform.position += transform.right * sideAxis * Time.deltaTime * 20.0f;

		transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * 50.0f);
	}
}
