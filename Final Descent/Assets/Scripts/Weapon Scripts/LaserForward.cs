using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserForward : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 10.0f;
    }
}
