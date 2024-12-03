using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotherCell : Cell
{
    // a cell that have 1 or 2 children cells (definitions)
    [Header("Mother cell settings")]
    public Definition def1;
    public Definition def2;
    public RectTransform line;
    public GridHandler grid;

    [Header("Arrows")]
    public Image arrow_up;
    public Image arrow_down;
    public List<Sprite> arrows; // 0 : fleche_bas, 1 : fleche_bas_haut, 2 : fleche_droite, 3 : fleche_droite_haut

    // START
    protected override void Start()
    {
        base.Start();

        // we get the components
        def1 = transform.Find("definitions/text_haut/def_bg_haut").GetComponent<Definition>();
        def2 = transform.Find("definitions/text_bas/def_bg_bas").GetComponent<Definition>();

        // we set the image alpha to 0
        image.color = new Color(1, 1, 1, 0);

        // we get the grid handler
        grid = transform.parent.GetComponent<GridHandler>();
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
        }
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
        // we update the top right arrow
        if (def1 != null)
        {
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
            // we check if the direction of the definition is horizontal -> the arrow is at the bottom left of the case and points right
            if (def2.horizontal) { arrow_down.sprite = arrows[3]; }

            // we points down so the arrow points down
            else { arrow_down.sprite = arrows[0]; }
        }
    }

    // DEFINITIONS MANAGEMENT
    public void MakeChildSleep(bool up = true)
    {
        if (up && def1 != null)
        {
            def1.transform.parent.gameObject.SetActive(false);
            def1 = null;

            // we disable the arrow
            arrow_up.gameObject.SetActive(false);
        }
        else if (!up && def2 != null)
        {
            def2.transform.parent.gameObject.SetActive(false);
            def2 = null;

            // we disable the arrow
            arrow_down.gameObject.SetActive(false);
        }
    }
    public void WakeUpChild(bool up = true)
    {
        if (up && def1 == null)
        {
            Transform def1_transform = transform.Find("definitions/text_haut/def_bg_haut");
            def1 = def1_transform.GetComponent<Definition>();
            def1_transform.parent.gameObject.SetActive(true);

            // we enable the arrow
            arrow_up.gameObject.SetActive(true);
        }
        else if (!up && def2 == null)
        {
            Transform def2_transform = transform.Find("definitions/text_bas/def_bg_bas");
            def2 = def2_transform.GetComponent<Definition>();
            def2_transform.parent.gameObject.SetActive(true);

            // we enable the arrow
            arrow_down.gameObject.SetActive(true);
        }
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

        /* Definition[] defs = new Definition[] { def1, def2 };
        for (int i=0; i<contents.Length; i++)
        {
            // we check if we have a definition
            if (contents[i] == "") { MakeChildSleep(i == 0); }
            else
            {
                // we enable the definition
                WakeUpChild(i == 0);

                // we set the content of the definition
                defs[i].SetHex(contents[i]);
            }
        } */

        // we set the content of the children
        def1?.SetHex(contents[0]);
        if (contents.Length == 1) { return; }
        def2?.SetHex(contents[1]);

        /* // we check if we have 1 or 2 definitions
        if (contents.Length == 1)
        {
            // we disable the second definition
            def2.gameObject.SetActive(false);
            def2 = null;
        }
        else
        {
            // we enable the second definition
            def2.SetHex(contents[1]);
        } */

    }
    public override void SetGridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;

        // we set the children grid position
        def1?.SetGridPosition(x, y);
        def2?.SetGridPosition(x, y);
    }
}