using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Case : Cell
{
    
    [Header("Components & References")]
    public TextMeshProUGUI letter;
    public CaseInputHandler input_handler;



    // START
    protected override void Start()
    {
        base.Start();

        // we get the components
        input_handler = transform.Find("letter").GetComponent<CaseInputHandler>();
        letter = transform.Find("letter").GetComponent<TextMeshProUGUI>();
    }

    // SELECT & UNSELECT
    public override void Select()
    {
        base.Select();
        
        // we start writing
        input_handler.Write();
    }

    public override void UnSelect()
    {
        base.UnSelect();

        // we stop writing
        input_handler.StopWriting();
    }

    // CONTENT
    public void DeleteContent()
    {
        input_handler.DeleteLetter();
    }
    public override string GetContent()
    {
        return letter.text;
    }
    public override void SetContent(string content)
    {
        input_handler.SetLetter(content);
    }
}