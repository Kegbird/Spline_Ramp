using UnityEngine;
using Assets.Scripts;

public class LevelManager : MonoBehaviour
{
    enum EditingMode
    {
        MoveMode,
        RotateMode,
        DeleteMode,
        CreateRampMode,
        EditRampMode,
        None
    }

    [SerializeField]
    private SimulationManager simulation_manager;
    [SerializeField]
    private UIManager ui_manager;
    [SerializeField]
    private bool editing;
    [SerializeField]
    private EditingMode editing_mode;
    [SerializeField]
    private Vector3 translation_offset;
    [SerializeField]
    private Vector3 rotation_offset;
    [SerializeField]
    private Transform boundary;
    [SerializeField]
    private bool moving;
    [SerializeField]
    private bool rotating;
    [SerializeField]
    private bool editing_ramp;
    [SerializeField]
    private int node_index;
    [SerializeField]
    private bool mouse_over_ui = false;

    private void Awake()
    {
        editing_mode = EditingMode.None;
        simulation_manager = GetComponent<SimulationManager>();
        ui_manager = GetComponent<UIManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            SetUnsetEditingMode();
        if (Input.GetKeyDown(KeyCode.T))
            SetUnsetMoveMode();
        if (Input.GetKeyDown(KeyCode.D))
            SetUnsetDeleteMode();
        if (Input.GetKeyDown(KeyCode.R))
            SetUnsetRotateMode();
        if (Input.GetKeyDown(KeyCode.M))
            SetUnsetCreateRampMode();
        if (Input.GetKeyDown(KeyCode.P))
            SetUnsetEditRampMode();
        PerformEditMode();
    }

    private void PerformEditMode()
    {
        switch (editing_mode)
        {
            case EditingMode.None:
                break;
            case EditingMode.MoveMode:
                MoveObject();
                break;
            case EditingMode.RotateMode:
                RotateObject();
                break;
            case EditingMode.DeleteMode:
                DeleteObject();
                break;
            case EditingMode.CreateRampMode:
                CreateRamp();
                break;
            default:
                EditRamp();
                break;
        }
    }

    private void EditRamp()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << Constants.TRANSLATION_GIZMO_LAYER_MASK);
            if (hit.collider != null)
            {
                editing_ramp = true;
                node_index = hit.transform.GetSiblingIndex();
                translation_offset = GetMouseWorldCoordinates() - hit.transform.position;
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (editing_ramp)
            {
                Vector3 new_position = GetMouseWorldCoordinates() - translation_offset;
                new_position.x = Mathf.Clamp(new_position.x, Constants.MIN_X_BOUNDARY, boundary.position.x);
                Ramp.Edit(new_position, node_index);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            translation_offset = Vector3.zero;
            editing_ramp = false;
            node_index = 0;
        }
    }

    private void CreateRamp()
    {
        if (mouse_over_ui)
            return;

        Vector3 new_position;
        if (Input.GetMouseButtonDown(0))
        {
            new_position = GetMouseWorldCoordinates();
            new_position.x = Mathf.Clamp(new_position.x, Constants.MIN_X_BOUNDARY, boundary.position.x);
            Ramp.AddPoint(new_position);
        }
        if (Input.GetMouseButtonDown(1))
        {
            SetUnsetCreateRampMode();
            return;
        }

        new_position = GetMouseWorldCoordinates();
        new_position.x = Mathf.Clamp(new_position.x, Constants.MIN_X_BOUNDARY, boundary.position.x);
        Ramp.SetLastPoint(new_position);
    }

    private void MoveObject()
    {
        if (Ramp.Created())
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Raycast
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << Constants.RAMP_LAYER_MASK);
                if (hit.collider != null)
                {
                    translation_offset = GetMouseWorldCoordinates() - Ramp.GetWorldPosition();
                    moving = true;
                }
                return;
            }
            if (Input.GetMouseButton(0) && moving)
            {
                Vector3 new_position = GetMouseWorldCoordinates() - translation_offset;
                Ramp.Translate(new_position, Constants.MIN_X_BOUNDARY, boundary.transform.position.x);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                moving = false;
                translation_offset = Vector3.zero;
            }

        }
    }

    private void RotateObject()
    {
        if (Ramp.Created())
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << Constants.ROTATION_GIZMO_LAYER_MASK);
                if (hit.collider != null)
                    rotating = true;
                return;
            }

            if (Input.GetMouseButton(0) && rotating)
            {
                Ramp.Rotate(Input.mousePosition, Constants.MIN_X_BOUNDARY, boundary.transform.position.x);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                rotating = false;
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
            if (hit.collider != null && hit.collider.gameObject.tag == "Interactable")
                Destroy(hit.collider.gameObject);
        }
    }

    public void SetUnsetEditingMode()
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

    public void SetUnsetMoveMode()
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
            moving = false;
        }
    }

    public void SetUnsetEditRampMode()
    {
        if (!editing)
            return;

        if (editing_mode == EditingMode.None && Ramp.Created())
        {
            Ramp.DisplayNodeTranslationGizmo();
            editing_mode = EditingMode.EditRampMode;
            ui_manager.DisplayHideLevelManagerUI(false);
            ui_manager.DisplayHideEditRampUI(true);
        }
        else if (editing_mode == EditingMode.EditRampMode && Ramp.Created())
        {
            Ramp.HideNodeTranslationGizmo();
            editing_mode = EditingMode.None;
            ui_manager.DisplayHideLevelManagerUI(true);
            ui_manager.DisplayHideEditRampUI(false);
            mouse_over_ui = false;
            editing_ramp = false;
            translation_offset = Vector3.zero;
            node_index = 0;
        }
    }

    public void SetUnsetCreateRampMode()
    {
        if (!editing)
            return;

        if (editing_mode == EditingMode.None)
        {
            editing_mode = EditingMode.CreateRampMode;
            ui_manager.DisplayHideLevelManagerUI(false);
            ui_manager.DisplayHideCreateRampUI(true);
        }
        else if (editing_mode == EditingMode.CreateRampMode)
        {
            mouse_over_ui = false;
            editing_mode = EditingMode.None;
            ui_manager.DisplayHideLevelManagerUI(true);
            ui_manager.DisplayHideCreateRampUI(false);
            Ramp.BuildEdgeCollider();
        }
    }

    public void SetUnsetRotateMode()
    {
        if (!editing)
            return;

        if (editing_mode == EditingMode.None && Ramp.Created())
        {
            editing_mode = EditingMode.RotateMode;
            ui_manager.DisplayHideLevelManagerUI(false);
            ui_manager.DisplayHideRotateUI(true);
            Ramp.DisplayRotationGizmo();
        }
        else if (editing_mode == EditingMode.RotateMode && Ramp.Created())
        {
            editing_mode = EditingMode.None;
            ui_manager.DisplayHideLevelManagerUI(true);
            ui_manager.DisplayHideRotateUI(false);
            Ramp.HideRotationGizmo();
            rotating = false;
        }
    }

    public void SetUnsetDeleteMode()
    {
        if (!editing)
            return;

        if (editing_mode == EditingMode.None)
        {
            editing_mode = EditingMode.DeleteMode;
            ui_manager.DisplayHideLevelManagerUI(false);
            ui_manager.DisplayHideDeleteModeUI(true);
        }
        else if (editing_mode == EditingMode.DeleteMode)
        {
            editing_mode = EditingMode.None;
            ui_manager.DisplayHideLevelManagerUI(true);
            ui_manager.DisplayHideDeleteModeUI(false);
        }
    }

    public void UpdateMouseOverUI()
    {
        mouse_over_ui = !mouse_over_ui;
    }

    private Vector3 GetMouseWorldCoordinates()
    {
        Vector3 position = Input.mousePosition;
        position = Camera.main.ScreenToWorldPoint(position);
        position.z = 0;
        return position;
    }
}
