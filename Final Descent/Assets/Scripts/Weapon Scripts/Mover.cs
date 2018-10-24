using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    public float speed;
    public float range = 100f;
    public float Damage = 10f;

    public Vector3 shotPos;
    public Quaternion shotRot;
    public float secondsToDeath;
    public bool isActive;
    public bool hasStarted;

    // Use this for initialization
    void Start()
    {

    }

    public void setPosition(Vector3 shotPos, Quaternion shotRot)
    {
        transform.position = shotPos;
        transform.rotation = shotRot;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position += speed * transform.forward;
    }

    IEnumerator DeathTime()
    {
        hasStarted = true;

        yield return new WaitForSeconds(secondsToDeath);

        this.gameObject.SetActive(false);

        hasStarted = false;
    }

    public void StartCour()
    {
        if (!hasStarted)
        {
            StartCoroutine(DeathTime());
        }
    }
}
