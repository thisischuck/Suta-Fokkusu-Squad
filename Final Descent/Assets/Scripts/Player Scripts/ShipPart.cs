/*
Manages the ship components. It's responsible for calculating the explosion and applying it
 
 12-10-2018
 The script calculates forces correctly and applies it sucessufully.
 It has a enum to indicate in what side of the ship the component is found and has a struct that contains the variables of the impact

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartSide { Left, Right, None};

public struct ImpactForce
{
    public Vector3 direction;
    public float force;
    public PartSide side;
    public float decay;
}

public class ShipPart : MonoBehaviour {

    public PartSide side;
    public bool explode = false;
    public float gravity;
    public ImpactForce impact;

    private Vector3 explosionPos;
    private Quaternion explosionRot;
    public float speed = .2f;
	public bool onPlace = true, defDeath = false;
    public float timeStartRepair = 0;

	// Use this for initialization
	void Start () {
        impact = new ImpactForce();
        gravity = 9.8f;
    }
	
	// Update is called once per frame
	void Update () {
        if (explode) //If explosion occurs
        {
            ReceiveImpact(ref impact); //Receives impact
            gravity += 2f * Time.deltaTime; //Gravity
            Rotate(Random.Range(5,15)); //Rotates
        }
        if (!onPlace && Time.time >= timeStartRepair && !defDeath)
        {
            explode = false;
            if (!AssemblyComplete())
            {
                GetInPlace();
            }
            else onPlace = true;
        }
	}

    public void Explode() //Initiates explosion
    {
        explosionPos = transform.position;
        explosionRot = transform.rotation;
        explode = true;
        transform.parent = null;
        impact = GenerateRandomImpact();
        onPlace = false;
    }

    ImpactForce GenerateRandomImpact() //Calculates impact force, direction and decay
    {
        ImpactForce f = new ImpactForce();
        f.force = Random.Range(20.0f, 60.0f);
        f.decay = Random.Range(0.25f, 2f);
        switch(side)
        {
            case PartSide.Left:
                f.side = PartSide.Left;
                f.direction = new Vector3(-1, Random.Range(-1.0f, 1.0f), 0);
                f.direction = f.direction.normalized;
                break;

            case PartSide.Right:
                f.side = PartSide.Right;
                f.direction = new Vector3(1, Random.Range(-1.0f, 1.0f), 0);
                f.direction = f.direction.normalized;
                break;
        }

        return f;
    }

    void ReceiveImpact(ref ImpactForce impact) //Applies the impact
    {
        if (impact.direction.magnitude > 0.2)
        {
            Move(impact.direction, impact.force);
            timeStartRepair = Time.time + .25f;
        }
        // consumes the impact energy each cycle:
        impact.direction = Vector3.Lerp(impact.direction, Vector3.zero, impact.decay * Time.deltaTime);
    }

    void Move(Vector3 direction, float velocity) //Moves the object based on a direction and a speed
    {
        transform.position += direction * velocity * Time.deltaTime;
    }

    void Rotate(float angle) //Rotates the object
    {
        transform.Rotate(new Vector3(0, 0, angle));
    }

    void GetInPlace()
    {
        float distance = Vector3.Distance(transform.position, explosionPos);
        float rotDistance = Quaternion.Angle(transform.rotation, explosionRot);
        if (distance < 1) distance = 1;
        if (rotDistance < 1) rotDistance = 1;
        transform.position = Vector3.MoveTowards(transform.position, explosionPos, Time.deltaTime * (speed * distance));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, explosionRot, Time.deltaTime * (speed * rotDistance));
    }

    private bool AssemblyComplete()
    {
        if (explosionPos == transform.position && explosionRot == transform.rotation)
            return true;
        else return false;
    }
}
