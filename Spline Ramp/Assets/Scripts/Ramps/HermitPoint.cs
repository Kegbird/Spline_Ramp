using UnityEngine;
using Assets.Scripts;

public class HermitPoint : MonoBehaviour
{
    [SerializeField]
    private bool rotating;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << Constants.ARROW_LAYER_MASK);
            if (hit.collider != null && hit.collider.gameObject==this.gameObject)
                rotating = true;
        }
        if(Input.GetMouseButton(0) && rotating)
        {
            Vector2 mouse_position=Input.mousePosition;
            Transform axis = transform.parent;
            Vector3 axis_screen_position = Camera.main.WorldToScreenPoint(axis.transform.position);
            float angle = Mathf.Atan2(mouse_position.y - axis_screen_position.y, mouse_position.x - axis_screen_position.x) * Mathf.Rad2Deg;
            axis.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        if(Input.GetMouseButtonUp(0))
        {
            rotating = false;
        }
    }
}
