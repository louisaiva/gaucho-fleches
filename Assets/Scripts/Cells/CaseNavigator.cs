using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaseNavigator : MonoBehaviour
{
    [Header("Navigation parameters")]
    public Cell selectedCase;
    public float lastNavigationTime = 0f;
    public float navigationCooldown = 0.1f;
    public KeyCode lastNavigationKey; // the last key pressed
    public float lastNavigationKeyTime = 0f; // the last time a key was pressed
    public bool fastNavigation = false; // if true we move the cursor faster
    public float fastNavigationTriggerTime = 0.5f; // after 0.5 seconds of pressing a key we switch to fast navigation

    private Dictionary<KeyCode, Vector2Int> navigationDirections = new Dictionary<KeyCode, Vector2Int>()
    {
        { KeyCode.LeftArrow, new Vector2Int(-1, 0) },
        { KeyCode.RightArrow, new Vector2Int(1, 0) },
        { KeyCode.UpArrow, new Vector2Int(0, -1) },
        { KeyCode.DownArrow, new Vector2Int(0, 1) }
    };

    [Header("Current definition's word")]
    public List<Cell> currentWord = new List<Cell>();
    public int currentWordIndex = -1;

    [Header("Components & References")]
    public GridHandler grid;

    // Start
    void Start()
    {
        // we get the components
        grid = GetComponent<GridHandler>();
    }

    // Navigation methods
    public void UpdateNavigation(Cell selected_case)
    {
        // we set the selected case
        selectedCase = selected_case;

        // we check if the selected case is inside the word
        if (currentWord.Count > 0 && !currentWord.Contains(selectedCase))
        {
            ResetWord();
        }
        else if (currentWord.Count > 0 && currentWord.Contains(selectedCase))
        {
            currentWordIndex = currentWord.IndexOf(selectedCase);
        }

        // we check if we press tab (we switch case / def)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // we reset the navigation not to break the navigation
            ResetNavigation();

            // we check if the selected case is a definition
            if (selectedCase is Definition)
            {
                Definition def = selectedCase as Definition;
                // we get the mother cell
                selectedCase = def.GetMother();
            }

            // we switch the case / def
            selectedCase = grid.SwitchCaseDef(selectedCase);
            if (selectedCase == null) { return; }

            // we update the new case so MotherCell update its children & arrows
            // selectedCase.ForceUpdate();

            // we reset the current word
            ResetWord();

            // we select the new case
            selectedCase.Select();
            return;
        }

        // we get the navigation direction
        Vector2Int direction = Vector2Int.zero;
        foreach (KeyValuePair<KeyCode, Vector2Int> entry in navigationDirections)
        {
            // we check if the key is pressed -> we set the navigation direction
            if (Input.GetKeyDown(entry.Key))
            {
                direction += entry.Value;
                lastNavigationKey = entry.Key;
                lastNavigationKeyTime = Time.time;
            }

            // we check if the key is released -> we reset the navigation
            if (Input.GetKeyUp(entry.Key))
            {
                if (entry.Key == lastNavigationKey)
                {
                    lastNavigationKey = KeyCode.None;
                    lastNavigationKeyTime = 0f;

                    // we reset the fast navigation
                    if (fastNavigation) { fastNavigation = false; }
                }
            }
        }

        // we check if we need to switch to fast navigation
        if (lastNavigationKey != KeyCode.None && !fastNavigation &&
         Time.time - lastNavigationKeyTime > fastNavigationTriggerTime)
        {
            fastNavigation = true;
        }

        // we apply the fast navigation
        if (fastNavigation)
        {
            direction = navigationDirections[lastNavigationKey];
        }

        // we check if we are in the case of a Definition cell
        bool navigate_definition = HandleDefinitionNavigation(selected_case, direction);

        // we check if we can navigate or we need to wait cooldown
        if (Time.time - lastNavigationTime < navigationCooldown) { return; }
        if (direction == Vector2Int.zero & !navigate_definition) { return; }

        // we navigate
        if (!navigate_definition)
        {
            Navigate(direction);
            return;
        }

        // we navigate through the definition
        NavigateDefinition();
    }

    void Navigate(Vector2Int direction)
    {
        // Find and navigate to the next case
        int x = selectedCase.x + direction.x;
        int y = selectedCase.y + direction.y;
        Cell next_case = grid.GetCell(x, y);

        if (next_case == null) return;

        // we check if the case is a MotherCell (we jump on a def)
        next_case = HandleJumpOnDef(next_case, direction);

        // we check if the still current cell is a definition and the next one is a case
        if (selectedCase is Definition && next_case is Case)
        {
            // we get the definition
            Definition def = selectedCase as Definition;

            // we check if the next_case is the start_cell of the definition
            if (def.GetFirstLetter() == next_case)
            {
                // we update the current word
                currentWord = def.GetWord();
                currentWordIndex = 0;
            }
        }

        // we check if we are in a word
        if (currentWord.Count > 0)
        {
            // we check if the next case is in the current word
            if (!currentWord.Contains(next_case))
            {
                // we reset the current word
                ResetWord();
            }
            else
            {
                // we get the new index
                currentWordIndex = currentWord.IndexOf(next_case);
            }
        }

        // we set the navigation time
        lastNavigationTime = Time.time;

        // Stop writing and start writing on the next case
        selectedCase.UnSelect();
        next_case.Select();
    }
    void ResetNavigation()
    {
        lastNavigationKey = KeyCode.None;
        lastNavigationKeyTime = 0f;
        fastNavigation = false;
    }

    // In Word navigation
    public void NavigateInWord(Case cell, int index_delta=1)
    {
        // we check if we can navigate or we need to wait cooldown
        if (Time.time - lastNavigationTime < navigationCooldown) { return; }

        // we check if we have a current word
        if (currentWord.Count == 0) { return; }
        // and that the case is in the current word
        if (!currentWord.Contains(cell)) { return; }

        // we get the new index
        int new_index = currentWordIndex + index_delta;
        if (new_index < 0 || new_index >= currentWord.Count) { return; }

        // we get the new case
        Case new_case = currentWord[new_index] as Case;
        if (new_case == null) { return; }

        // we set the navigation time
        lastNavigationTime = Time.time;
        currentWordIndex = new_index;

        // we stop writing and start writing on the new case
        selectedCase.UnSelect();
        new_case.SelectNextFrame();
        // new_case.input_handler.will_write_next_frame = true;
    }
    public bool IsInWord(Case cell)
    {
        // we check if we have a current word
        if (currentWord.Count == 0) { return false; }
        // and that the case is in the current word
        if (!currentWord.Contains(cell)) { return false; }

        return true;
    }

    public void ResetWord()
    {
        currentWord.Clear();
        currentWordIndex = -1;
    }

    // Definition navigation - a particular case
    bool HandleDefinitionNavigation(Cell selected_case, Vector2Int direction)
    {
        // we check if the selected case is a definition
        if (!(selected_case is Definition)) { return false; }

        // we check the direction
        if (direction.y == 0) { return false; } // definitions are only navigable vertically

        // we check if the definition is alone or not
        Definition def = selected_case as Definition;
        if (def.GetSibling() == null) { return false; }

        // we check if we are on the top or bottom definition (Y axis is inverted)
        if (direction.y < 0 && def.is_on_top) { return false; }
        if (direction.y > 0 && !def.is_on_top) { return false; }

        return true;
    }

    void NavigateDefinition()
    {
        // we get the definition
        Definition def = selectedCase as Definition;

        // we get the new definition
        Definition new_def = def.GetSibling();

        // we set the navigation time
        lastNavigationTime = Time.time;

        // we stop writing and start writing on the new definition
        selectedCase.UnSelect();
        new_def.Select();
    }

    Cell HandleJumpOnDef(Cell original_case, Vector2Int direction)
    {
        // we check if the case is a MotherCell
        if (!(original_case is MotherCell)) { return original_case; }

        // we check if the direction is horizontal
        if (direction.y == 0) { return original_case; } // we don't care about wheter we are on the top or bottom

        // we get the mother cell
        MotherCell mother = original_case as MotherCell;
        if (mother.GetChildrenCount() == 1) { return original_case; } // we don't care if there is only one child

        // we get the definition
        Definition def = direction.y > 0 ? mother.def1 : mother.def2;
        return def;
    }
}