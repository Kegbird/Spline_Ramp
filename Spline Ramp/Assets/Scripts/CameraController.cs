using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private bool simulation_lock_ball;
    [SerializeField]
    private Transform ball_transform;
    [SerializeField]
    private Vector3 last_pan_position;
    [SerializeField]
    private Transform camera_default_transform;

    // Update is called once per frame
    private void Update()
    {
        if (simulation_lock_ball)
        {
            Vector3 target_position = new Vector3(ball_transform.position.x, ball_transform.position.y, transform.position.z);
            transform.position = target_position;
            return;
        }

        if(Input.GetAxis("Mouse ScrollWheel")!=0)
            Zoom();
        if (Input.GetMouseButtonDown(2))
            last_pan_position = Input.mousePosition;
        else if(Input.GetMouseButton(2))
            Pan(Input.mousePosition);
    }

    public void StartSimulationLock(Transform ball_transform)
    {
        simulation_lock_ball = true;
        this.ball_transform = ball_transform;
    }

    public void StopSimulation()
    {
        simulation_lock_ball = false;
        transform.position = new Vector3(camera_default_transform.position.x, 
                                         camera_default_transform.position.y, 
                                         transform.position.z);
    }

    private void Zoom()
    {
        float amount = -Input.GetAxis("Mouse ScrollWheel");
        amount = Mathf.Clamp( Camera.main.orthographicSize+Constants.ZOOM_SPEED*amount,
                                                        Constants.MIN_ORT_SIZE,
                                                        Constants.MAX_ORT_SIZE);
        Camera.main.orthographicSize = amount;
    }

    private void Pan(Vector3 new_pan_position)
    {
        Vector3 offset = Camera.main.ScreenToViewportPoint(last_pan_position - new_pan_position);
        offset.x *= Constants.PAN_SPEED;
        offset.y *= Constants.PAN_SPEED;
        offset.z= 0;
        last_pan_position = new_pan_position;
        transform.position += offset;
    }
}
