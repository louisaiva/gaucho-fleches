using UnityEngine;

public class ClickButton : HoverButton
{

    [Header("Click Button")]
    public bool clicked = false;
    public Color clicked_color = Color.red;

    [Header("Event")]
    public UnityEngine.Events.UnityEvent OnClick;


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
            if (clicked) { OnClickEvent(); }
            clicked = false;
        }

        UpdateColors();
    }

    protected override void UpdateColors()
    {
        base.UpdateColors();
        
        if (clicked)
        {
            graphic.color = clicked_color;
        }
    }

    // ON CLICK
    public void OnClickEvent()
    {
        OnClick.Invoke();

        // we reset the clicked state
        clicked = false;
        hovered = false;
        UpdateCursor();
        UpdateColors();
    }

}