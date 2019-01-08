using UnityEngine;
using UnityEngine.Networking;


public class Network_PlayerMovement : NetworkBehaviour
{
    #region Variables 
    public GameObject ship;
    public GameObject weaponHolder;
    public GameObject shield;
    public GameObject slot1, slot2, slot3;
    public Color color1, color2, color3;
    public string name1, name2, name3;
    private Rigidbody rB;

    //----------------Key Tap----------------------
    private int keyTapCount_right = 0;
    private float keyTapCool_right = 0f;

    private int keyTapCount_left = 0;
    private float keyTapCool_left = 0f;

    [Header("Dash Settings")]
    [Tooltip("How fast does the player need to double tap the keys to dash.")]
    public float keyTapTimeFrame = 0.5f;

    //----------------Dash Values----------------------
    private bool isDashing = false;
    private Vector3 dir = Vector3.zero;
    private float dashDurCount = 0;
    [Tooltip("Dash Duration.")]
    public float dashDur = 1;
    [Tooltip("Dash speed.")]
    public float dashSpeed = 40;
    public int numberOfRotations = 1;

    //----------------Dash Cooldown----------------------
    private bool hasDashed = false;
    [Tooltip("Dash cooldown.")]
    public float dashCooldown;
    private float dashCDRCount;

    private float hor, ver;

    private float rotX, rotY;

    //----------------Movement settings----------------------
    [Space]
    [Header("Movement Settings")]
    [Tooltip("Movement speed")]
    public float speed = 20;
    [Tooltip("Mouse sensibility")]
    public float sens = 50; //por a cena das opções
    public float rotSpeed = 5;

    // LIFE POD
    public bool lifePodActive = false;
    #endregion

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            gameObject.name = "localPlayer";
            Camera cam = transform.GetChild(0).GetComponent<Camera>();
            cam.transform.gameObject.SetActive(true);
            rB = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            //ship = GameObject.Find("localShip");
            CmdInstantiateShip();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Rotate();
            Move();

            CharacterTappingControl(); //Controls the double tapping duration
            CharacterDashControl(); //Controls everything about the dash
        }
    }

    public void SetStats(Color c1, Color c2, Color c3, GameObject w1, GameObject w2, GameObject w3, string name1, string name2, string name3)
    {
        color1 = c1;
        color2 = c2;
        color3 = c3;
        if (w1 != null)
            slot1 = w1;
        if (w2 != null)
            slot2 = w2;
        if (w3 != null)
            slot3 = w3;
        this.name1 = name1;
        this.name2 = name2;
        this.name3 = name3;
    }

    [ClientRpc]
    public void RpcGiveParent(GameObject original, GameObject pappy)
    {
        if (original != null && pappy != null)
        {
            original.transform.parent = pappy.transform;
            original.transform.localPosition = Vector3.zero;
            Debug.Log(original.ToString() + " to " + pappy.ToString());
        }
    }

    [ClientRpc]
    public void RpcGearUpWeapon(GameObject weapon, GameObject l, GameObject r)
    {
        weapon.GetComponent<Network_PsGunManager>().GearUpWeapon(l, r);
    }

    [Command]
    void CmdInstantiateShip()
    {
        GameObject s = Instantiate(ship);
        s.GetComponent<Network_Ship>().player = transform;
        #region Instantiate
        GameObject wH = Instantiate(weaponHolder, ship.GetComponent<Network_Ship>().child.position, ship.GetComponent<Network_Ship>().child.rotation);
        GameObject sd = Instantiate(shield, ship.GetComponent<Network_Ship>().child.position, ship.GetComponent<Network_Ship>().child.rotation);
        slot1 = Instantiate(slot1);
        slot2 = Instantiate(slot2);
        slot3 = Instantiate(slot3);

        //GameObject l_slot1 = slot1.GetComponent<Network_PsGunManager>().leftClickObject;
        //GameObject r_slot1 = slot1.GetComponent<Network_PsGunManager>().rightClickObject;

        //GameObject l_slot2 = slot2.GetComponent<Network_PsGunManager>().leftClickObject;
        //GameObject r_slot2 = slot2.GetComponent<Network_PsGunManager>().rightClickObject;
        #endregion

        #region Spawn with authority
        NetworkServer.SpawnWithClientAuthority(s, connectionToClient);
        NetworkServer.SpawnWithClientAuthority(wH, connectionToClient);
        NetworkServer.SpawnWithClientAuthority(sd, connectionToClient);

        NetworkServer.SpawnWithClientAuthority(slot1, connectionToClient);
        NetworkServer.SpawnWithClientAuthority(slot2, connectionToClient);
        NetworkServer.SpawnWithClientAuthority(slot3, connectionToClient);

        //NetworkServer.SpawnWithClientAuthority(l_slot1, connectionToClient);
        //NetworkServer.SpawnWithClientAuthority(r_slot1, connectionToClient);
        //NetworkServer.SpawnWithClientAuthority(l_slot1, connectionToClient);
        //NetworkServer.SpawnWithClientAuthority(r_slot1, connectionToClient);
        //GameObject l_slot3 = slot3.GetComponent<Network_PsGunManager>().leftClickObject;
        //GameObject r_slot3 = slot3.GetComponent<Network_PsGunManager>().rightClickObject;
        //l_slot3 = Instantiate(l_slot3, slot3.transform.position, slot3.transform.rotation);
        //r_slot3 = Instantiate(r_slot3, slot3.transform.position, slot3.transform.rotation);

        //NetworkServer.SpawnWithClientAuthority(l_slot3, connectionToClient);
        //NetworkServer.SpawnWithClientAuthority(r_slot3, connectionToClient);
        #endregion

        sd.GetComponent<Network_ShieldController>().player = s;
        RpcGiveParent(wH, s);
        RpcGiveParent(sd, s);

        wH.GetComponent<Network_PsWeaponSwitching>().slot1 = slot1;
        wH.GetComponent<Network_PsWeaponSwitching>().slot2 = slot2;
        wH.GetComponent<Network_PsWeaponSwitching>().slot3 = slot3;
        wH.GetComponent<Network_PsWeaponSwitching>().Weapons.Add(slot1.GetComponent<MeshRenderer>());
        wH.GetComponent<Network_PsWeaponSwitching>().Weapons.Add(slot2.GetComponent<MeshRenderer>());
        wH.GetComponent<Network_PsWeaponSwitching>().Weapons.Add(slot3.GetComponent<MeshRenderer>());
        slot1.GetComponent<Network_PsGunManager>().player = transform.gameObject;
        slot2.GetComponent<Network_PsGunManager>().player = transform.gameObject;
        slot3.GetComponent<Network_PsGunManager>().player = transform.gameObject;
        RpcGiveParent(slot1, wH);
        RpcGiveParent(slot2, wH);
        RpcGiveParent(slot3, wH);

        //RpcGearUpWeapon(slot1, l_slot1, r_slot1);
        //RpcGearUpWeapon(slot2, l_slot2, r_slot2);
        //RpcGearUpWeapon(slot3, l_slot3, r_slot3);
        //RpcGiveParent(l_slot3, slot3);
        //RpcGiveParent(r_slot3, slot3);

        //wH.GetComponent<Network_PsWeaponSwitching>().Order66(transform);
    }

    [Command]
    public void CmdGiveAuthorityToObjects(GameObject g, GameObject pappy)
    {
        NetworkServer.SpawnWithClientAuthority(g, connectionToClient);
    }

    [Command]
    public void CmdHelpingWeapon(GameObject g)
    {
        GameObject l_slot = g.GetComponent<Network_PsGunManager>().leftClickObject;
        GameObject r_slot = g.GetComponent<Network_PsGunManager>().rightClickObject;
        l_slot = Instantiate(l_slot, g.transform.position, g.transform.rotation);
        r_slot = Instantiate(r_slot, g.transform.position, g.transform.rotation);

        NetworkServer.SpawnWithClientAuthority(l_slot, connectionToClient);
        NetworkServer.SpawnWithClientAuthority(r_slot, connectionToClient);

        RpcGearUpWeapon(g, l_slot, r_slot);
        RpcGiveParent(l_slot, g);
        RpcGiveParent(r_slot, g);
    }

    private void Rotate()
    {
        rotX += -Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -88, 88);
        Vector3 dir = new Vector3(rotX, rotY, 0.0f);

        Quaternion rot = Quaternion.Euler(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
    }

    void Move()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(hor, 0.0f, ver);
        move = transform.TransformDirection(move);
        rB.velocity = move * speed;

        if (hor != 0 || ver != 0)
        {
            SendMessage("PlayShipMovementAudio");
        }
        else
        {
            SendMessage("StopShipMovementAudio");
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            #region DASH INPUT
            if (Input.GetKeyDown(KeyCode.A) && !isDashing && !hasDashed) //Left dash
            {
                if (keyTapCool_left > 0 && keyTapCount_left >= keyTapTimeFrame)
                {
                    isDashing = true;
                    hasDashed = true;

                    dir = -transform.right;
                    dashDurCount = 0;
                    dashCDRCount = 0;

                    ship.GetComponent<Network_Ship>().DashRotation(360 * numberOfRotations, dashDur);

                }
                else
                {
                    keyTapCool_left += 0.5f;
                    keyTapCount_left += 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.D) && !isDashing && !hasDashed) //Right Dash
            {
                if (keyTapCool_right > 0 && keyTapCount_right >= keyTapTimeFrame)
                {
                    isDashing = true;
                    hasDashed = true;

                    dir = transform.right;
                    dashDurCount = 0;
                    dashCDRCount = 0;

                    ship.GetComponent<Network_Ship>().DashRotation(-360 * numberOfRotations, dashDur);
                }
                else
                {
                    keyTapCool_right += 0.5f;
                    keyTapCount_right += 1;
                }
            }
            #endregion

            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 5))
            {
                if (hit.transform.tag == "Water")
                {
                    hit.transform.GetComponent<WaterGenerator>().Wave(hit.point);
                }
            }

            //Mouse sensibility
            sens = PlayerStatsInfo.sens;
        }
    }

    //Key timer and stuff
    private void CharacterTappingControl()
    {
        //Right Tap
        if (keyTapCool_right > 0)
        {
            keyTapCool_right -= 1 * Time.deltaTime;
        }
        else
        {
            keyTapCount_right = 0;
        }

        if (keyTapCount_right > 2)
        {
            keyTapCount_right = 0;
        }

        //Left Tap
        if (keyTapCool_left > 0)
        {
            keyTapCool_left -= 1 * Time.deltaTime;
        }
        else
        {
            keyTapCount_left = 0;
        }

        if (keyTapCount_left > 2)
        {
            keyTapCount_left = 0;
        }
    }

    //Dash timers and stuff
    private void CharacterDashControl()
    {
        if (hasDashed)
        {
            if (isDashing)
            {
                dashDurCount += 1 * Time.deltaTime;
            }
            else
            {
                dashCDRCount += 1 * Time.deltaTime;
            }

            rB.AddForce(dir * dashSpeed, ForceMode.Impulse);
        }

        if (dashDurCount >= dashDur && isDashing) //End of the dash
        {
            dashDurCount = 0;
            isDashing = false;
            dir = Vector3.zero;
        }

        if (dashCDRCount >= dashCooldown && hasDashed) //End of the cooldown
        {
            dashCDRCount = 0;
            hasDashed = false;
        }
    }
}