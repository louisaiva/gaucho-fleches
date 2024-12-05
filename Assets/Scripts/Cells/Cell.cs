using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{

    [Header("Cell settings")]
    public int x;
    public int y;
    public bool hovered = false;
    public bool selected = false;
    public bool stanby = false;
    public GridHandler grid;

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

        // we get the grid
        grid = transform.parent.GetComponent<GridHandler>();
    }

    // UPDATE
    protected virtual void Update()
    {
        // we first check if we aren't pressing alt or if we are in static mode
        if (Input.GetKey(grid.GetComponentInParent<GridBodyWindowManager>().move_key) || stanby)
        {
            // we set the hover to false
            hovered = false;
            // selected = false;
            if (stanby) { selected = false; }
        }
        else
        {
            // we then check if mouse is over the case
            bool mouse_over = RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition);

            // we check the hover state
            if (mouse_over && !hovered) { hovered = true; } // we enter the case
            else if (!mouse_over && hovered) { hovered = false; } // we leave the case

            // we check the selection state
            if (hovered && !selected && Input.GetMouseButtonDown(0)) { Select(); } // we click on the case
            else if (hovered && Input.GetMouseButtonDown(1)) { RightClick(); } // we right click on the case
            else if (!hovered && selected && Input.GetMouseButtonDown(0)) { UnSelect(); } // we click outside the case
        }

        // we change the color with the state
        if (selected) { image.color = color_selected; }
        else if (hovered) { image.color = color_hover; }
        else { image.color = color_normal; }

        // we navigate if the case is selected
        if (!stanby && selected && navigator != null)
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

    // LINE MANAGEMENT
    public virtual void ActivateLeftUpLinesIfOnSide()
    {
        // we check that lines is not null
        if (lines == null) { Start(); }

        // we check if x=0
        if (x == 0) { lines.ActivateLeftLine(); }

        // we check if y=0
        if (y == 0) { lines.ActivateUpLine(); }
    }
    public virtual void ResetLine(bool right= true)
    {
        // we get the line
        if (lines == null) { Start(); }

        // we reset the line
        lines.ResetLine(right);
    }
    public virtual void ExpandLine(bool right = true)
    {
        // we check if we are well started
        if (lines == null) { Start(); }

        // we check if we can expand the line
        Cell target = right ? GetRightCell() : GetDownCell();
        if (target == null || !(target is Case)) { return; }

        // we get the line
        RectTransform line = right ? lines.line_right : lines.line_down;

        // we expand the line
        lines.ExpandLine(line);
    }
    public virtual string GetExpandedLines()
    {
        string expanded = "";

        // check the right line
        expanded += lines.expanded_right ? "1" : "0";
        expanded += ".";
        expanded += lines.expanded_down ? "1" : "0";

        return expanded;
    }
    public virtual void SetExpandedLines(string expanded)
    {
        // check the type of the cell
        if (this is not Case) { return; }

        // we split the string
        string[] parts = expanded.Split('.');
        if (parts.Length != 2) { return; }

        // we expand the lines
        if (parts[0] == "1") { ExpandLine(true); }
        if (parts[1] == "1") { ExpandLine(false); }
        // lines.ExpandLine(parts[0] == "1" ? lines.line_right : null);
        // lines.ExpandLine(parts[1] == "1" ? lines.line_down : null);
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

    // CELL GETTERS
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

        // we update the lines if we are not definition
        if (this is not Definition)
        {
            ActivateLeftUpLinesIfOnSide();
        }
    }
}