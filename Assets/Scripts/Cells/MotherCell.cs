using UnityEngine;

public class MotherCell : Cell
{
    // a cell that have 1 or 2 children cells (definitions)
    [Header("Mother cell settings")]
    public Definition def1;
    public Definition def2;
    public RectTransform line;

    // START
    protected override void Start()
    {
        base.Start();

        // we get the components
        def1 = transform.Find("definitions/text_haut/def_bg_haut").GetComponent<Definition>();
        def2 = transform.Find("definitions/text_bas/def_bg_bas").GetComponent<Definition>();

        // we set the image alpha to 0
        image.color = new Color(1, 1, 1, 0);
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

        // we update the line
        if (line != null)
        {
            // if we have only one child we disable the line
            if (def2 == null)
            {
                line.gameObject.SetActive(false);
            }
            else
            {
                // we set the line position at the border between the two definitions
                line.gameObject.SetActive(true);
                RectTransform def1_rect = def1.GetComponent<RectTransform>();
                float y = -def1_rect.rect.height;
                line.anchoredPosition = new Vector2(line.anchoredPosition.x, y);
            }
        }
    }

    // SELECT & UNSELECT
    public override void Select()
    {
        // we select the first definition
        def1.Select();
    }
    public override void UnSelect()
    {
        // we unselect both definitions
        def1.UnSelect();
        def2.UnSelect();
    }

    // GETTERS
    public CaseNavigator GetNavigator()
    {
        return navigator;
    }
    public override string GetContent()
    {
        return def1.GetHex()+ "%_%" +def2.GetHex();
    }
    public int GetChildrenCount()
    {
        if (def2 == null) { return 1; }
        return 2;
    }

    // SETTERS
    public override void SetContent(string content)
    {
        // we split the content
        string[] contents = content.Split(new string[] { "%_%" }, System.StringSplitOptions.None);

        // we set the content of the children
        def1.SetHex(contents[0]);

        // we check if we have 1 or 2 definitions
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
        }

    }
    public override void SetGridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;

        // we set the children grid position
        def1.SetGridPosition(x, y);
        def2.SetGridPosition(x, y);
    }
}