using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    float angularSpeed;

    void Start()
    {
        angularSpeed = 10.0f;
    }

    void Update()
    {
        transform.parent.transform.position += transform.parent.transform.forward * 0.2f;
        transform.RotateAround(transform.parent.transform.position, transform.parent.transform.forward, angularSpeed);
    }
}
