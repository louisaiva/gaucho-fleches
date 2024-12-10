using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Definition : Cell
{
    [Header("Definition settings")]
    public bool is_on_top = false; // either the definition is on top or on bottom (in its cell)
    public bool horizontal; // the direction of the definition (if true, horizontal, if false, vertical)
    public int length = 1; // the length of the definition (in cells)


    // [Header("Target word")]

    [Header("Components & References")]
    public TextMeshProUGUI text;
    public DefInputHandler input;
    private MotherCell mother;
    private Cell start_cell;


    // START
    protected override void Start()
    {
        // we get the components
        image = GetComponent<Image>();

        // we get the mother cell
        mother = transform.parent.parent.parent.GetComponent<MotherCell>();
        navigator = mother.GetNavigator();

        // we get the grid
        grid = transform.parent.parent.parent.parent.GetComponent<GridHandler>();
        // Debug.Log("Definition grid: " + grid);
    }

    // UPDATE
    protected override void Update()
    {
        base.Update();

        // we update the length of the definition
        // todo we check where is the next definition cell
        // ehe that's we do today !

        // we update length
        UpdateLength();
    }

    public override void RightClick()
    {
        // we get the grid
        GridHandler grid = mother.grid;
        grid.SwitchCaseDef(mother);
    }


    // TARGET WORD
    public void UpdateLength()
    {        
        // we get the next wall cell
        int max_index = horizontal ? grid.columns : grid.rows;

        for (int i=0; i<=max_index; i++)
        {
            Cell c = GetLetter(i);
            if (c == null || c is not Case)
            {
                length = i;
                return;
            }
        }

        // if we are here that means we did not find the next wall cell
        Debug.LogError("(ERROR DEFINITION UPDATE LENGTH) We did not find the next def !"
            + mother.name + " (" + (horizontal ? "horizontal)" : "vertical)") + " start cell : " + start_cell.name); return;
    }

    public void LightWord(bool off=false)
    {
        for (int i=0; i<length; i++)
        {
            Cell c = GetLetter(i);
            if (c == null) { break; }

            if (off) { c.UnLight(); }
            else { c.Light(); }
        }
    }

    public bool TryWord(string word)
    {
        // we check the length
        if (word.Length != length) { return false; }

        // we check the letters that are already written
        for (int i=0; i<length; i++)
        {
            Cell c = GetLetter(i);
            if (c is not Case) { return false; }
            
            // we check the letter
            if (((Case) c).GetChar() != word[i]) { return false; }
        }

        return true;
    }



    // SELECT & UNSELECT
    public override void Select()
    {
        base.Select();

        // we light the word
        LightWord();

        // we select the input field
        input.Write();
    }
    public override void UnSelect()
    {
        base.UnSelect();

        // we unlight the word
        LightWord(true);

        // we unselect the input field
        input.StopWriting();
    }

    // SETTERS
    public void SwitchDirection()
    {
        horizontal = !horizontal;
    }
    public void SetHorizontal(bool horizontal)
    {
        this.horizontal = horizontal;
    }

    // GETTERS
    public Definition GetSibling()
    {
        if (mother == null) { Start(); }

        if (is_on_top) { return mother.def2; }
        else { return mother.def1; }
    }
    public bool IsAlone()
    {
        return GetSibling() == null;
    }
    public MotherCell GetMother()
    {
        if (mother == null) { Start(); }

        return mother;
    }
    public bool IsHorizontal()
    {
        return horizontal;
    }
    public Cell GetLetter(int index)
    {
        // we get the first cell
        if (start_cell == null) { start_cell = GetFirstLetter(); }
        if (start_cell == null) { return null; }

        // if index = 0, we return the start_cell
        if (index == 0) { return start_cell; }

        // return null if the index is out of range
        if (horizontal)
        {
            return grid.GetCell(start_cell.x + index, start_cell.y);
        }
        else
        {
            return grid.GetCell(start_cell.x, start_cell.y + index);
        }
    }
    public Cell GetFirstLetter()
    {
        // we get the grid
        if (grid == null || mother == null) { Start(); }

        // we get the offset
        Vector2Int offset = new(0, 0);
        if (!is_on_top)
        {
            offset = new(0, 1);
        }
        else if (is_on_top)
        {
            offset = new(1, 0);
        }

        // we log
        string log = "Getting first letter of " + mother.name + " - ";
        Debug.Log(log + "grid: " + grid);

        return grid.GetCell(mother.x + offset.x, mother.y + offset.y);
    }
    public List<Cell> GetWord()
    {
        List<Cell> word = new List<Cell>();

        for (int i=0; i<length; i++)
        {
            Cell c = GetLetter(i);
            if (c == null) { break; }

            word.Add(c);
        }

        return word;
    }

    // CONTENT
    public string GetHex()
    {
        string content = text.text;

        // on convertit en hexa pour garder les caractères spéciaux
        string hex = "";
        foreach (char c in content)
        {
            int tmp = c;
            hex += string.Format("{0:X}", tmp) + ".";
        }
        // on enlève le dernier point
        if (hex.Length > 0) {hex = hex[..^1];}

        // on retourne le résultat
        return hex;
    }
    public void SetHex(string hex)
    {
        // on reconvertit en string
        string content = "";
        /* for (int i = 0; i < hex.Length; i += 2)
        {
            string hex_char = hex.Substring(i, 2); 
            content += (char)System.Convert.ToInt32(hex_char, 16);
        } */
        string[] hex_chars = hex.Split('.');
        foreach (string hex_char in hex_chars)
        {
            // check if the hex_char is empty
            if (hex_char == "") { continue; }

            // we convert the hex char to a character
            try
            {
                content += (char)System.Convert.ToInt32(hex_char, 16);
            }
            catch
            {
                content += "?";
                Debug.LogWarning("(Definition - SetHex) Unrecognized hex character : " + hex_char);
            }
        }
        text.text = content;
    }
    public override string GetContent()
    {
        return text.text;
    }
    public override void SetContent(string content)
    {
        text.text = content;
    }

}