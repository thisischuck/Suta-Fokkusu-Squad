/*
Manages the camera collision
 
 12-10-2018
 The script is complete and executes its task sucessufully.
 28-12-2018
 The script was optimized 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public float minDistance = 0.6f;
    public float maxDistance = 3.0f;

    public float smooth = 30.0f;
    Vector3 dollyDir;

    [Tooltip("Offset of the camera position")]
    public Vector3 dollyDirAdjusted;
    public float distance;
    public Transform dad;

    public Vector3 dadPosition;

    // Use this for initialization
    void Start()
    {
        dollyDir = (transform.localPosition + dollyDirAdjusted).normalized;
        distance = transform.localPosition.magnitude;
        dad = transform.parent;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredCameraPos = dad.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;
        dadPosition = dad.position;
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        Debug.DrawLine(transform.position, dad.position, Color.red);

        if (Physics.Linecast(dad.position, desiredCameraPos, out hit))
        {
            //Debug.Log("HEY");
            if (hit.transform.tag != "Player" && hit.transform.tag != "PlayerPart" && hit.transform.tag != "Ship" && hit.transform.tag != "AircraftController" && hit.transform.tag != "Gun")
            {
                Debug.Log(hit.transform.tag);
                distance = Mathf.Clamp((hit.distance * 0.8f), minDistance, maxDistance);
            }
            else
            {
                distance = maxDistance;
            }
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}

