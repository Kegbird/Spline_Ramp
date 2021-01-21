using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Assets.Scripts;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private bool accelerate;
    [SerializeField]
    public UnityEvent pick_coin;
    public UnityEvent win;

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
            win.Invoke();
        }
        if(collision.gameObject.layer==Constants.COIN_LAYER_MASK)
        {
            collision.gameObject.SetActive(false);
            pick_coin.Invoke();
        }
    }

    IEnumerator Accelerate()
    {
        while(accelerate)
        {
            rigidbody2D.velocity += rigidbody2D.velocity * Constants.ACCELERATION_FACTOR;
            yield return new WaitForSeconds(Constants.ACCELERATION_TICK);
        }
        yield return null;
    }
}
