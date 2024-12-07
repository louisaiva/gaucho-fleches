using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{

    [Header("Toggle Button")]
    public bool toggled = false;
    public bool interactable = true;
    public bool hovered = false;

    // get on -> return true if toggled
    public bool On { get { return toggled; } }
    public bool Off { get { return !toggled; } }

    [Header("Components")]
    public Graphic graphic;

    [Header("Colors")]
    public Color on_color = Color.green;
    public Color on_hover_color = Color.green;
    public Color off_color = Color.red;
    public Color off_hover_color = Color.red;

    // START
    void Start()
    {
        // we get the graphic
        graphic = GetComponent<Graphic>();
    }

    // UPDATE
    void Update()
    {
        // we check if we are interactable
        if (!interactable) { return; }

        // we check if we are hovered
        hovered = RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, Input.mousePosition);

        // we check if we clicked
        if (Input.GetMouseButtonDown(0) && hovered)
        {
            Toggle();
        }

        // we change the color
        if (toggled)
        {
            graphic.color = hovered ? on_hover_color : on_color;
        }
        else
        {
            graphic.color = hovered ? off_hover_color : off_color;
        }
    }

    // TOGGLE
    public void Toggle()
    {
        if (interactable)
        {
            toggled = !toggled;
        }
    }

}