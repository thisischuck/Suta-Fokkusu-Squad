﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelIK : MonoBehaviour
{
    public List<Transform> bones;
    private float[] distances;
    private Vector3 oldPos;
    private Vector3 velocity;
    private EelTest test;

    void Start()
    {
        distances = new float[bones.Count - 1];
        for (int i = 0; i < distances.Length - 1; i++)
        {
            distances[i] = (bones[i + 1].transform.position - bones[i].transform.position).magnitude * 0.1f;
        }

        velocity = Vector3.forward;
        test = this.GetComponent<EelTest>();
    }

    void FixedUpdate()
    {
        for (int i = 1; i < bones.Count; i++)
        {
            Transform previous = bones[i - 1];
            Transform current = bones[i];

            float distance = Vector3.Distance(previous.position, current.position);

            float t = Time.deltaTime * distance / distances[i - 1] * test.Velocity.magnitude;

            if (t > 0.5f)
                t = 0.5f;
            current.position = Vector3.Slerp(current.position, previous.position, t);
            current.rotation = Quaternion.Slerp(current.rotation, previous.rotation, t);
        }
    }
}
