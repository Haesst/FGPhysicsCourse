using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGPhysicsBody : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] float m_Mass = 1.0f;
    [SerializeField] bool m_UseGravity = true;
    [SerializeField] float m_Drag = 2f;

    private List<FGPhysicsPlane> m_CurrentlyCollidingPlanes = new List<FGPhysicsPlane>();

    private Vector3 m_Velocity;
    public Vector3 Velocity { get => m_Velocity; set => m_Velocity = value; }
    public float Mass => m_Mass;
    public float Radius => transform.localScale.y * 0.5f;

    private void FixedUpdate()
    {
        if (CurrentlyStaticOnAPlane())
        {
            Velocity = Vector3.zero;
        }

        ApplyForce(Vector3.zero);
        ApplyDynamicDrag();
    }

    public bool CurrentlyStaticOnAPlane()
    {
        foreach (var plane in m_CurrentlyCollidingPlanes)
        {
            if (plane.IsBodyStaticOnPlane(this))
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        FGPhysicsPlane plane = other.GetComponent<FGPhysicsPlane>();

        if (plane)
        {
            m_CurrentlyCollidingPlanes.Add(plane);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FGPhysicsPlane plane = other.GetComponent<FGPhysicsPlane>();

        if (plane && m_CurrentlyCollidingPlanes.Contains(plane))
        {
            m_CurrentlyCollidingPlanes.Remove(plane);
        }
    }

    public bool CollidingWithPlane(FGPhysicsPlane plane)
    {
        return m_CurrentlyCollidingPlanes.Contains(plane);
    }

    public void ApplyForce(Vector3 force, bool overrideGravity = false)
    {
        Vector3 totalForce = m_CurrentlyCollidingPlanes.Count <= 0 ? force + m_Mass * Physics.gravity : force;

        if (overrideGravity)
        {
            totalForce = force;
        }

        // f = m * a
        // a = f / m
        Vector3 acc = totalForce / m_Mass;
        Integrate(acc);
    }

    void Integrate(Vector3 acc)
    {
        transform.position +=
            Velocity * Time.fixedDeltaTime +
            acc * Time.fixedDeltaTime * Time.fixedDeltaTime * 0.5f;

        Velocity += acc * Time.fixedDeltaTime * 0.5f * (1.0f - m_Drag);
    }

    private void ApplyDynamicDrag()
    {
        Vector3 dragForce = -DynamicDragAmount() * Velocity;

        ApplyForce(dragForce, true);
    }

    private float DynamicDragAmount()
    {
        return Vector3.Dot(Velocity, Velocity) * m_Drag / 1000f;
    }
}
