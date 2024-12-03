using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{

    [Header("Cell settings")]
    public int x;
    public int y;
    public bool hovered = false;
    public bool selected = false;

    [Header("Lines")]
    public LinesHandler lines;

    [Header("Graphics & Colors")]
    public Image image;
    // public 
    public Color color_normal;
    public Color color_hover;
    public Color color_selected;

    [Header("Navigation")]
    protected CaseNavigator navigator;

    // START
    protected virtual void Start()
    {
        // we get the components
        image = GetComponent<Image>();
        navigator = transform.parent.GetComponent<CaseNavigator>();

        // we get the lines
        lines = transform.Find("lines").GetComponent<LinesHandler>();
    }

    // UPDATE
    protected virtual void Update()
    {
        // we first check if mouse is over the case
        bool mouse_over = RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition);

        // we check the hover state
        if (mouse_over && !hovered) { hovered = true; } // we enter the case
        else if (!mouse_over && hovered) { hovered = false; } // we leave the case

        if (hovered && !selected && Input.GetMouseButtonDown(0)) { Select(); } // we click on the case
        else if (hovered && Input.GetMouseButtonDown(1)) { RightClick(); } // we right click on the case
        else if (!hovered && selected && Input.GetMouseButtonDown(0)) { UnSelect(); } // we click outside the case

        // we change the color with the state
        if (selected) { image.color = color_selected; }
        else if (hovered) { image.color = color_hover; }
        else { image.color = color_normal; }

        // we navigate if the case is selected
        if (selected && navigator != null)
        {
            navigator.UpdateNavigation(this);
        }
    }
    public void ForceUpdate()
    {
        // we force the update of the case
        Update();
    }

    // PUBLIC METHODS
    public virtual void Select()
    {
        selected = true;
    }
    public virtual void UnSelect()
    {
        selected = false;
    }

    public virtual void RightClick()
    {
        // we switch case / def
        transform.parent.GetComponent<GridHandler>().SwitchCaseDef(this);
    }

    public virtual void ResetLine(bool right= true)
    {
        // we reset the line
        lines.ResetLine(right);
    }

    // GETTERS
    public virtual string GetContent()
    {
        return "";
    }
    public virtual void SetContent(string content)
    {
        // nothing
    }

    public virtual Cell GetRightCell()
    {
        // we get the right cell
        return transform.parent.GetComponent<GridHandler>().GetCell(x + 1, y);
    }
    public virtual Cell GetLeftCell()
    {
        // we get the left cell
        return transform.parent.GetComponent<GridHandler>().GetCell(x - 1, y);
    }
    public virtual Cell GetUpCell()
    {
        // we get the up cell
        return transform.parent.GetComponent<GridHandler>().GetCell(x, y - 1);
    }
    public virtual Cell GetDownCell()
    {
        // we get the down cell
        return transform.parent.GetComponent<GridHandler>().GetCell(x, y + 1);
    }

    // SETTERS
    public virtual void SetGridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}