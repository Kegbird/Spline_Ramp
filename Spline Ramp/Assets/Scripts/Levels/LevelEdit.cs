using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.Ramps;
using System.Collections.Generic;

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
        AudioManager audio_manager;
        [SerializeField]
        private Simulation simulation;
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
            audio_manager = GetComponent<AudioManager>();
            simulation = GetComponent<Simulation>();
            ui_manager = GetComponent<UIManager>();
        }

        private void Update()
        {
            if (simulation.IsSimulationRunning())
                return;

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
                    switch (ui_manager.GetRampType())
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
                    ui_manager.EnableDisableInteractionRampTypeDropdown(false);
                }
                ramp.AddPoint(new_position);
                audio_manager.TapSound();
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
            audio_manager.PlayButtonPressedSound();
            if (editing && editing_mode == EditingMode.None && ramp != null)
            {
                ramp.DestroyRamp();
                ramp = null;
                ui_manager.EnableDisableInteractionMenuEditBtn(false);
                ui_manager.EnableDisableInteractionCreateRampBtn(true);
                ui_manager.EnableDisableInteractionMoveBtn(false);
                ui_manager.EnableDisableInteractionDeleteBtn(false);
                ui_manager.EnableDisableInteractionRotateBtn(false);
                ui_manager.EnableDisableInteractionEditBtn(false);
            }
        }

        public void SetUnsetEditingMode()
        {
            audio_manager.PlayButtonPressedSound();
            if (editing_mode == EditingMode.None)
            {
                editing = !editing;
                if (editing)
                {
                    simulation.enabled = false;
                    ui_manager.DisplayHideSimulation(false);
                    ui_manager.DisplayHideLevelManager(true);
                    return;
                }
                simulation.enabled = true;
                ui_manager.DisplayHideSimulation(true);
                ui_manager.DisplayHideLevelManager(false);
            }
        }

        public void SetUnsetMoveMode()
        {
            if (!editing || ramp == null)
                return;
            audio_manager.PlayButtonPressedSound();
            if (editing_mode == EditingMode.None)
            {
                editing_mode = EditingMode.MoveMode;
                ui_manager.DisplayHideLevelManager(false);
                ui_manager.DisplayHideMove(true);
            }
            else if (editing_mode == EditingMode.MoveMode)
            {
                audio_manager.PlayConfirmSound();
                editing_mode = EditingMode.None;
                ui_manager.DisplayHideLevelManager(true);
                ui_manager.DisplayHideMove(false);
                moving_ramp = false;
            }
        }

        public void SetUnsetEditRampMode()
        {
            if (!editing || ramp == null)
                return;
            audio_manager.PlayButtonPressedSound();
            if (editing_mode == EditingMode.None)
            {
                editing_mode = EditingMode.EditRampMode;
                ui_manager.DisplayHideLevelManager(false);
                ui_manager.DisplayHideEditRamp(true);
                ramp.AllowEdit();
            }
            else if (editing_mode == EditingMode.EditRampMode)
            {
                audio_manager.PlayConfirmSound();
                editing_mode = EditingMode.None;
                ui_manager.DisplayHideLevelManager(true);
                ui_manager.DisplayHideEditRamp(false);
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
            audio_manager.PlayButtonPressedSound();
            ui_manager.EnableDisableInteractionMenuEditBtn(ramp != null);
            if (editing_mode == EditingMode.None && ramp == null)
            {
                editing_mode = EditingMode.CreateRampMode;
                ui_manager.DisplayHideLevelManager(false);
                ui_manager.DisplayHideCreateRamp(true);
            }
            else if (editing_mode == EditingMode.CreateRampMode)
            {
                audio_manager.PlayConfirmSound();
                if (ramp != null)
                    StartCoroutine(ramp.BuildRamp());
                mouse_over_ui = false;
                editing_mode = EditingMode.None;
                ui_manager.DisplayHideLevelManager(true);
                ui_manager.DisplayHideCreateRamp(false);
            }
        }

        public void SetUnsetRotateMode()
        {
            if (!editing || ramp == null)
                return;
            audio_manager.PlayButtonPressedSound();
            if (editing_mode == EditingMode.None)
            {
                editing_mode = EditingMode.RotateMode;
                ui_manager.DisplayHideLevelManager(false);
                ui_manager.DisplayHideRotate(true);
                ramp.DisplayRotationGizmo();
            }
            else if (editing_mode == EditingMode.RotateMode)
            {
                audio_manager.PlayConfirmSound();
                editing_mode = EditingMode.None;
                ui_manager.DisplayHideLevelManager(true);
                ui_manager.DisplayHideRotate(false);
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
