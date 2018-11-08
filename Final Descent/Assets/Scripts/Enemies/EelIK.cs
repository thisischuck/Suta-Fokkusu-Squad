using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelIK : MonoBehaviour
{
    public List<Transform> bones;
    private Vector3[] positions;
    private Vector3[] rotations;
    private Vector3 velocity;
    private Vector3 oldPos;
    void Start()
    {
        positions = new Vector3[bones.Count];
        rotations = new Vector3[bones.Count];
        velocity = Vector3.forward;
    }

    void FixedUpdate()
    {
        if (oldPos != transform.position)
        {
            foreach (Transform b in bones)
            {
                if (bones.IndexOf(b) == 0)
                {
                    b.position = Vector3.SmoothDamp(b.position, transform.position, ref velocity, 1.0f, 0.5f);
                    continue;
                }
                b.position = Vector3.Lerp(b.position, positions[bones.IndexOf(b) - 1], 0.5f);
            }
        }


        oldPos = transform.position;
    }
}
