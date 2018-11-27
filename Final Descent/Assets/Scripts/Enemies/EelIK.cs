using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelIK : MonoBehaviour
{
    public List<Transform> bones;
    public float MinDistance;
    private Vector3[] positions;
    private Vector3[] rotations;
    private Vector3 oldPos;
    private Vector3 velocity;
    void Start()
    {
        positions = new Vector3[bones.Count];
        rotations = new Vector3[bones.Count];
        velocity = Vector3.forward;
    }

    void FixedUpdate()
    {
        for (int i = 1; i < bones.Count; i++)
        {
            Transform previous = bones[i - 1];
            Transform current = bones[i];

            float distance = Vector3.Distance(previous.position, current.position);

            float t = Time.deltaTime * distance / MinDistance;

            if (t > 0.5f)
                t = 0.5f;
            current.position = Vector3.Slerp(current.position, previous.position, t);
            current.rotation = Quaternion.Slerp(current.rotation, previous.rotation, t);
        }
    }
}
