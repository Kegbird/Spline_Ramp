using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 last_pan_position;

    // Update is called once per frame
    private void Update()
    {   
        if (Input.GetMouseButtonDown(2))
            last_pan_position = Input.mousePosition;
        else if(Input.GetMouseButton(2))
            Pan(Input.mousePosition);
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
