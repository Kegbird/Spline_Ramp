﻿using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.Ramps;


namespace Assets.Scripts.Levels
{
    public class LevelEdit : MonoBehaviour
    {
        enum EditingMode
        {
            MoveMode,
            RotateMode,
            CreateRampMode,
            EditRampMode,
            None
        }

        [SerializeField]
        private Simulation simulation;
        [SerializeField]
        private UIEvent ui_event;
        [SerializeField]
        private bool editing;
        [SerializeField]
        private EditingMode editing_mode;
        [SerializeField]
        private Vector3 translation_offset;
        [SerializeField]
        private Vector3 rotation_offset;
        [SerializeField]
        private bool moving_ramp;
        [SerializeField]
        private bool rotating_ramp;
        [SerializeField]
        private bool editing_ramp;
        [SerializeField]
        private int node_index;
        [SerializeField]
        private bool mouse_over_ui = false;
        [SerializeField]
        private Ramp ramp;


        private void Awake()
        {
            editing_mode = EditingMode.None;
            simulation = GetComponent<Simulation>();
            ui_event = GetComponent<UIEvent>();
        }

        private void Update()
        {
            if (simulation.IsSimulationRunning())
                return;

            if (Input.GetKeyDown(KeyCode.E))
                SetUnsetEditingMode();
            if (Input.GetKeyDown(KeyCode.T))
                SetUnsetMoveMode();
            if (Input.GetKeyDown(KeyCode.D))
                DeleteRamp();
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
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << Constants.SUPPORT_NODE_LAYER_MASK);
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
                    ramp.Edit(new_position, node_index);
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
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                    return;

                new_position = GetMouseWorldCoordinates();
                if (!ValidCoordinates(new_position))
                    return;
                if (ramp == null)
                {
                    switch (ui_event.GetRampType())
                    {
                        case RampType.Segment:
                            ramp = new SegmentRamp(new_position);
                            break;
                        case RampType.Bezier:
                            ramp = new BezierRamp(new_position);
                            break;
                        case RampType.Hermite:
                            ramp = new HermiteRamp(new_position);
                            break;
                        default:
                            ramp = new BSplineRamp(new_position);
                            break;
                    }
                    ui_event.EnableDisableInteractionRampTypeDropdown(false);
                }
                ramp.AddPoint(new_position);
            }
            if (Input.GetMouseButtonDown(1))
            {
                SetUnsetCreateRampMode();
                return;
            }
        }

        private void MoveObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Raycast
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << Constants.RAMP_LAYER_MASK);
                if (hit.collider != null)
                {
                    translation_offset = GetMouseWorldCoordinates() - ramp.GetWorldPosition();
                    moving_ramp = true;
                }
                return;
            }
            if (Input.GetMouseButton(0) && moving_ramp)
            {
                Vector3 new_position = GetMouseWorldCoordinates() - translation_offset;
                ramp.Translate(new_position);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                moving_ramp = false;
                translation_offset = Vector3.zero;
            }
        }

        private void RotateObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << Constants.ROTATION_GIZMO_LAYER_MASK);
                if (hit.collider != null)
                    rotating_ramp = true;
                return;
            }

            if (Input.GetMouseButton(0) && rotating_ramp)
            {
                ramp.Rotate(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                rotating_ramp = false;
                rotation_offset = Vector3.zero;
            }
        }

        public void DeleteRamp()
        {
            if (editing && editing_mode == EditingMode.None && ramp != null)
            {
                ramp.DestroyRamp();
                ramp = null;
                ui_event.EnableDisableInteractionMenuEditBtn(false);
                ui_event.EnableDisableInteractionCreateRampBtn(true);
                ui_event.EnableDisableInteractionMoveBtn(false);
                ui_event.EnableDisableInteractionRotateBtn(false);
                ui_event.EnableDisableInteractionEditBtn(false);
            }
        }

        public void SetUnsetEditingMode()
        {
            if (editing_mode == EditingMode.None)
            {
                editing = !editing;
                if (editing)
                {
                    simulation.enabled = false;
                    ui_event.DisplayHideSimulation(false);
                    ui_event.DisplayHideLevelManager(true);
                    return;
                }
                simulation.enabled = true;
                ui_event.DisplayHideSimulation(true);
                ui_event.DisplayHideLevelManager(false);
            }
        }

        public void SetUnsetMoveMode()
        {
            if (!editing || ramp == null)
                return;

            if (editing_mode == EditingMode.None)
            {
                editing_mode = EditingMode.MoveMode;
                ui_event.DisplayHideLevelManager(false);
                ui_event.DisplayHideMove(true);
            }
            else if (editing_mode == EditingMode.MoveMode)
            {
                editing_mode = EditingMode.None;
                ui_event.DisplayHideLevelManager(true);
                ui_event.DisplayHideMove(false);
                moving_ramp = false;
            }
        }

        public void SetUnsetEditRampMode()
        {
            if (!editing || ramp == null)
                return;

            if (editing_mode == EditingMode.None)
            {
                editing_mode = EditingMode.EditRampMode;
                ui_event.DisplayHideLevelManager(false);
                ui_event.DisplayHideEditRamp(true);
            }
            else if (editing_mode == EditingMode.EditRampMode)
            {
                editing_mode = EditingMode.None;
                ui_event.DisplayHideLevelManager(true);
                ui_event.DisplayHideEditRamp(false);
                mouse_over_ui = false;
                editing_ramp = false;
                translation_offset = Vector3.zero;
                node_index = 0;
                StartCoroutine(ramp.BuildRamp());
            }
        }

        public void SetUnsetCreateRampMode()
        {
            if (!editing)
                return;

            ui_event.EnableDisableInteractionMenuEditBtn(ramp != null);
            if (editing_mode == EditingMode.None && ramp == null)
            {
                editing_mode = EditingMode.CreateRampMode;
                ui_event.DisplayHideLevelManager(false);
                ui_event.DisplayHideCreateRamp(true);
            }
            else if (editing_mode == EditingMode.CreateRampMode)
            {
                if (ramp != null)
                    StartCoroutine(ramp.BuildRamp());
                mouse_over_ui = false;
                editing_mode = EditingMode.None;
                ui_event.DisplayHideLevelManager(true);
                ui_event.DisplayHideCreateRamp(false);
            }
        }

        public void SetUnsetRotateMode()
        {
            if (!editing || ramp == null)
                return;

            if (editing_mode == EditingMode.None)
            {
                editing_mode = EditingMode.RotateMode;
                ui_event.DisplayHideLevelManager(false);
                ui_event.DisplayHideRotate(true);
                ramp.DisplayRotationGizmo();
            }
            else if (editing_mode == EditingMode.RotateMode)
            {
                editing_mode = EditingMode.None;
                ui_event.DisplayHideLevelManager(true);
                ui_event.DisplayHideRotate(false);
                ramp.HideRotationGizmo();
                rotating_ramp = false;
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

        private bool ValidCoordinates(Vector3 coordinates)
        {
            return coordinates.x < Constants.MAX_X_BOUNDARY;
        }
    }
}