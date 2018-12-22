/*
Manages the actual ship. With this script we can execute rotations that only affect the ship.
 
 12-10-2018
 The script executes correctly the dash rotation.
 28-12-2018
 The script was optimized 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public Transform player;
    public Transform child;
    private GameObject trail;

    [Space]
    [Header("Movement Settings")]
    public float speed;
    public float rotSpeed;

    public void Start()
    {
        //trail = transform.Find("Trail").gameObject;
        //trail.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        Rotate();
        //trail.SetActive(Input.GetButton("Vertical"));
    }

    void Move()
    {
        if (Vector3.Distance(transform.position, player.position) > 0.005f)
        {
            Vector3 dir = player.position - transform.position;
            float step = speed * Time.deltaTime;

            Debug.DrawRay(transform.position, dir * 5, Color.red);
            transform.position += dir * step;
        }
    }

    void Rotate()
    {
        Vector3 newPos = player.position + player.forward * 10;
        Vector3 newDir = newPos - transform.position;

        Quaternion rot = Quaternion.LookRotation(newDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
    }

    //Starts the dash rotation
    public void DashRotation(float angleZ, float duration)
    {
        child.GetComponent<ShipRotation>().DashRotation(angleZ, duration);
    }
}