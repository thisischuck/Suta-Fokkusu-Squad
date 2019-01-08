using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_Ship : NetworkBehaviour {
    [SyncVar]
    public Transform player;
    public Transform child;
    private GameObject trail;
    public NetworkConnection playerConn;
    public GameObject weaponHolder;
    public GameObject shield;

    [Space]
    [Header("Movement Settings")]
    public float speed;
    public float rotSpeed;

    public override void OnStartAuthority()
    {
        if (hasAuthority)
        {
            gameObject.name = "localShip";
            player = GameObject.Find("localPlayer").transform;
            Network_PlayerMovement pM = player.GetComponent<Network_PlayerMovement>();
            SetColors(pM.color1, pM.color2, pM.color3);
        }
    }

    private void SetColors(Color c1, Color c2, Color c3)
    {
        GameObject shipWithColor = transform.Find("Player_aircraft").Find("Aircraft").gameObject;
        shipWithColor.GetComponent<DynamicTexture>().ColorShip1 = c1;
        shipWithColor.GetComponent<DynamicTexture>().ColorShip2 = c2;
        shipWithColor.GetComponent<DynamicTexture>().ColorShip3 = c3;
    }

    public void AssignWeaponsAndShiled(GameObject wH, GameObject sH)
    {
        weaponHolder = wH;
        shield = sH;
        player.GetComponent<Network_PlayerMovement>().RpcGiveParent(weaponHolder, transform.gameObject);
        player.GetComponent<Network_PlayerMovement>().RpcGiveParent(shield, transform.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        Rotate();
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
        //Ray crosshair = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 newPos = player.position + player.forward * 1000;
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