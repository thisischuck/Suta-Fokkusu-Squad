using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_BulletBehaviour : NetworkBehaviour {

    float speed;

    // Use this for initialization
    void Start()
    {
        speed = GetComponent<Mover>().speed;
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        transform.position += speed * transform.forward;
    }
}

