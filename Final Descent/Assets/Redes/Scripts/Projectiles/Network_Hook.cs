using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_Hook : NetworkBehaviour {

    public Transform weaponPos;
    private Transform hookObject;

    public float hookTravelSpeed;
    public float playerTravelSpeed;
    public Vector3 Forward;
    public static bool hooked;

    public float maxDistance;
    private float currentDistance;

    private void Start()
    {
        transform.forward = Forward;
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        if (!isServer)
            return;

        if (!hooked)
        {
            //firing the hook
            transform.Translate(Vector3.forward * Time.deltaTime * hookTravelSpeed);
            currentDistance = Vector3.Distance(transform.position, weaponPos.transform.position);

            if (currentDistance >= maxDistance)
                ReturnHook();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, weaponPos.position, playerTravelSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, weaponPos.position) <= 2f)
            {
                hookObject.parent = null;
                NetworkServer.Destroy(this.gameObject);
                hooked = false;
            }
        }
    }

    void ReturnHook()
    {
        NetworkServer.Destroy(this.gameObject);
        hooked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "Enemy")
        {
            hooked = true;
            hookObject = other.transform;
            other.transform.parent = this.transform;
        }
    }
}
