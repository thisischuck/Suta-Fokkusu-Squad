using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyBehaviours
{
    public static float wanderDistance = 10.0f;
    public static float wanderRadius = 5.0f;

    public static Vector3 Seek(Transform t, Vector3 velocity, Vector3 target)
    {
        Vector3 velocitySeek = (target - t.position).normalized;
        return (velocitySeek + velocity).normalized;
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
            return Vector3.zero;
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
            return Vector3.zero;
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
            if (x != 0) x = 1 / x;
            if (y != 0) y = 1 / y;
            if (z != 0) z = 1 / z;

            distance += new Vector3(x, y, z);
        }
        if (distance == Vector3.zero)
            return Vector3.zero;
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
}
