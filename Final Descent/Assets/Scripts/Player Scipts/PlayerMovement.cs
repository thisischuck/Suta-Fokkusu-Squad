/*
 Script written by Vasco.
 Manages the player's movement, the camera direction (indirectly) and the dash.
 
 12-10-2018
 The script is almost complete. 
 Right now, the movement has 2 different modes. We can swap between them with the variable "mode".
 It also has a speed boost, which is experimental.
 28-11-2018
 The script was cleaned and optimized 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables 
    public GameObject ship;
    public Camera cam;
    private Transform ship;
    private GameObject trail;
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
        rB = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rotate();
        Move();

        trail.SetActive(Input.GetButton("Vertical"));
        CharacterTappingControl(); //Controls the double tapping duration
        CharacterDashControl(); //Controls everything about the dash

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
    }

    private void Update()
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

                ship.GetComponent<Ship>().DashRotation(360, dashDur);

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

                ship.GetComponent<Ship>().DashRotation(-360, dashDur);
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
        sens = PlayerInfo.sens;

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

//#region OBJECT TO FOLLOW
//if (lifePodActive)
//{
//    ship = transform.Find("LifePod");
//    isDashing = false;
//}
//else ship = transform.Find("Aircraft");
//#endregion
