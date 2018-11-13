/*
 Script written by Vasco.
 Manages the player's movement, the camera direction (indirectly) and the dash.
 
 12-10-2018
 The script is almost complete. 
 Right now, the movement has 2 different modes. We can swap between them with the variable "mode".
 It also has a speed boost, which is experimental.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    private Transform ship;
    private GameObject trail;
    private Rigidbody rB;
    public bool lifePodActive = false;

    //General movement variables
    private float forwardAxis;
    private float sideAxis;
    public float moveStep = 1.5f;
    public float shipSpeed = 20;

    #region Dash
    //Key Tap
    public int keyTapCount_right = 0;
    public float keyTapCool_right = 0f;

    public int keyTapCount_left = 0;
    public float keyTapCool_left = 0f;

    public float keyTapTimeFrame = 0.5f;

    //Dash Values
    public bool isDashing = false;
    public Vector3 dir = Vector3.zero;
    private float dashDurCount = 0;
    public float dashDur = 1;
    public float dashSpeed = 40;

    //Dash Cooldown
    public bool hasDashed = false;
    public float dashCooldown;
    public float dashCDRCount;
    #endregion

    //Mouse sensitivity and camera rotations
    [Range(1.0f, 150.0f)]
    public float sensitivity = 4f;
    private float rotX, rotY;

    //Movement mode: debug only
    public int mode;

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ship = transform.Find("Aircraft"); //Detects the ship
        trail = transform.Find("Aircraft").Find("Trail").gameObject;
        trail.SetActive(false);
        rB = GetComponent<Rigidbody>();
    }

    // Controls only the actual movement.
    void FixedUpdate()
    {
        //rB.AddForce(cam.transform.forward * forwardAxis * Time.deltaTime * shipSpeed);
        Vector3 movement = new Vector3(sideAxis, 0, forwardAxis);
        movement = transform.TransformDirection(movement);
        rB.velocity = movement * shipSpeed;

        trail.SetActive(Input.GetButton("Vertical"));


        if (!isDashing)
        {
            switch (mode) //Switches the rotation method
            {
                case 0:
                    rotX = Mathf.Clamp(rotX, -90, 90);
                    transform.eulerAngles = new Vector3(rotX, rotY, 0);
                    break;
                case 1:
                    transform.Rotate(new Vector3(rotX, rotY, 0), Space.Self);
                    break;
            }
        }
    }

    void Update()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, -transform.up * 1, Color.yellow);
        if (Physics.Raycast(transform.position, -transform.up, out hit, 5))
        {
            if (hit.transform.tag == "Water")
            {
                Debug.Log(hit.distance);
                hit.distance = Mathf.Clamp(hit.distance, 2, 5);
                hit.transform.GetComponent<WaterGenerator>().FindPoint(hit.point, 2 / hit.distance + 0.25f);
            }
        }

        #region OBJECT TO FOLLOW
        if (lifePodActive)
        {
            ship = transform.Find("LifePod");
            isDashing = false;
        }
        else ship = transform.Find("Aircraft");
        #endregion

        #region MOVEMENT INPUT
        forwardAxis = Input.GetAxis("Vertical");
        sideAxis = Input.GetAxis("Horizontal");
        #endregion

        #region ROTATION INPUT
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.forward, moveStep);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.forward, -moveStep);
        }
        #endregion

        #region SPEED BOOST

        if (Input.GetKey(KeyCode.Space))
        {
            //shipSpeed = 140f;
            //ship.GetComponent<Ship>().DashRotation(360, 1);
        }
        else
        {
            shipSpeed = 20f;
        }

        #endregion

        if (!lifePodActive)
        {
            #region DASH INPUT
            if (Input.GetKeyDown(KeyCode.A) && !isDashing && !hasDashed)
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

            if (Input.GetKeyDown(KeyCode.D) && !isDashing && !hasDashed)
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

            CharacterTappingControl(); //Controls the double tapping duration
            CharacterDashControl(); //Controls everything about the dash
        }

        #region CAMERA ROTATION
        switch (mode)
        {
            case 0:
                rotX += -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
                rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;// * Mathf.Sign(transform.up.y);
                break;

            case 1:
                rotX = -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
                rotY = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;// * Mathf.Sign(transform.up.y);
                break;
        }
        #endregion
    }


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

    private void CharacterDashControl()
    {
        if (hasDashed)
        {
            if (isDashing)
                dashDurCount += 1 * Time.deltaTime;
            else
                dashCDRCount += 1 * Time.deltaTime;

            transform.position += dir * dashSpeed * Time.deltaTime;
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

