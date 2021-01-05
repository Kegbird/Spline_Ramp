using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Ramps;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject edit_level_btn;
    [SerializeField]
    private GameObject create_ramp_btn;
    [SerializeField]
    private GameObject rotate_btn;
    [SerializeField]
    private GameObject delete_btn;
    [SerializeField]
    private GameObject move_btn;
    [SerializeField]
    private GameObject edit_btn;
    [SerializeField]
    private GameObject start_stop_simulation_btn;
    [SerializeField]
    private GameObject move_hint_text;
    [SerializeField]
    private GameObject exit_move_mode_btn;
    [SerializeField]
    private GameObject rotate_hint_text;
    [SerializeField]
    private GameObject exit_rotate_mode_btn;
    [SerializeField]
    private GameObject create_ramp_hint_text;
    [SerializeField]
    private GameObject ramp_type_dropdown;
    [SerializeField]
    private GameObject exit_create_ramp_btn;
    [SerializeField]
    private GameObject edit_ramp_hint_text;
    [SerializeField]
    private GameObject exit_edit_ramp_btn;

    public void DisplayHideLevelManagerUI(bool display)
    {
        if (display)
            edit_level_btn.GetComponentInChildren<Text>().text = "End edit (E)";
        else
            edit_level_btn.GetComponentInChildren<Text>().text = "Edit level (E)";
        create_ramp_btn.SetActive(display);
        rotate_btn.SetActive(display);
        delete_btn.SetActive(display);
        move_btn.SetActive(display);
        edit_btn.SetActive(display);
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

    public void DisplayHideRotateUI(bool display)
    {
        edit_level_btn.SetActive(!display);
        rotate_hint_text.SetActive(display);
        exit_rotate_mode_btn.SetActive(display);
    }

    public void DisplayHideCreateRampUI(bool display)
    {
        edit_level_btn.SetActive(!display);
        exit_create_ramp_btn.SetActive(display);
        create_ramp_hint_text.SetActive(display);
        ramp_type_dropdown.SetActive(display);
        ramp_type_dropdown.GetComponent<Dropdown>().value = 0;
    }

    public void DisplayHideEditRampUI(bool display)
    {
        edit_level_btn.SetActive(!display);
        exit_edit_ramp_btn.SetActive(display);
        edit_ramp_hint_text.SetActive(display);
    }

    public void EnableDisableInteractionRampTypeDropdown(bool value)
    {
        ramp_type_dropdown.GetComponent<Dropdown>().interactable = value;
    }

    public void EnableDisableInteractionCreateRampBtn(bool value)
    {
        create_ramp_btn.GetComponent<Button>().interactable = value;
    }

    public void EnableDisableInteractionMoveBtn(bool value)
    {
        move_btn.GetComponent<Button>().interactable = value;
    }

    public void EnableDisableInteractionRotateBtn(bool value)
    {
        rotate_btn.GetComponent<Button>().interactable = value;
    }

    public void EnableDisableInteractionEditBtn(bool value)
    {
        edit_btn.GetComponent<Button>().interactable = value;
    }

    public void EnableDisableInteractionMenuEditBtn(bool value)
    {
        if (value)
        {
            create_ramp_btn.GetComponent<Button>().interactable = false;
            move_btn.GetComponent<Button>().interactable = true;
            rotate_btn.GetComponent<Button>().interactable = true;
            edit_btn.GetComponent<Button>().interactable = true;
        }
        else
        {
            create_ramp_btn.GetComponent<Button>().interactable = true;
            move_btn.GetComponent<Button>().interactable = false;
            rotate_btn.GetComponent<Button>().interactable = false;
            edit_btn.GetComponent<Button>().interactable = false;
        }
        EnableDisableInteractionRampTypeDropdown(!value);
    }

    public RampType GetRampType()
    {
        Dropdown dropdown = ramp_type_dropdown.GetComponent<Dropdown>();
        return (RampType)dropdown.value;
    }
}
