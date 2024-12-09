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

    // UPDATE
    protected override void Update()
    {
        base.Update();

        // we check if we are in the navigator's word, then we light up
        if (!hovered && !selected && navigator != null && navigator.IsInWord(this))
        {
            image.color = color_lighted;
        }
    }


    // SELECT & UNSELECT
    public override void Select()
    {
        base.Select();
        
        // we start writing
        input_handler.Write();
    }
    public void SelectNextFrame()
    {
        base.Select();

        // we start writing next frame
        input_handler.will_write_next_frame = true;
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
    public char GetChar()
    {
        return letter.text[0];
    }
}