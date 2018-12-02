using System.Collections.Generic;
using UnityEngine;

public static class EnemyBehaviours
{
    public static float wanderDistance = 20.0f;
    public static float wanderRadius = 8.0f;

    public static Vector3 Seek(Transform t, Vector3 velocity, Vector3 target)
    {
        Vector3 velocitySeek = (target - t.position).normalized;
        return (velocitySeek * 0.1f + velocity).normalized;
    }

    public static Vector3 Pursuit(Transform t, Vector3 velocity, Transform target, float prediction)
    {
        return Seek(t, velocity, target.position + target.forward * prediction);
    }

    public static Vector3 CalculateMidPos(List<GameObject> friends)
    {
        Vector3 midPos = Vector3.zero;
        foreach (GameObject f in friends)
        {
            midPos += f.transform.position;
        }

        return midPos / friends.Count;
    }

    public static Vector3 Cohesion(Transform t, List<GameObject> friends)
    {
        Vector3 accel = (CalculateMidPos(friends) - t.position);
        //In the case that boids are in the same place, it's impossible to normalize a vector of zero
        if (accel == Vector3.zero)
        {
            return Vector3.zero;
        }

        return accel.normalized;
    }

    public static Vector3 Alignment(List<GameObject> friends, Vector3 velocity)
    {
        Vector3 avgAccel = Vector3.zero;
        foreach (GameObject f in friends)
        {
            avgAccel += f.transform.forward;
        }
        if (avgAccel == Vector3.zero)
        {
            return Vector3.zero;
        }

        return ((avgAccel - velocity) / friends.Count).normalized;
    }

    public static Vector3 Separation(Transform t, List<GameObject> friends)
    {
        Vector3 distance = Vector3.zero;
        foreach (GameObject f in friends)
        {
            float x = t.position.x - f.transform.position.x;
            float y = t.position.y - f.transform.position.y;
            float z = t.position.z - f.transform.position.z;

            //To avoid getting one divided by zero
            if (x != 0)
            {
                x = 1 / x;
            }

            if (y != 0)
            {
                y = 1 / y;
            }

            if (z != 0)
            {
                z = 1 / z;
            }

            distance += new Vector3(x, y, z);
        }
        if (distance == Vector3.zero)
        {
            return Vector3.zero;
        }

        return distance.normalized;
    }

    public static Vector3 Wander(Transform t, Vector3 velocity)
    {
        Vector3 circleCenter = t.position + velocity * wanderDistance;
        float phi = Random.Range(0.0f, 2.0f * Mathf.PI);
        float theta = Random.Range(0.0f, 2.0f * Mathf.PI);
        float x = wanderRadius * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = wanderRadius * Mathf.Sin(theta) * Mathf.Sin(phi);
        float z = wanderRadius * Mathf.Cos(theta);
        Vector3 target = circleCenter + new Vector3(x, y, z);
        return Seek(t, velocity, target);
    }

    public static Quaternion RotateTowards(Transform t, Transform target, float rotSpeed)
    {
        return Quaternion.RotateTowards(
                t.rotation,
                Quaternion.LookRotation(target.transform.position - t.position, Vector3.up),
                Time.deltaTime * rotSpeed);
    }

    /// <summary>
    /// Esta função serve para calcular matemáticamente o ângulo entre 2 vetores. no entanto, ao contrario da Vector3.Angle, esta retorna números negativos
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    public static Vector3 AvoidObstacles(Transform t, Vector3 velocity, ref bool isThereAnything)
    {
        RaycastHit hit, hitL, hitR;
        Vector3 dir = t.forward;

        if (Physics.SphereCast(t.position, 2f, t.forward, out hit, 5))
        {
            if (hit.collider.gameObject.CompareTag("Stalactite"))
            {
                isThereAnything = true;
                Vector3 collDir = hit.point - t.position;
                collDir = collDir.normalized;
                Vector3 normalAxis = t.up.normalized;
                float angle = AngleSigned(t.forward, collDir, normalAxis);
                
                if (angle > -170 && angle < 170)
                {
                    if (angle < 0) //Esquerda
                    {
                        t.Rotate(Vector3.up * 15f);
                        dir = t.forward;
                        Debug.Log("Going left");
                    }
                    else if (angle >= 0)
                    {
                        t.Rotate(Vector3.up * -15f);
                        dir = t.forward;
                        Debug.Log("Going right");
                    }
                }
                else
                {
                    dir = t.forward;
                    isThereAnything = false;
                }
            }
        }
        else
        {
            dir = t.forward;
            isThereAnything = false;
        }

        return dir;
    }
}
