using System.Collections;
using System.Collections.Generic;
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
                break;
        }
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
                grabbed_gameobject.transform.position = GetMouseWorldCoordinates() - translation_offset;
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
                grabbed_gameobject = null;
        }
    }

    private float AngleBetweenTwoPoints()
    {
        return 0;
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
        else
        {
            editing_mode = EditingMode.None;
            ui_manager.DisplayHideLevelManagerUI(true);
            ui_manager.DisplayHideDeleteModeUI(false);
        }
    }
}
