using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject edit_level_btn;
    [SerializeField]
    private GameObject create_edit_spline_btn;
    [SerializeField]
    private GameObject rotate_btn;
    [SerializeField]
    private GameObject delete_btn;
    [SerializeField]
    private GameObject move_btn;
    [SerializeField]
    private GameObject start_stop_simulation_btn;
    [SerializeField]
    private GameObject move_hint_text;
    [SerializeField]
    private GameObject exit_move_mode_btn;
    [SerializeField]
    private GameObject delete_hint_text;
    [SerializeField]
    private GameObject exit_delete_mode_btn;
    [SerializeField]
    private GameObject rotate_hint_text;
    [SerializeField]
    private GameObject exit_rotate_mode_btn;
    [SerializeField]
    private GameObject create_edit_spline_hint_text;
    [SerializeField]
    private GameObject exit_create_edit_spline_btn;

    public void DisplayHideLevelManagerUI(bool display)
    {
        if (display)
            edit_level_btn.GetComponentInChildren<Text>().text = "End edit (E)";
        else
            edit_level_btn.GetComponentInChildren<Text>().text = "Edit level (E)";
        create_edit_spline_btn.SetActive(display);
        rotate_btn.SetActive(display);
        delete_btn.SetActive(display);
        move_btn.SetActive(display);
    }

    public void DisplayHideEditLevelBtn(bool display)
    {
        edit_level_btn.SetActive(display);
    }

    public void ChangeSimulationButtonText(string text)
    {
        start_stop_simulation_btn.GetComponentInChildren<Text>().text = text;
    }

    public void DisplayHideSimulationUI(bool display)
    {
        start_stop_simulation_btn.SetActive(display);
    }

    public void DisplayHideMoveUI(bool display)
    {
        edit_level_btn.SetActive(!display);
        move_hint_text.SetActive(display);
        exit_move_mode_btn.SetActive(display);
    }

    public void DisplayHideDeleteModeUI(bool display)
    {
        edit_level_btn.SetActive(!display);
        delete_hint_text.SetActive(display);
        exit_delete_mode_btn.SetActive(display);
    }

    public void DisplayHideRotateUI(bool display)
    {
        edit_level_btn.SetActive(!display);
        rotate_hint_text.SetActive(display);
        exit_rotate_mode_btn.SetActive(display);
    }

    public void DisplayHideCreateEditSplineUI(bool display)
    {
        edit_level_btn.SetActive(!display);
        exit_create_edit_spline_btn.SetActive(display);
        create_edit_spline_hint_text.SetActive(display);
    }
}
