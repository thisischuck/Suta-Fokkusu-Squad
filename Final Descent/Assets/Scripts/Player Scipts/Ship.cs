/*
Manages the actual ship. With this script we can execute rotations that only affect the ship.
 
 12-10-2018
 The script executes correctly the dash rotation.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    float rotX, rotY, rotZ;
    float aux = 0;

    // Sticks to the Player Controller
    void FixedUpdate()
    {
        transform.position = transform.parent.position;
    }

    //Starts the dash rotation
    public void DashRotation(float angleZ, float duration)
    {
        StartCoroutine(DashRotate(angleZ, duration));
    }

    private IEnumerator DashRotate(float angleZ, float duration)
    {
        float aux;
        Quaternion localRotation;
        for (float clock = 0; clock < duration; clock += Time.deltaTime)
        {
            rotZ += (angleZ / duration * Time.deltaTime);

            aux = Mathf.Sign(angleZ);

            switch ((int)aux) //Checks if the angle is negative or positive
            {
                case -1:
                    if (rotZ <= angleZ)
                        rotZ = angleZ;

                    localRotation = Quaternion.Euler(0f, 0f, rotZ);

                    transform.localRotation = localRotation;

                    if (rotZ <= angleZ)
                    {
                        rotZ = 0;
                    }
                    break;

                case 1:
                    if (rotZ >= angleZ)
                        rotZ = angleZ;

                    localRotation = Quaternion.Euler(0f, 0f, rotZ);

                    transform.localRotation = localRotation;
                    if (rotZ >= angleZ)
                    {
                        rotZ = 0;
                    }
                    break;
            }

            if (transform.localRotation.z != 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            yield return null;
        }
    }
}
