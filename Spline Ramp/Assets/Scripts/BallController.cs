using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private bool accelerate;

    private void OnEnable()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            accelerate = true;
            StartCoroutine(Accelerate());
        }
        else if (Input.GetKeyUp(KeyCode.A))
            accelerate = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer==Constants.GOAL_LAYER_MASK)
        {
            Debug.Log("Win!");
        }

    }

    IEnumerator Accelerate()
    {
        while(accelerate)
        {
            Debug.Log(String.Format("Velocity: {0} {1}", rigidbody2D.velocity.x, rigidbody2D.velocity.y));
            rigidbody2D.velocity += rigidbody2D.velocity * Constants.ACCELERATION_FACTOR;
            yield return new WaitForSeconds(Constants.ACCELERATION_TICK);
        }
        yield return null;
    }
}
