﻿/*
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

    private Vector3 dadPosition;

    // Use this for initialization
    void Start()
    {
        dollyDir = (transform.localPosition + dollyDirAdjusted).normalized;
        distance = transform.localPosition.magnitude;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit))
        {
            if (hit.transform.tag != "Player" && hit.transform.tag != "PlayerPart" && hit.transform.tag != "Ship" && hit.transform.name == "AircraftController")
            {
                Debug.Log(hit.transform.tag);
                distance = Mathf.Clamp((hit.distance * 0.8f), minDistance, maxDistance);
            }
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}

