using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class w : MonoBehaviour {
    public Material mat;

    public float dampingSpeed = 0.02f, streachDistance = 2;
    float streach;
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        streach = Mathf.Clamp(streach - dampingSpeed, 0, 20);
        mat.SetFloat("_streach", streach);
	}

    public void Wave(Vector3 point)
    {
        mat.SetVector("_point_of_bend", point);
        streach = streachDistance;

    }
}
