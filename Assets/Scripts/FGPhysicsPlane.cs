using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGPhysicsPlane : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_EnergyDissipation = 0.1f;
    [SerializeField] private float m_StaticBodyVelocityLimit = 0.2f;
    [SerializeField] private float m_DeltaMoveCoef = 0.2f;
    [SerializeField] private float m_CorrectedPostionCoef = 5.0f;

    List<FGPhysicsBody> m_FGPhysicBodies = default;

    Vector3 Normal => transform.up;
    Vector3 ParentVelocity = Vector3.zero;

    void Start()
    {
        m_FGPhysicBodies = new List<FGPhysicsBody>(FindObjectsOfType<FGPhysicsBody>());
    }

    private void FixedUpdate()
    {
        UpdateSpheres();
    }

    private void UpdateSpheres()
    {
        foreach (var body in m_FGPhysicBodies)
        {
            if (body.CollidingWithPlane(this))
            {
                Shock(body);
            }
        }
    }

    private void Shock(FGPhysicsBody body)
    {
        if (IsColliding(body) == false)
            return;;

        if (IsBodyStaticOnPlane(body))
        {
            body.transform.position = CorrectedPosition(body);
            body.ApplyForce(-body.Mass * Physics.gravity);
        }
        else
        {
            body.transform.position = CorrectedPosition(body);
            InverseRelativeVelocity(body, Reflect(RelativeVelocity(body)));
        }
    }

    private bool IsColliding(FGPhysicsBody body)
    {
        if (WillBeCollision(body) == false)
            return false;

        return Distance(body) >= 0f || Mathf.Abs(Distance(body)) <= body.Radius;
        throw new NotImplementedException();
    }

    private bool WillBeCollision(FGPhysicsBody body)
    {
        return Vector3.Dot(RelativeVelocity(body), Normal) < 0f;
    }

    private Vector3 RelativeVelocity(FGPhysicsBody body)
    {
        return body.Velocity - ParentVelocity;
    }

    private float Distance(FGPhysicsBody body)
    {
        Vector3 bodyToPlane = transform.position - body.transform.position;

        return Vector3.Dot(bodyToPlane, Normal);
    }

    public bool IsBodyStaticOnPlane(FGPhysicsBody body)
    {
        bool lowVelocity = RelativeVelocity(body).magnitude < m_StaticBodyVelocityLimit;

        return lowVelocity && TouchingThePlane(body);
    }

    private bool TouchingThePlane(FGPhysicsBody body)
    {
        float deltaMove = Mathf.Max(m_DeltaMoveCoef * body.Radius, RelativeVelocity(body).magnitude * Time.fixedDeltaTime);
        return (CorrectedPosition(body) - body.transform.position).magnitude <= m_CorrectedPostionCoef * deltaMove;
    }

    private Vector3 CorrectedPosition(FGPhysicsBody body)
    {
        return Projection(body) + Normal * body.Radius;
    }

    private Vector3 Projection(FGPhysicsBody body)
    {
        Vector3 bodyToProjection = Distance(body) * Normal;

        return body.transform.position + bodyToProjection;
    }

    private void InverseRelativeVelocity(FGPhysicsBody body, Vector3 vel)
    {
        body.Velocity = vel + 2f * ParentVelocity;
    }

    private Vector3 Reflect(Vector3 v)
    {
        return (v - 2f * Vector3.Dot(v, Normal) * Normal) * (1f - m_EnergyDissipation);
    }
}
