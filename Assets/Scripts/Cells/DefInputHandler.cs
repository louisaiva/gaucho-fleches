using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DefInputHandler : MonoBehaviour
{
    // custom input handler for the definition input field
    [Header("Input settings")]
    public bool is_writing = false;
    public int max_characters_per_line = 50;
    public int max_lines = 4;
    public float max_width = 40f;
    
    [Header("Components & References")]
    public Definition def;
    public TextMeshProUGUI text;

    /* private char[] authorized_letters = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                                                    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                                                    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                                                    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                                                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                                                    '!', '?', '.', ',', ';', ':', '(', ')', '[', ']', '{', '}',
                                                    '-', '_', '+', '=', '*', '/', '%', '&', '#', '@', '$', //'€',
                                                    /* '£', '¥', '§', '°', '²', '³', 'µ', '¤',  '¨', '´', '`', '^',
                                                    '~', '<', '>', '|', '\\', '"', '\''/* , ' ', '\n', '\t'  }; */

    // characteres interdits : £ ¥ § ° ² ³ µ ¤ ¨ ´ space, \t \n € 


    // START
    void Start()
    {
        // we get the components
        text = GetComponent<TextMeshProUGUI>();
    }


    // UPDATE
    void Update()
    {
        // we check if we are in writing mode
        if (!is_writing) { return; }

        // we check if we press escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // we unselect the definition
            def.UnSelect();
            return;
        }

        /* string unrecognized = "non reconized characters : ";
        // we check if we press an authorized key
        foreach (char letter in authorized_letters)
        {
            // we check if the key is recognized
            try
            {
                Input.GetKeyDown(letter.ToString());
            }
            catch (System.Exception)
            {
                // we convert the letter to hexa
                string hex = "";
                foreach (char c in letter.ToString())
                {
                    int tmp = c;
                    hex += string.Format("{0:X}", tmp);
                }
                // unrecognized += hex + " "; 
                unrecognized += letter + " ";
                continue;
            }

            // we check if the key is pressed
            if (Input.GetKeyDown(letter.ToString()))
            {
                // we check if we press shift
                bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                // we add the letter
                AddLetter(shift ? char.ToUpper(letter) : letter);
                break;
            }
        }
        // if (unrecognized != "non reconized characters : ") { Debug.Log(unrecognized); } */

        // we check if we press a key
        if (Input.anyKeyDown)
        {
            foreach (char letter in Input.inputString)
            {
                // Debug.Log("input string : " + letter);

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
    }
    public void StopWriting()
    {
        // we stop the writing mode
        is_writing = false;
    }

    // LETTERS MANAGEMENT
    bool AddLetter(char letter)
    {
        // we check if this is a backspace
        if (letter == '\b')
        {
            // we remove the last letter
            if (text.text.Length > 0)
            {
                text.text = text.text.Substring(0, text.text.Length - 1);
            }
            return true;
        }
        else if (letter == '\n')
        {
            // we add a new line
            return Return();
        }

        // we try to add the letter so we can see if the width is too big
        text.text += letter;
        text.ForceMeshUpdate();

        // we get the width of the text
        float width = text.textBounds.size.x;
        if (width < max_width) { return true; }
        
        // we remove the last letter
        text.text = text.text.Substring(0, text.text.Length - 1);

        // we check if we can add a new line, and we do if we can
        if (!Return()) { return false; }

        // we finally add the letter
        text.text += letter;
        return true;
    }

    bool Return()
    {
        // we check if we can add a new line
        if (GetLinesCount() >= max_lines) { return false; }

        string last_line = GetLastLine();
        // we check if the last line is empty
        if (last_line.Trim().Length == 0) { return false; }
        // we check if we have a space at the end of the last line
        if (last_line[last_line.Length - 1] == ' ')
        {
            // we remove the space
            text.text = text.text.Substring(0, text.text.Length - 1);
        }

        // we add a new line
        text.text += "\n";
        return true;
    }

    public string GetLastLine()
    {
        // we get the last line
        List<string> lines = GetLines();
        if (lines.Count == 0) { return ""; }
        return lines[lines.Count - 1];
    }

    public int GetLinesCount()
    {
        // we get the lines count
        /* int lines = 1;
        int characters_in_line = 0;
        for (int i = 0; i < text.text.Length; i++)
        {
            if (text.text[i] == '\n')
            {
                lines++;
                characters_in_line = 0;
            }
            else
            {
                characters_in_line++;
                if (characters_in_line >= max_characters_per_line)
                {
                    lines++;
                    characters_in_line = 0;
                }
            }
        } */
        int lines = GetLines().Count;
        return lines;
    }

    public List<string> GetLines()
    {
        // we get the lines
        List<string> lines = new List<string>();
        string line = "";
        int characters_in_line = 0;
        for (int i = 0; i < text.text.Length; i++)
        {
            if (text.text[i] == '\n')// || characters_in_line >= max_characters_per_line)
            {
                lines.Add(line);
                line = "";
                characters_in_line = 0;
            }
            else
            {
                line += text.text[i];
                characters_in_line++;
            }
        }
        lines.Add(line);
        return lines;
    }
}