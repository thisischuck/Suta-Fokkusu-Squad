using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{

	static float alpha;
	public Material shield;
	public float upTime;
	public float fadeSpeed;
	bool up = false;
	bool fadeIn = false;
	bool fadeout = false;

	void Start()
	{
		alpha = 1f;
		shield.SetFloat("Vector1_26FA8A98", alpha);
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.P))
		{
			fadeIn = true;
		}

		if (fadeIn)
			Fade(true);
		if (fadeout)
			Fade(false);

		if (up && upTime < 2f)
		{
			upTime += Time.deltaTime;
		}
		else if(up && upTime >= 2f)
		{
			up = false;
			fadeout = true;
		}

		if (alpha <= -1)
		{
			up = true;
			fadeIn = false;
		}
		if( alpha >= 1 && fadeout)
		{
			up = false;
			fadeout = false;
			upTime = 0f;
		}
	}

	void Fade(bool _in)
	{
		if (_in)
			alpha -= fadeSpeed * Time.deltaTime;
		else
			alpha += fadeSpeed * Time.deltaTime;
		shield.SetFloat("Vector1_26FA8A98", alpha);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			fadeIn = true;
			fadeout = false;
		}
	}
}
