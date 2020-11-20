using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbox : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }

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
