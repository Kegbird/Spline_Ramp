using UnityEngine;
using Assets.Scripts;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private SimulationManager simulation_manager;
    [SerializeField]
    private UIManager ui_manager;
    [SerializeField]
    private bool editing;
    [SerializeField]
    private EditingMode editing_mode;
    [SerializeField]
    GameObject grabbed_gameobject;
    [SerializeField]
    private Vector3 translation_offset;
    [SerializeField]
    private Vector3 rotation_offset;
    [SerializeField]
    private Transform boundary;
    [SerializeField]
    private GameObject ramp;
    [SerializeField]
    private LineRenderer ramp_line_renderer;
    [SerializeField]
    private EdgeCollider2D ramp_edge_collider;
    [SerializeField]
    private int point_index = 0;

    private void Awake()
    {
        editing_mode = EditingMode.None;
        simulation_manager = GetComponent<SimulationManager>();
        ui_manager = GetComponent<UIManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            DisplayHideLevelManagerUI();
        if (Input.GetKeyDown(KeyCode.T))
            DisplayHideMoveModeUI();
        if (Input.GetKeyDown(KeyCode.D))
            DisplayHideDeleteModeUI();
        if (Input.GetKeyDown(KeyCode.R))
            DisplayHideRotateModeUI();
        if (Input.GetKeyDown(KeyCode.M))
            DisplayHideCreateEditSplineUI();
        PerformEditMode();
    }

    private void PerformEditMode()
    {
        switch (editing_mode)
        {
            case EditingMode.None:
                break;
            case EditingMode.MoveMode:
                MoveGrabbedObject();
                break;
            case EditingMode.RotateMode:
                RotateObject();
                break;
            case EditingMode.DeleteMode:
                DeleteObject();
                break;
            default:
                CreateEditSegmentRamp();
                break;
        }
    }

    private void CreateEditSegmentRamp()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 position = GetMouseWorldCoordinates();
            if (ramp == null)
            {
                ramp = new GameObject();
                ramp.name = "Ramp";
                ramp.tag = "Interactable";
                ramp.transform.position = position;
                ramp_line_renderer = ramp.AddComponent<LineRenderer>();
                ramp_line_renderer.startWidth = Constants.RAMP_EDGE_RADIUS;
                ramp_line_renderer.endWidth = Constants.RAMP_EDGE_RADIUS;
                ramp_line_renderer.SetPosition(point_index, position);
                point_index++;
            }
            else
            {
                ramp_line_renderer.positionCount++;
                point_index++;
            }
        }
        else if(Input.GetMouseButton(0))
        {

        }
        else if(Input.GetMouseButtonDown(1))
        {
            if (ramp == null)
                return;
            BuildRampCollider();
        }

        if(point_index>0)
            ramp_line_renderer.SetPosition(point_index, GetMouseWorldCoordinates());
    }

    private void MoveGrabbedObject()
    {
        if (grabbed_gameobject == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Raycast
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    grabbed_gameobject = hit.collider.gameObject;
                    translation_offset = GetMouseWorldCoordinates() - grabbed_gameobject.transform.position;
                }
            }
        }
        else if (grabbed_gameobject != null)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 new_position = GetMouseWorldCoordinates() - translation_offset;
                new_position.x = Mathf.Clamp(new_position.x, Constants.MIN_X_BOUNDARY, boundary.position.x);
                grabbed_gameobject.transform.position = new_position;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                grabbed_gameobject = null;
                translation_offset = Vector3.zero;
            }
        }
    }

    private void RotateObject()
    {
        if (grabbed_gameobject == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject.tag == "Pivot")
                {
                    grabbed_gameobject = hit.collider.gameObject;
                    rotation_offset = hit.collider.gameObject.transform.position - GetMouseWorldCoordinates();
                }
            }
        }
        else if (grabbed_gameobject != null)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mouse_position = Input.mousePosition;
                mouse_position.z = 0;
                Vector3 grabbed_gameobject_position = Camera.main.WorldToScreenPoint(grabbed_gameobject.transform.parent.position-rotation_offset);
                float angle = Mathf.Atan2(mouse_position.y-grabbed_gameobject_position.y, mouse_position.x-grabbed_gameobject_position.x) * Mathf.Rad2Deg;
                grabbed_gameobject.transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                grabbed_gameobject = null;
                rotation_offset = Vector3.zero;
            }
        }
    }

    private void DeleteObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Raycast
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.tag=="Interactable")
                Destroy(hit.collider.gameObject);
        }
    }

    private Vector3 GetMouseWorldCoordinates()
    {
        Vector3 position = Input.mousePosition;
        position = Camera.main.ScreenToWorldPoint(position);
        position.z = 0;
        return position;
    }

    private void BuildRampCollider()
    {
        ramp_line_renderer.positionCount -= 1;
        //Creation edge collider
        int points_number = point_index;
        ramp_edge_collider = ramp.AddComponent<EdgeCollider2D>();
        ramp_edge_collider.edgeRadius = Constants.RAMP_EDGE_RADIUS;
        Vector2[] points = new Vector2[points_number];
        points[0] = Vector2.zero;

        for (int i = 1; i < points_number; i++)
        {
            Vector2 point = ramp_line_renderer.GetPosition(i);
            point.x = point.x - ramp.transform.position.x;
            point.y = point.y - ramp.transform.position.y;
            points[i] = point;
        }
        ramp_edge_collider.points = points;
        point_index = 0;
    }

    public void DisplayHideLevelManagerUI()
    {
        if (editing_mode == EditingMode.None)
        {
            editing = !editing;
            if (editing)
            {
                simulation_manager.enabled = false;
                ui_manager.DisplayHideSimulationUI(false);
                ui_manager.DisplayHideLevelManagerUI(true);
                return;
            }
            simulation_manager.enabled = true;
            ui_manager.DisplayHideSimulationUI(true);
            ui_manager.DisplayHideLevelManagerUI(false);
        }
    }

    public void DisplayHideMoveModeUI()
    {
        if (!editing)
            return;

        if (editing_mode == EditingMode.None)
        {
            editing_mode = EditingMode.MoveMode;
            ui_manager.DisplayHideLevelManagerUI(false);
            ui_manager.DisplayHideMoveUI(true);
        }
        else if (editing_mode == EditingMode.MoveMode)
        {
            editing_mode = EditingMode.None;
            ui_manager.DisplayHideLevelManagerUI(true);
            ui_manager.DisplayHideMoveUI(false);
        }
    }

    public void DisplayHideCreateEditSplineUI()
    {
        if (!editing)
            return;

        if(editing_mode==EditingMode.None)
        {
            editing_mode = EditingMode.CreateEditSplineMode;
            ui_manager.DisplayHideLevelManagerUI(false);
            ui_manager.DisplayHideCreateEditSplineUI(true);
        }
        else if(editing_mode == EditingMode.CreateEditSplineMode)
        {
            if (point_index > 0)
                BuildRampCollider();

            editing_mode = EditingMode.None;
            ui_manager.DisplayHideLevelManagerUI(true);
            ui_manager.DisplayHideCreateEditSplineUI(false);
        }
    }

    public void DisplayHideRotateModeUI()
    {
        if (!editing)
            return;

        if (editing_mode == EditingMode.None)
        {
            editing_mode = EditingMode.RotateMode;
            ui_manager.DisplayHideLevelManagerUI(false);
            ui_manager.DisplayHideRotateUI(true);

            GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
            //Enable all pivots
            for (int i = 0; i < interactables.Length; i++)
                interactables[i].transform.GetChild(Constants.ROTATION_PIVOT_INDEX).gameObject.SetActive(true);
        }
        else if (editing_mode == EditingMode.RotateMode)
        {
            editing_mode = EditingMode.None;
            ui_manager.DisplayHideLevelManagerUI(true);
            ui_manager.DisplayHideRotateUI(false);

            GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
            //Disable all pivots
            for (int i = 0; i < interactables.Length; i++)
                interactables[i].transform.GetChild(Constants.ROTATION_PIVOT_INDEX).gameObject.SetActive(false);
        }
    }

    public void DisplayHideDeleteModeUI()
    {
        if (!editing)
            return;

        if (editing_mode == EditingMode.None)
        {
            editing_mode = EditingMode.DeleteMode;
            ui_manager.DisplayHideLevelManagerUI(false);
            ui_manager.DisplayHideDeleteModeUI(true);
        }
        else if(editing_mode == EditingMode.DeleteMode)
        {
            editing_mode = EditingMode.None;
            ui_manager.DisplayHideLevelManagerUI(true);
            ui_manager.DisplayHideDeleteModeUI(false);
        }
    }
}
