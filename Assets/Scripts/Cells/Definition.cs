using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Definition : Cell
{
    [Header("Definition settings")]
    public bool is_on_top = false; // either the definition is on top or on bottom (in its cell)

    [Header("Components & References")]
    public TextMeshProUGUI text;
    public DefInputHandler input;
    private MotherCell mother;


    // START
    protected override void Start()
    {
        // we get the components
        image = GetComponent<Image>();

        // we get the mother cell
        mother = transform.parent.parent.parent.GetComponent<MotherCell>();
        navigator = mother.GetNavigator();
    }

    // GETTERS
    public Definition GetSibling()
    {
        if (mother == null) { Start(); }

        if (is_on_top) { return mother.def2; }
        else { return mother.def1; }
    }

    public MotherCell GetMother()
    {
        if (mother == null) { Start(); }

        return mother;
    }

    // SET CASE
    public override void Select()
    {
        base.Select();

        // we select the input field
        input.Write();
    }
    public override void UnSelect()
    {
        base.UnSelect();

        // we unselect the input field
        input.StopWriting();
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