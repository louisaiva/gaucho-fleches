using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotherCell : Cell
{
    // a cell that have 1 or 2 children cells (definitions)
    [Header("Mother cell settings")]
    public Definition def1;
    public RectTransform line;
    public Definition def2;

    [Header("Definitions management")]
    public bool defs_initialized = false;
    public bool force_def1_sleep = false;
    public bool force_def2_sleep = false;

    [Header("Arrows")]
    public Image arrow_up;
    public Image arrow_down;
    public List<Sprite> arrows; // 0 : fleche_bas, 1 : fleche_bas_haut, 2 : fleche_droite, 3 : fleche_droite_haut

    // START
    protected override void Start()
    {
        base.Start();
     
        // we set the image alpha to 0
        image.color = new Color(1, 1, 1, 0);

        if (defs_initialized) { return; }

        // we get the components if the transform are not disabled
        def1 = transform.Find("definitions/text_haut/def_bg_haut").GetComponent<Definition>();
        def2 = transform.Find("definitions/text_bas/def_bg_bas").GetComponent<Definition>();

        defs_initialized = true;
    }

    // UPDATE
    protected override void Update()
    {
        // we do nothing !!
        // we don't want the mother cell to be selected
        // we want the children to be selected
        // so we do nothing here
        // this text is long !!
        // the end of the development is still far away...
        // keep going my fellow developer :D
        // ...
        // (i lied we do a little few things there but we do not update the color because we
        // don't have graphics for the mother cell !!)

        // we update the children definitions
        UpdateDefinitions();

        // we update the line
        UpdateLine();

        // we update the arrows
        UpdateArrows();

    }
    void UpdateDefinitions()
    {
        // we check if we are properly started
        if (grid == null) { Start(); }

        // we check if we can have a right definition (if the first cell to the right is a normal case it's good)
        Cell right_cell = grid.GetCell(x + 1, y);
        if (right_cell == null || !(right_cell is Case))
        {
            // we disable the right definition (the def on top)
            MakeChildSleep();
        }
        else
        {
            // we enable the right definition (the def on top)
            WakeUpChild();
        }

        // we check if we can have a bottom definition (if the first cell to the bottom is a normal case it's good)
        Cell bottom_cell = grid.GetCell(x, y + 1); // Y axis is inverted
        if (bottom_cell == null || !(bottom_cell is Case))
        {
            // we disable the bottom definition (the def on bottom)
            MakeChildSleep(false);
        }
        else
        {
            // we enable the bottom definition (the def on bottom)
            WakeUpChild(false);
        }

        // we check how many children we have
        if (GetChildrenCount() == 0)
        {
            // if we are selected we unselect the mother cell
            UnSelect();

            // we switch the mother cell to a case
            grid.SwitchCaseDef(this);
            return;
        }

        // here we have at least one child
        // we set up the direction of the children
        
        if (def1 != null && def1.y == 0) { def1.horizontal = false; } // the first definition is on top
        if (def2 != null && def2.x == 0) { def2.horizontal = true; } // the second definition is on bottom
    }
    void UpdateLine()
    {
        if (line == null) { return; }

        // if we have only one child we disable the line
        if (def1 == null || def2 == null)
        {
            line.gameObject.SetActive(false);
            return;
        }

        // here we have 2 children so
        // we set the line position at the border between the two definitions
        line.gameObject.SetActive(true);
        RectTransform def1_rect = def1.GetComponent<RectTransform>();
        float y = -def1_rect.rect.height;
        line.anchoredPosition = new Vector2(line.anchoredPosition.x, y);
    }
    void UpdateArrows()
    {
        // we get the grid spacing
        if (grid == null) { Start(); }
        float spacing = grid.GetComponent<GridLayoutGroup>().spacing.y;

        // we update the top right arrow
        if (def1 != null)
        {

            // we check if the position of the arrow is okey -> should be (spacing, 0)
            RectTransform arrow_up_rect = arrow_up.rectTransform;
            if (arrow_up_rect.anchoredPosition != new Vector2(spacing, 0)) { arrow_up_rect.anchoredPosition = new Vector2(spacing, 0); }

            // we check if the direction of the definition is vertical -> the arrow is at the top of the case and points down
            if (!def1.horizontal) { arrow_up.sprite = arrows[1]; }

            // we only have one definition -> the arrow is in the middle of the case
            else if (def2 == null) { arrow_up.sprite = arrows[2]; }

            // we have more lines than the second definition, the arrow is in middle of the case
            else if (def1.input.GetLinesCount() > def2.input.GetLinesCount()) { arrow_up.sprite = arrows[2]; }

            // we have less lines than the second definition, the arrow is at the top of the case
            else { arrow_up.sprite = arrows[3]; }
        }

        // we update the bottom arrow
        if (def2 != null)
        {

            // we check if the position of the arrow is okey -> should be (0, -spacing)
            RectTransform arrow_down_rect = arrow_down.rectTransform;
            if (arrow_down_rect.anchoredPosition != new Vector2(0, -spacing)) { arrow_down_rect.anchoredPosition = new Vector2(0, -spacing); }

            // we check if the direction of the definition is horizontal -> the arrow is at the bottom left of the case and points right
            if (def2.horizontal) { arrow_down.sprite = arrows[3]; }

            // we points down so the arrow points down
            else { arrow_down.sprite = arrows[0]; }
        }
    }


    // DEFINITIONS MANAGEMENT
    public void MakeChildSleep(bool up = true, bool force = false)
    {
        if (up)
        {
            if (def1 != null)
            {
                def1.transform.parent.gameObject.SetActive(false);
                def1 = null;
            }

            // we disable the arrow
            arrow_up.gameObject.SetActive(false);

            // we check if we force
            if (force) { force_def1_sleep = true; }
        }
        else
        {
            if (def2 != null)
            {
                def2.transform.parent.gameObject.SetActive(false);
                def2 = null;
            }

            // we disable the arrow
            arrow_down.gameObject.SetActive(false);

            // we check if we force
            if (force) { force_def2_sleep = true; }
        }
    }
    public void WakeUpChild(bool up = true, bool force = false)
    {
        if (up && def1 == null && (!force_def1_sleep || force))
        {
            Transform def1_transform = transform.Find("definitions/text_haut/def_bg_haut");
            def1 = def1_transform.GetComponent<Definition>();
            def1_transform.parent.gameObject.SetActive(true);

            // we enable the arrow
            arrow_up.gameObject.SetActive(true);

            // we reset the force
            force_def1_sleep = false;
        }
        else if (!up && def2 == null && (!force_def2_sleep || force))
        {
            Transform def2_transform = transform.Find("definitions/text_bas/def_bg_bas");
            def2 = def2_transform.GetComponent<Definition>();
            def2_transform.parent.gameObject.SetActive(true);

            // we enable the arrow
            arrow_down.gameObject.SetActive(true);

            // we reset the force
            force_def2_sleep = false;
        }
    }

    public string GetDefHorizontals()
    {
        string directions = "";
        if (def1 != null) { directions += def1.IsHorizontal() ? "1" : "0"; }
        directions += ".";
        if (def2 != null) { directions += def2.IsHorizontal() ? "1" : "0"; }
        return directions;
    }
    public void SetDefHorizontals(string directions)
    {
        
        // we split the directions
        string[] dirs = directions.Split('.');

        // check the up definition
        if (dirs[0] == "1") { def1.SetHorizontal(true); }
        else if (dirs[0] == "0") { def1.SetHorizontal(false); }
        else { MakeChildSleep(true,true); }

        // we check if we have only one definition
        if (dirs.Length == 1)
        {
            MakeChildSleep(false, true);
            return;
        }

        // check the down definition
        if (dirs[1] == "1")
        {
            if (def2 == null) { WakeUpChild(false,true); }
            def2.SetHorizontal(true);
        }
        else if (dirs[1] == "0" && def2 != null) { def2.SetHorizontal(false); }
        else { MakeChildSleep(false,true); }
    }

    public override void Standby(bool value)
    {
        // we set the standby mode
        def1?.Standby(value);
        def2?.Standby(value);
    }

    // SELECT & UNSELECT
    public override void Select()
    {
        // we select the first definition
        if (def1 != null) { def1.Select(); }
        else { def2?.Select(); }
    }
    public override void UnSelect()
    {
        // we unselect both definitions
        def1?.UnSelect();
        def2?.UnSelect();
    }

    // GETTERS
    public CaseNavigator GetNavigator()
    {
        return navigator;
    }
    public override string GetContent()
    {
        string content = "";
        if (def1 != null) { content += def1.GetHex(); }
        content += "%_%";
        if (def2 != null) { content += def2.GetHex(); }
        return content;
    }
    public int GetChildrenCount()
    {
        return (def1 == null ? 0 : 1) + (def2 == null ? 0 : 1);
    }

    // SETTERS
    public override void SetContent(string content)
    {
        // we split the content
        string[] contents = content.Split(new string[] { "%_%" }, System.StringSplitOptions.None);

        // we set the content of the children
        def1?.SetHex(contents[0]);
        if (contents.Length == 1) { return; }
        def2?.SetHex(contents[1]);

    }
    public override void SetGridPosition(int x, int y)
    {
        base.SetGridPosition(x, y);

        // we check if we are properly started
        if (grid == null) { Start(); }

        // we set the children grid position
        def1?.SetGridPosition(x, y);
        def2?.SetGridPosition(x, y);
    }
}