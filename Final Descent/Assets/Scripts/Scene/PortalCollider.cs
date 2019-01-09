using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalCollider : MonoBehaviour {

	private Transform endPortal;
	private Scene scene;
	private GameObject Boss;
	private GameObject Dungeon;
	private GameObject BossRoom;

	public void Start()
	{
		endPortal = GameObject.Find("EndPoint").transform;
		scene = SceneManager.GetActiveScene();

		if (scene.name == "Level 2")
		{
			BossRoom = GameObject.Find("2Room");
			Boss = GameObject.Find("Eel");
			Dungeon = GameObject.Find("Dungeon2");
		}
		else if (scene.name == "Level 1")
		{
			Dungeon = GameObject.Find("Dungeon");
			BossRoom = GameObject.Find("1Room");
			Boss = GameObject.Find("Butterfly");
		}

		BossRoom.SetActive(false);
		Boss.SetActive(false);
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.tag == "Player")
		{
			BossRoom.SetActive(true);
			collision.transform.position = endPortal.position;
			collision.transform.rotation = endPortal.rotation;			Dungeon.SetActive(false);
			Boss.SetActive(true);
		}
	}
}