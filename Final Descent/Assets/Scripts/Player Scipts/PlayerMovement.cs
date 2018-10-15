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

    //General movement variables
    private float forwardAxis;
    private float sideAxis;
    public float moveStep = 1.5f;
    public float shipSpeed = 20;

    #region Dash
    //Key Tap
    public int keyTapCount = 0;
    public float keyTapCool = 0f;

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
        ship = transform.GetChild(0); //Detects the ship
        mode = 1;
    }

    // Controls only the actual movement.
    void FixedUpdate()
    {
        transform.position += cam.transform.forward * forwardAxis * Time.deltaTime * shipSpeed;
        transform.position += cam.transform.right * sideAxis * Time.deltaTime * shipSpeed;

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

    void Update()
    {
        #region MOVEMENT INPUT
        forwardAxis = Input.GetAxis("Vertical");
        sideAxis = Input.GetAxis("Horizontal");
        #endregion

        #region DASH INPUT
        if (Input.GetKeyDown(KeyCode.A) && !isDashing && !hasDashed)
        {
            if (keyTapCool > 0 && keyTapCount >= 1)
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
                keyTapCool += 0.5f;
                keyTapCount += 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.D) && !isDashing && !hasDashed)
        {
            if (keyTapCool > 0 && keyTapCount == 1)
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
                keyTapCool += 0.5f;
                keyTapCount += 1;
            }
        }
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

        CharacterTappingControl(); //Controls the double tapping duration
        CharacterDashControl(); //Controls everything about the dash

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
        if (keyTapCool > 0)
        {
            keyTapCool -= 1 * Time.deltaTime;
        }
        else
        {
            keyTapCount = 0;
        }

        if (keyTapCount > 2)
        {
            keyTapCount = 0;
        }
    }

    private void CharacterDashControl()
    {
        if (hasDashed)
        {
            if (isDashing)
                dashDurCount += 1 * Time.deltaTime;

            dashCDRCount += 1 * Time.deltaTime;
            transform.position += dir * dashSpeed * Time.deltaTime;
        }

        if (dashDurCount >= dashDur && isDashing) //End of the dash
        {
            dashDur = 1.5f;
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
