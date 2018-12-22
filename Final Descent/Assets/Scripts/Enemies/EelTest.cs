using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelTest : MonoBehaviour
{
    public Transform target;
    public Vector3 Velocity;
    public float speed;
    // Use this for initialization
    void Start()
    {
        Velocity = Vector3.forward;
        speed = 20.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Velocity = EnemyBehaviours.Pursuit(transform, Velocity, target, 1.0f) * Time.deltaTime * speed;
        transform.forward = Velocity;
        transform.position += Velocity;
    }
}
