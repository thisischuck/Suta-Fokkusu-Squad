using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	public Transform player;
	private float bias;
	private Vector3 wantedPos;

	void Start () 
	{
		bias = 0.90f;
		transform.position = player.transform.position - player.transform.forward * 3.0f + Vector3.up * 2.0f;
	}
	
	void Update () 
	{
		wantedPos = player.transform.position - player.transform.forward * 5.0f + Vector3.up * 2.0f;
		transform.position = transform.position * bias + wantedPos * (1.0f - bias);
		transform.LookAt(player.transform.position + player.transform.forward * 10.0f);
	}
}
