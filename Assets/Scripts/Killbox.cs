using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GolfBall ball = other.GetComponent<GolfBall>();

        if (ball != null)
        {
            Debug.Log("Ball found");
            ball.MoveBackBall();
        }
    }
}
