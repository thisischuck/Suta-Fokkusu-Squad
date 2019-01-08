using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_Shotgun : NetworkBehaviour {

    List<Transform> addForceObj;
    // Use this for initialization
    void Start()
    {
        addForceObj = new List<Transform>();
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {

        if (addForceObj != null)
        {
            foreach (Transform t in addForceObj)
            {
                Rigidbody rb = t.GetComponent<Rigidbody>();

                rb.AddForce(transform.forward * 60f);
                Debug.Log(rb.transform);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.tag == "Enemy")
        {
            addForceObj.Add(other.transform);
            Debug.Log("collision");
        }
    }

    public void ClearList()
    {
        addForceObj.Clear();
    }
}
