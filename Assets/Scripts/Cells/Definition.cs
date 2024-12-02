using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Definition : Cell
{
    [Header("Definition settings")]
    public bool is_on_top = false; // either the definition is on top or on bottom (in its cell)

    [Header("Components & References")]
    public TextMeshProUGUI text;
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
        if (is_on_top) { return mother.def2; }
        else { return mother.def1; }
    }

    // SET CASE
    public override void Select()
    {
        base.Select();
    }
    public override void UnSelect()
    {
        base.UnSelect();
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
            hex += string.Format("{0:X}", tmp);
        }
        return hex;
    }
    public void SetHex(string hex)
    {
        // on reconvertit en string
        string content = "";
        for (int i = 0; i < hex.Length; i += 2)
        {
            string hex_char = hex.Substring(i, 2);
            content += (char)System.Convert.ToInt32(hex_char, 16);
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