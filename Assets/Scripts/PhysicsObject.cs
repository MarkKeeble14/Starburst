using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [Header("Physics Object")]
    public bool enableVelocity;
    public bool enableAcceleration;
    [Header("1st Law")]
    public float mass = 1;
    public Vector3 velocity;
    public Vector3 acceleration;

    [Header("2nd Law")]
    public TrackForcesMode trackForcesMode;
    public List<Vector3> forceVectorList = new List<Vector3>();
    public Vector3 forceSum;


    public void AddForce(Vector3 force)
    {
        switch (trackForcesMode)
        {
            case TrackForcesMode.SUM:
                SumForces();
                Vector3 newSum = forceSum + force;
                forceVectorList.Clear();
                forceVectorList.Add(newSum);
                break;
            case TrackForcesMode.ADD:
                forceVectorList.Add(force);
                break;
        }
    }

    public void AddForce(Vector3 force, PhysicsObject objectAddingForce)
    {
        AddForce(force);
        objectAddingForce.AddForce(-force);
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    protected void Update()
    {
        switch (trackForcesMode)
        {
            case TrackForcesMode.SUM:
                SumForces();
                Vector3 currentSum = forceSum;
                forceVectorList.Clear();
                forceVectorList.Add(currentSum);
                break;
            default:
                break;
        }
    }

    protected void FixedUpdate()
    {
        // Part 3
        // Newtons 2nd Law
        SumForces();
        acceleration = forceSum / mass;

        // Adding Acceleration to Velocity
        if (enableAcceleration)
            velocity += acceleration * Time.fixedDeltaTime;

        // Part 1
        // Newtons 1st Law
        if (enableVelocity)
        {
            transform.position += velocity * Time.fixedDeltaTime;
        }
    }


    // Determines the sum of forces acting upon this object
    public void SumForces()
    {
        forceSum = Vector3.zero;
        foreach (Vector3 v in forceVectorList)
        {
            forceSum += v;
        }
    }
}
