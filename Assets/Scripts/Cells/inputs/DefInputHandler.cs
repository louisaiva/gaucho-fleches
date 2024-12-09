using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DefInputHandler : MonoBehaviour
{
    // custom input handler for the definition input field
    [Header("Input settings")]
    public bool is_writing = false;
    public int max_lines = 5;

    [Header("Backspace management")]
    public bool backspace_fast_mode = false;
    public float backspace_timer = 0f;
    public float backspace_fast_cooldown = 0.5f;
    public float backspace_writing_speed = 0.075f;


    [Header("Components & References")]
    public Definition def;
    public TextMeshProUGUI text;

    
    // START
    void Start()
    {
        // we get the components
        text = GetComponent<TextMeshProUGUI>();
    }

    // UPDATE
    void Update()
    {

        // 1 - we check if we are in writing mode
        if (!is_writing) { return; }


        // 2 - special keys


        // we check if we press escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // we unselect the definition
            def.UnSelect();
            return;
        }

        // we check if we press suppr
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            // we unselect the definition
            def.UnSelect();
            
            // we delete the whole def
            def.GetMother().MakeChildSleep(def.is_on_top, true);
            return;
        }

        // we check if we press backspace
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            // we remove the last letter
            RemoveLetter();

            // we check if we are in fast mode
            if (!backspace_fast_mode)
            {
                // we reset the timer
                backspace_timer = Time.time;
            }
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Backspace))
        {
            // we reset the timer
            backspace_timer = 0f;
            if (backspace_fast_mode) { backspace_fast_mode = false; }
            return;
        }

        // we check the fast backspace mode
        if (!backspace_fast_mode && backspace_timer != 0f && Time.time - backspace_timer >= backspace_fast_cooldown)
        {
            backspace_fast_mode = true;
            backspace_timer = 0f;
        }
        if (backspace_fast_mode)
        {
            backspace_timer += Time.deltaTime;
            if (backspace_timer >= backspace_writing_speed)
            {
                // we remove the last letter
                RemoveLetter();
                backspace_timer = 0f;
            }
        }

        

        // 3 - CTRL - V

        // we check if we press ctrl
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // we check if we press v
            if (Input.GetKeyDown(KeyCode.V))
            {
                // we get the clipboard
                string clipboard = GUIUtility.systemCopyBuffer;


                // we convert the clipboard to hexa
                string hex = "";
                foreach (char c in clipboard.ToString())
                {
                    int tmp = c;
                    hex += string.Format("{0:X}", tmp) + ".";
                }
                Debug.Log("WE COPY PASTE : " + clipboard + " (" + hex + ")");


                // we add the clipboard
                foreach (char letter in clipboard)
                {
                    AddLetter(letter);
                }
            }
        }





        // 4 - input characters
        if (Input.anyKeyDown)
        {
            foreach (char letter in Input.inputString)
            {
                // we convert the letter to hexa
                string hex = "";
                foreach (char c in letter.ToString())
                {
                    int tmp = c;
                    hex += string.Format("{0:X}", tmp);
                }
                // Debug.Log("input string : " + letter + " (" + hex + ")");
                
                // we add the letter
                AddLetter(letter);
            }
        }
    }

    // SELECT & UNSELECT
    public void Write()
    {
        // we set the writing mode
        is_writing = true;
        UpdateMaxLines();
    }
    public void StopWriting()
    {
        // we stop the writing mode
        is_writing = false;
    }
    public void UpdateMaxLines()
    {
        // we check how many lines do we have left (5 - the other definitions lines)
        int other_def_lines = 0;
        Definition other_def = def.GetSibling();
        if (other_def == null) { max_lines = 5; return; }

        // we get the other definition lines
        other_def_lines = other_def.input.GetLinesCount();
        max_lines = 5 - other_def_lines;
    }

    // LETTERS MANAGEMENT
    bool AddLetter(char input)
    {
        string letter = input.ToString();

        // we check if we add a new line
        if (input == '\n' || input == '\r') { letter = "\\n"; }

        // we don't add it if it's a /t
        if (input == '\t') { return false; }

        // we check if we are empty and this is a space
        if (text.text.Length == 0 && input == ' ')
        {
            Debug.Log("empty + space");
            // we don't add the space
            return false;
        }

        // we try to add the letter so we can see if the height is too big
        text.text += letter;
        text.ForceMeshUpdate();

        // we check if the height is too big
        // the width is adapting itself to the space so we don't need to check it
        if (GetLinesCount() <= max_lines) { return true; }

        // we remove the last letter
        text.text = text.text[..^letter.Length];
        return false;
    }
    bool RemoveLetter()
    {
        // we check if we have a letter to remove
        if (text.text.Length == 0) { return false; }

        // we get the last letter
        char last_letter = text.text[text.text.Length - 1];

        // we check special characters
        if (last_letter == 'n' && text.text.Length > 1 && text.text[text.text.Length - 2] == '\\')
        {
            // we remove the last 2 characters
            text.text = text.text[..^1];
        }

        // we remove the last character
        text.text = text.text[..^1];
        return true;
    }

    // GETTERS
    public int GetLinesCount()
    {
        // calculate the height of the text
        float height = text.textBounds.size.y;
        int lines = Mathf.CeilToInt(height / text.fontSize) - 1;
        if (lines < 0) { lines = 0; }

        // Debug.Log("height : " + height + " / " + text.fontSize + " = " + lines);

        return lines;
    }
}