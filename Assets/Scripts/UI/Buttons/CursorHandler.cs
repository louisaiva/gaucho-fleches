using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    
    [Header("Textures")]
    public Texture2D handCursorTexture;
    public Texture2D arrowCursorTexture;

    [Header("Cursor demands")]
    public List<CursorDemand> cursorDemands = new List<CursorDemand>();

    // START
    void Start()
    {
        // we set the default cursor
        Cursor.SetCursor(arrowCursorTexture, Vector2.zero, CursorMode.Auto);

        // we ask for the default cursor
        AddCursorDemand(gameObject, "", 0);
    }

    // UPDATE
    void UpdateCursor()
    {
        // we get the first demand
        CursorDemand demand = cursorDemands.Count > 0 ? cursorDemands[0] : null;
        if (demand == null) { return; }

        // we get the cursor texture
        Texture2D cursorTexture = null;
        if (demand.cursorTexture == "hand") { cursorTexture = handCursorTexture; }
        else if (demand.cursorTexture == "arrow") { cursorTexture = arrowCursorTexture; }

        // we set the cursor
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    // ASK CURSOR
    public void AddCursorDemand(GameObject gameObject, string cursorTexture, int priority=1)
    {
        // we create the demand
        CursorDemand demand = new CursorDemand(gameObject, cursorTexture, priority);

        // we add the demand to the list
        cursorDemands.Add(demand);

        // we sort the list
        cursorDemands.Sort((y, x) => x.priority.CompareTo(y.priority));

        string log = gameObject.name + " demand cursor " + cursorTexture + " with priority " + priority;
        foreach (CursorDemand d in cursorDemands)
        {
            log += "\n\t" + d.gameObject.name + " - " + d.cursorTexture + " - " + d.priority;
        }
        // Debug.Log(log);

        // we update the cursor
        UpdateCursor();
    }

    public void RemoveCursorDemand(GameObject gameObject)
    {
        // we remove the demand from the list
        cursorDemands.RemoveAll(d => d.gameObject == gameObject);

        // Debug.Log(gameObject.name + " remove demand cursor");

        // we update the cursor
        UpdateCursor();
    }

}

public class CursorDemand
{
    public GameObject gameObject;
    public string cursorTexture; // "hand" or "arrow" or "" for default
    public int priority;

    public CursorDemand(GameObject gameObject, string cursorTexture, int priority)
    {
        this.gameObject = gameObject;
        this.cursorTexture = cursorTexture;
        this.priority = priority;
    }
}