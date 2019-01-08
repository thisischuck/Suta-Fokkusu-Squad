using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
	public GameObject player;
	static float alpha;
	public Material shield;
	public float upTime;
	public float fadeSpeed;
	bool up = false;
	public bool fadeIn = false;
	public bool fadeOut = false;
	public bool activateShield = false;

	void Start()
	{
		alpha = 1f;
		shield.SetFloat("Vector1_26FA8A98", alpha);
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
			fadeIn = true;
		if (fadeIn)
			Fade(true);
		if (fadeOut)
			Fade(false);

		if (up && upTime < 2f)
		{
			upTime += Time.deltaTime;
		}
		else if(up && upTime >= 2f)
		{
			up = false;
			fadeOut = true;
		}

		if (alpha <= -1)
		{
			up = true;
			fadeIn = false;
		}
		if( alpha >= 1 && fadeOut)
		{
			up = false;
			fadeOut = false;
			upTime = 0f;
		}

		if (player.GetComponent<HealthPlayer>().shield <= 0)
			transform.GetComponent<MeshRenderer>().enabled = false;
		else
			transform.GetComponent<MeshRenderer>().enabled = true;

	}

	void Fade(bool _in)
	{
		if (_in)
			alpha -= fadeSpeed * Time.deltaTime;
		else
			alpha += fadeSpeed * Time.deltaTime;
		shield.SetFloat("Vector1_26FA8A98", alpha);
	}
}
