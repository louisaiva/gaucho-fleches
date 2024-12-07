using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : HoverButton
{

    [Header("Toggle Button")]
    public bool toggled = false;
    private bool clicked = false;

    // get on -> return true if toggled
    public bool On { get { return toggled; } }
    public bool Off { get { return !toggled; } }

    [Header("Colors")]
    public Color toggled_color = Color.green;
    public Color toggled_hover_color = Color.green;

    [Header("Event")]
    public UnityEngine.Events.UnityEvent OnToggle;

    // UPDATE
    protected override void Update()
    {
        // we check if we are interactable
        if (!interactable) { return; }

        // we check if we are hovered
        UpdateHover();

        // we check if we clicked
        if (Input.GetMouseButtonDown(0) && hovered)
        {
            clicked = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (clicked) { Toggle(); }
            clicked = false;
        }

        UpdateColors();
    }

    protected override void UpdateColors()
    {
        base.UpdateColors();
        if (!interactable) { return; }

        if (toggled && hovered)
        {
            graphic.color = toggled_hover_color;
        }
        else if (toggled)
        {
            graphic.color = toggled_color;
        }
    }

    // TOGGLE
    public void Toggle()
    {
        toggled = !toggled;
        OnToggle.Invoke();
        UpdateColors();
    }

}