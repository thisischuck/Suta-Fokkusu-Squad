using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineMovement : MonoBehaviour
{
    float bladeSpeed = 400f;
    public GameObject Blades;

    float forwardAxis;
    float sideAxis;

    Vector3 initialRot1, leftTarget, rightTarget;
    float leftWeight, rightWeight, restweight;
    Quaternion initialRot;

    // Use this for initialization
    void Start()
    {
        leftTarget = new Vector3(90.0f, -90.0f, 0.0f);
        rightTarget = new Vector3(90.0f, 90.0f, 0.0f);

        initialRot = transform.localRotation;

        initialRot1 = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        forwardAxis = Input.GetAxis("Vertical");
        sideAxis = Input.GetAxis("Horizontal");

        if (sideAxis < 0.0f)
        {
            leftWeight = -sideAxis;
        }
        if (sideAxis > 0.0f)
        {
            rightWeight = sideAxis;
        }
        if (sideAxis == 0.0f)

            restweight = 0.0f;

        Quaternion target = Quaternion.Euler((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(sideAxis) + ((1 - Mathf.Abs(sideAxis)) * initialRot1)); //initialRot1 =  new Vector3(0.0f, 0.0f,0.0f);
        //Quaternion target = Quaternion.Euler((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(sideAxis) + ((restweight * initialRot1)));
        transform.localRotation = target * initialRot;
        //Debug.Log((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(sideAxis) + ((1 - Mathf.Abs(sideAxis)) * initialRot1));

        Blades.transform.Rotate(new Vector3(0.0f, 0.0f, Time.deltaTime * bladeSpeed));
    }
}