using UnityEngine;

public class SmallerWindow : MonoBehaviour
{
    [Header("Grid Body Window")]
    public GridBodyWindowManager grid_body_window;
    public EditorButtonsHandler editor_buttons_handler;
    public GameObject window;
    public GameObject help_window;

    public void Show()
    {
        // we freeze the game
        grid_body_window.Standby();

        // we deactivate the buttons
        editor_buttons_handler.DeactivateButtons();

        // we hide the help window
        help_window.SetActive(false);

        // we show the window
        window.SetActive(true);
    }

    public void Hide()
    {
        // we activate the buttons
        editor_buttons_handler.ActivateButtons();

        // we unfreeze the game
        window.SetActive(false);

        // we show the help window
        help_window.SetActive(true);

        // we hide the window
        grid_body_window.Standby(false);
    }
}
