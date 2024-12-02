using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Numerics;

public class CaseInputHandler : MonoBehaviour
{
    // public Case case_target;
    public bool is_writing = false;
    public float lastNavigationTime = 0f;
    
    [Header("Components & References")]
    public TextMeshProUGUI input_text;
    public char[] authorized_letters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
                                                    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };


    // START
    void Start()
    {
        // we get the components
        input_text = GetComponent<TextMeshProUGUI>();
    }

    // UPDATE
    void Update()
    {
        if (!is_writing) { return; }

        // we check if enter is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            transform.parent.GetComponent<Cell>().UnSelect();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            input_text.text = "";
        }

        // we check if a key is pressed
        foreach (char letter in authorized_letters)
        {
            if (Input.GetKeyDown(letter.ToString().ToLower()))
            {
                // we switch the letter
                input_text.text = letter.ToString();
            }
        }
    }



    // PUBLIC METHODS

    public void SetLetter(string letter)
    {
        // check if null
        if (letter == "") { return; }

        // check if the letter is authorized
        if (System.Array.IndexOf(authorized_letters, letter[0]) == -1) { return; }

        // we set the letter
        if (input_text == null) { input_text = GetComponent<TextMeshProUGUI>(); }
        input_text.text = letter[0].ToString();
    }

    public void Write()
    {
        is_writing = true;

        // we set the navigation time
        lastNavigationTime = Time.time;
    }

    public void StopWriting()
    {
        is_writing = false;
    }
}