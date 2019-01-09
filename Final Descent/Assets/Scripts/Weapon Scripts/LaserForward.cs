using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserForward : MonoBehaviour
{
    public float secondsToDeath = 5.0f;
    public Vector3 Velocity;
    public float Speed = 30.0f;
    private bool hasStarted;
    public int damage = 5;

    void Start()
    {
        transform.forward = Velocity;
        StartCour();
    }

    void Update()
    {
        transform.position += Velocity * Time.deltaTime * Speed;
    }

    IEnumerator DeathTime()
    {
        hasStarted = true;

        yield return new WaitForSeconds(secondsToDeath);

        Destroy(this.gameObject);
    }

    public void StartCour()
    {
        if (!hasStarted)
        {
            StartCoroutine(DeathTime());
        }
    }
}
