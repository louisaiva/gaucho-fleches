using System.Collections.Generic;
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

        // we check if we press tab (we switch case / def)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // we reset the navigation not to break the navigation
            ResetNavigation();

            // we switch the case / def
            selectedCase = grid.SwitchCaseDef(selectedCase);
            if (selectedCase == null) { return; }

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

        // we check if we can navigate or we need to wait cooldown
        if (Time.time - lastNavigationTime < navigationCooldown) { return; }
        if (direction == Vector2Int.zero) { return; }

        // we navigate
        Navigate(direction);
    }

    void Navigate(Vector2Int direction)
    {
        // Find and navigate to the next case
        int x = selectedCase.x + direction.x;
        int y = selectedCase.y + direction.y;
        Cell next_case = grid.GetCell(x, y);

        if (next_case == null) return;

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

}