using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRotation : MonoBehaviour {
    private float rotZ;

    //Starts the dash rotation
    public void DashRotation(float angleZ, float duration)
    {
        StartCoroutine(DashRotate(angleZ, duration));
    }

    //Ienmuerator for the rotation.
    private IEnumerator DashRotate(float angleZ, float duration)
    {
        float aux;
        Quaternion localRotation;
        for (float clock = 0; clock < duration; clock += Time.deltaTime)
        {
            rotZ += (angleZ / (duration / Time.smoothDeltaTime));

            //Debug.Log(rotZ);
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

                        if (transform.localRotation.z != 0)
                        {
                            transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
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

                        if (transform.localRotation.z != 0)
                        {
                            transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                    }
                    break;
            }



            yield return null;
        }

        if (transform.localRotation.z != 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
            rotZ = 0;
        }
    }
}
