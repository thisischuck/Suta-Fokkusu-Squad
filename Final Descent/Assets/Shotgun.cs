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

		if(addForceObj != null)
		{
			foreach(Transform t in addForceObj)
			{
				Rigidbody rb = t.GetComponent<Rigidbody>();

				rb.AddForce(transform.forward * 30f);
				Debug.Log(rb.transform);
			}
		}			
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			addForceObj.Add(other.transform);
			Debug.Log("collision");
		}
	}

	public void ClearList()
	{
		addForceObj.Clear();
	}
}
