using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(FGPhysicsPlane))]
public class GolfHole : MonoBehaviour
{
    FGPhysicsPlane holePlane = default;

    void Start()
    {
        holePlane = GetComponent<FGPhysicsPlane>();

        Assert.IsNotNull(holePlane, "No plane found in hole");
    }

    private void OnTriggerEnter(Collider other)
    {
        GolfBall ball = other.GetComponent<GolfBall>();

        if (ball != null)
        {
            Debug.Log("Ball in hole!");
        }
    }
}
