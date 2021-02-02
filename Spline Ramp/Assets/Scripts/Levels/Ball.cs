using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Assets.Scripts;
using Assets.Scripts.Levels;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private bool accelerate;
    [SerializeField]
    public UIManager ui_manager;
    [SerializeField]
    public UnityEvent pick_coin;
    public UnityEvent win;
    [SerializeField]
    public AudioSource audio_source;
    [SerializeField]
    public AudioClip bounce_fx;
    [SerializeField]
    public AudioClip obstacle_fx;

    private void OnEnable()
    {
        audio_source = GetComponent<AudioSource>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        ui_manager.HideAccelerationText();
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
        if (collision.gameObject.layer == Constants.GOAL_LAYER_MASK)
        {
            win.Invoke();
        }
        if (collision.gameObject.layer == Constants.COIN_LAYER_MASK)
        {
            collision.gameObject.SetActive(false);
            pick_coin.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Constants.OBSTACLE_LAYER_MASK)
            PlayObstacleSound();
        if (collision.gameObject.layer == Constants.MATTRESS_LAYER_MASK)
            PlayBounceSound();
    }

    IEnumerator Accelerate()
    {
        ui_manager.ShowAccelerationText();
        while (accelerate)
        {
            rigidbody2D.velocity += rigidbody2D.velocity * Constants.ACCELERATION_FACTOR;
            yield return new WaitForSeconds(Constants.ACCELERATION_TICK);
        }
        ui_manager.HideAccelerationText();
        yield return null;
    }

    public void PlayBounceSound()
    {
        audio_source.clip = bounce_fx;
        audio_source.Play();
    }

    public void PlayObstacleSound()
    {
        audio_source.clip = obstacle_fx;
        audio_source.Play();
    }
}
