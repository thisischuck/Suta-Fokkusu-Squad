using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCollider : MonoBehaviour {

	public Transform endPortal;

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "CompletePlayerObject")
		{
			other.transform.position = endPortal.position;
			Debug.Log("hit");
		}
	}
}
