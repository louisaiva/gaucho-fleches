using UnityEngine;
using UnityEngine.UI;

public class LinesHandler : MonoBehaviour
{
    [Header("Lines settings")]
    public float line_width = 1f;
    public bool auto_line_width = true;
    public float wide_line_width = 2f;

    [Header("Hover settings")]
    public Color line_color = Color.black;
    public Color hover_color = Color.red;
    public bool hover_right = false;
    public bool hover_down = false;
    public bool expanded_right = false;
    public bool expanded_down = false;

    [Header("Components & References")]
    public RectTransform line_right;
    public RectTransform line_down;
    public GridLayoutGroup grid;
    public Cell cell;

    // START
    void Start()
    {
        // we get the components
        line_right = transform.Find("right").GetComponent<RectTransform>();
        line_down = transform.Find("down").GetComponent<RectTransform>();

        // we set the line colors
        line_right.GetComponent<Image>().color = line_color;
        line_down.GetComponent<Image>().color = line_color;

        // we get the grid
        grid = transform.parent.parent.GetComponent<GridLayoutGroup>();

        // we get the cell
        cell = transform.parent.GetComponent<Cell>();
    }


    // UPDATE
    void Update()
    {

        // update the line width
        if (auto_line_width) {UpdateLineWidth();}

        // we check if the Cell is a MotherCell
        if (cell is MotherCell) {return;}

        // we update the lines
        // we check if the mouse is over one of the lines
        bool mouse_over_right = RectTransformUtility.RectangleContainsScreenPoint(line_right, Input.mousePosition);
        bool mouse_over_down = RectTransformUtility.RectangleContainsScreenPoint(line_down, Input.mousePosition);

        // we get the cell on the right
        Cell right_cell = cell.GetRightCell();
        if (right_cell != null && right_cell is Case)
        {
            // we check the hover state
            if (mouse_over_right && !hover_right) { EnterHover(line_right); } // we enter the line
            else if (!mouse_over_right && hover_right) { ExitHover(line_right); } // we leave the line

            // we check the expanded state
            if (hover_right && Input.GetMouseButtonDown(0))
            {
                if (!expanded_right) { ExpandLine(line_right); }
                else { TrimLine(line_right); }
            }
        }

        // we get the cell below
        Cell down_cell = cell.GetDownCell();
        if (down_cell != null && down_cell is Case)
        {
            // we do the same for the down line
            if (mouse_over_down && !hover_down) { EnterHover(line_down); }
            else if (!mouse_over_down && hover_down) { ExitHover(line_down); }

            // we check the expanded state
            if (hover_down && Input.GetMouseButtonDown(0))
            {
                if (!expanded_down) { ExpandLine(line_down); }
                else { TrimLine(line_down); }
            }
        }
    }
    void UpdateLineWidth()
    {
        if (!grid) { Start(); }

        // we get the cell size
        if (line_width != grid.spacing.y)
        {
            // we update the line width
            line_width = grid.spacing.y;
            if (!expanded_right) {line_right.sizeDelta = new Vector2(line_right.sizeDelta.x, line_width);}
            if (!expanded_down ) {line_down.sizeDelta  = new Vector2(line_down.sizeDelta.x , line_width);}
        }
    }


    // PUBLIC METHODS
    public void ResetLine(bool right=true)
    {
        // we reset the line
        if (right && expanded_right) {TrimLine(line_right);}
        else if (!right && expanded_down) {TrimLine(line_down);}
    }


    // LINE MANAGEMENT
    private void EnterHover(RectTransform line)
    {
        // we enter the hover state
        line.GetComponent<Image>().color = hover_color;
        Debug.Log("Hovering " + line.name);

        if (line == line_right) {hover_right = true;}
        else if (line == line_down) {hover_down = true;}
    }
    private void ExitHover(RectTransform line)
    {
        // we exit the hover state
        line.GetComponent<Image>().color = line_color;

        if (line == line_right) { hover_right = false; }
        else if (line == line_down) { hover_down = false; }
    }

    private void ExpandLine(RectTransform line)
    {
        Vector2 size = line.sizeDelta;

        // we check which line we expand
        if (line == line_right)
        {
            // the line is vertical -> we change the width
            size.x = wide_line_width;
            expanded_right = true;
        }
        else if (line == line_down)
        {
            // the line is horizontal -> we change the height
            size.y = wide_line_width;
            expanded_down = true;
        }

        // we expand the line
        line.sizeDelta = size;
    }
    private void TrimLine(RectTransform line)
    {
        Vector2 size = line.sizeDelta;

        // we check which line we trim
        if (line == line_right)
        {
            // the line is vertical -> we change the width
            size.x = line_width;
            expanded_right = false;
        }
        else if (line == line_down)
        {
            // the line is horizontal -> we change the height
            size.y = line_width;
            expanded_down = false;
        }

        // we trim the line
        line.sizeDelta = size;
    }

}