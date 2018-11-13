using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    EM FALTA: turbinas e flaps (top flaps) a rodar com a rotaçao da nave quando é movida pela rato, rotaçoes para a frente e em conjunto com as para o lado
              intaker flaps a rodar quando quando é carregado no Q ou E
     */
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
        initialRot = transform.localRotation;
        initialRot1 = new Vector3(0.0f, 0.0f, 0.0f);

    }

    // Update is called once per frame
    void Update()
    {
        Quaternion target;

        forwardAxis = Input.GetAxis("Vertical");
        sideAxis = Input.GetAxis("Horizontal");

        if (Input.GetButton("Vertical"))
        {
            leftTarget = new Vector3(-90.0f, 0.0f, 0.0f);
            rightTarget = new Vector3(0.0f, 0.0f, 0.0f);

            #region ForwardWeigths
            if (forwardAxis < 0.0f)
            {
                leftWeight = -forwardAxis;
            }
            if (forwardAxis > 0.0f)
            {
                rightWeight = forwardAxis;
            }
            if (forwardAxis == 0.0f)

                restweight = 0.0f;

            target = Quaternion.Euler((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(forwardAxis) + ((1 - Mathf.Abs(forwardAxis)) * initialRot1)); //initialRot1 =  new Vector3(0.0f, 0.0f,0.0f);
            //Quaternion target = Quaternion.Euler((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(sideAxis) + ((restweight * initialRot1)));
            transform.localRotation = target * initialRot;
            //Debug.Log((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(sideAxis) + ((1 - Mathf.Abs(sideAxis)) * initialRot1));
            #endregion
        }
        else
        {
            restweight = 0.0f;

            target = Quaternion.Euler((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(forwardAxis) + ((1 - Mathf.Abs(forwardAxis)) * initialRot1));
            transform.localRotation = target * initialRot;
        }

        if (Input.GetButton("Horizontal"))
        {
            leftTarget = new Vector3(90.0f, -90.0f, 0.0f);
            rightTarget = new Vector3(90.0f, 90.0f, 0.0f);

            #region SideWeigths
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

            target = Quaternion.Euler((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(sideAxis) + ((1 - Mathf.Abs(sideAxis)) * initialRot1)); //initialRot1 =  new Vector3(0.0f, 0.0f,0.0f);
            //Quaternion target = Quaternion.Euler((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(sideAxis) + ((restweight * initialRot1)));
            transform.localRotation = target * initialRot;
            //Debug.Log((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(sideAxis) + ((1 - Mathf.Abs(sideAxis)) * initialRot1));
            #endregion
        }
        else
        {
            restweight = 0.0f;

            target = Quaternion.Euler((leftTarget * leftWeight + rightTarget * rightWeight) * Mathf.Abs(sideAxis) + ((1 - Mathf.Abs(sideAxis)) * initialRot1));
            transform.localRotation = target * initialRot;
        }

        Blades.transform.Rotate(new Vector3(0.0f, 0.0f, Time.deltaTime * bladeSpeed));
    }
}