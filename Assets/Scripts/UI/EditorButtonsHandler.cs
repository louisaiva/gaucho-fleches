using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EditorButtonsHandler : MonoBehaviour
{
    [Header("Buttons")]
    public List<Button> buttons;

    // START
    void Start()
    {
        // we get the buttons
        buttons = new List<Button>(GetComponentsInChildren<Button>());
    }

    // ACTIVATE & DEACTIVATE BUTTONS
    public void ActivateButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    public void DeactivateButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }
}