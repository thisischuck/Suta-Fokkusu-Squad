using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalCollider : MonoBehaviour {

	private Transform endPortal;
	private Scene scene;
	private GameObject Boss;
	private GameObject Dungeon;

	public void Start()
	{
		endPortal = GameObject.Find("EndPoint").transform;
		scene = SceneManager.GetActiveScene();

		if(scene.name == "Level 2")
		{
			Dungeon = GameObject.Find("2Room");
			Boss = GameObject.Find("Eel");
		}
		else if(scene.name == "Level 1")
		{
			Dungeon = GameObject.Find("1Room");
			Boss = GameObject.Find("Butterfly");
		}

		Dungeon.SetActive(false);
		Boss.SetActive(false);
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.tag == "Player")
		{
			Dungeon.SetActive(true);
			collision.transform.position = endPortal.position;
			collision.transform.rotation = endPortal.rotation;
			Boss.SetActive(true);
		}
	}
}
