using UnityEngine;
using UnityEngine.UI;

public abstract class HoverButton : MonoBehaviour
{

    [Header("Base Button")]
    public bool interactable = true;
    public bool hovered = false;

    [Header("Components")]
    public Graphic graphic;
    public RectTransform rect_to_click;
    public CursorHandler cursor;

    [Header("Colors")]
    public Color base_color = Color.red;
    public Color hover_color = Color.red;
    public Color disabled_color = Color.gray;


    // START
    void Start()
    {
        // we get the graphic
        if (graphic == null)
        {
            graphic = GetComponent<Graphic>();
        }
        
        // we get the rect to click
        if (rect_to_click == null)
        {
            rect_to_click = (RectTransform)transform;
        }

        // we get the cursor
        cursor = GameObject.Find("ui").GetComponent<CursorHandler>();
    }


    // UPDATE
    protected virtual void Update()
    {
        // ! this is the base button, it should be inherited
        // ! we should not use it directly

        // we check if we are interactable
        if (!interactable)
        {
            graphic.color = disabled_color;
            return;
        }

        // we check if we are hovered
        UpdateHover();

        UpdateColors();
    }
    protected virtual void UpdateColors()
    {
        if (!interactable)
        {
            graphic.color = disabled_color;
            return;
        }

        // we change the color
        if (hovered)
        {
            graphic.color = hover_color;
        }
        else
        {
            graphic.color = base_color;
        }
    }

    // HOVER
    public void Hover()
    {
        hovered = true;
        cursor.AddCursorDemand(gameObject, "hand", 1);
    }
    public void UnHover()
    {
        hovered = false;
        cursor.RemoveCursorDemand(gameObject);
    }
    public void UpdateHover()
    {
        bool mouse_in_rect = RectTransformUtility.RectangleContainsScreenPoint(rect_to_click, Input.mousePosition);
        if (mouse_in_rect && !hovered)
        {
            Hover();
        }
        else if (!mouse_in_rect && hovered)
        {
            UnHover();
        }
    }
}