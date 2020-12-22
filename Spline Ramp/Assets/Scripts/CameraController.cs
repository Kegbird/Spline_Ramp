using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private bool lock_ball;
    [SerializeField]
    private Transform ball_transform;
    [SerializeField]
    private Vector3 last_pan_position;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        if(Input.GetKeyDown(KeyCode.Space))
            lock_ball = !lock_ball;

        if (lock_ball)
        {
            Vector3 target_position = new Vector3(ball_transform.position.x, ball_transform.position.y, transform.position.z);
            transform.position = target_position;
        }

        if(Input.GetAxis("Mouse ScrollWheel")!=0)
            Zoom();

        if (Input.GetMouseButtonDown(2))
        {
            last_pan_position = Input.mousePosition;
        }
        else if(Input.GetMouseButton(2))
        {
            Pan(Input.mousePosition);
        }  
    }

    void Zoom()
    {
        float amount = Input.GetAxis("Mouse ScrollWheel");
        float z = Mathf.Clamp(transform.position.z + (Constants.ZOOM_SPEED * amount),
                                                    Constants.MIN_DEPTH_CAMERA,
                                                    Constants.MAX_DEPTH_CAMERA);
        transform.position = new Vector3(transform.position.x,
                                        transform.position.y, z);
    }

    void Pan(Vector3 new_pan_position)
    {
        Vector3 offset = Camera.main.ScreenToViewportPoint(last_pan_position - new_pan_position);
        offset.x *= Constants.PAN_SPEED;
        offset.y *= Constants.PAN_SPEED;
        offset.z= 0;
        last_pan_position = new_pan_position;
        transform.position += offset;
    }
}
