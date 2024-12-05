using UnityEngine;

public class GridBodyWindowManager : MonoBehaviour
{

    [Header("Moving around settings")]
    public KeyCode move_key = KeyCode.LeftAlt;
    public bool is_moving = false;
    public float moving_speed = 1f;
    public Texture2D handCursorTexture;
    public Texture2D arrowCursorTexture;

    [Header("Zoom settings")]
    public float zoom_speed = 1f;
    public float zoom_min = 0.5f;
    public float zoom_max = 2f;
    public RectTransform grid;

    // fix the height of the rect transform based on the parent's height - the target rect transform height
    [Header("Height Fix")]
    public RectTransform target;
    private RectTransform parent;
    private RectTransform rect; // the rect transform of the object

    // START
    void Start()
    {
        // we get the components
        parent = transform.parent.GetComponent<RectTransform>();
        rect = GetComponent<RectTransform>();
    }

    // ON RESIZE
    void OnResize()
    {
        // we fix the height
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, parent.sizeDelta.y - target.sizeDelta.y);
    }

    // UPDATE
    void Update()
    {
        // we check if the mouse wheel is used
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Zoom(Input.GetAxis("Mouse ScrollWheel") * zoom_speed);
        }
        else if (Input.GetMouseButton(2)) // we check if the middle mouse button is pressed
        {
            // we reset the zoom
            ResetZoom();
        }

        // we check if we are moving
        UpdateMoving();

    }


    // ZOOM
    public void Zoom(float zoom)
    {

        // we get the mouse position inside the grid
        Vector2 position_before_zoom;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(grid, Input.mousePosition, null, out position_before_zoom);


        // 1 - WE ZOOm

        // we get the new scale
        float new_scale = transform.localScale.x + zoom;

        // we check if the new scale is in the limits
        if (new_scale < zoom_min || new_scale > zoom_max)
        {
            // we set the new scale to the limit
            new_scale = Mathf.Clamp(new_scale, zoom_min, zoom_max);
        }

        // we set the new scale
        transform.localScale = new Vector3(new_scale, new_scale, new_scale);


        // 2 - WE MOVE to keep the mouse position in the same place

        // we get the mouse position inside the grid
        Vector2 position_after_zoom;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(grid, Input.mousePosition, null, out position_after_zoom);

        // we calculate the difference with consideration of the zoom
        Vector2 difference = (position_after_zoom - position_before_zoom);// / new_scale;

        // we set the position of the grid
        grid.anchoredPosition += difference;
    }
    public void ResetZoom()
    {
        // reset the position of the grid
        grid.anchoredPosition = new Vector2(0, 0);
        
        // set scale on 1 1 1
        transform.localScale = new Vector3(1, 1, 1);
    }


    // MOVING VIEW
    public void UpdateMoving()
    {
        // change the cursor
        if (Input.GetKeyDown(move_key))
        {
            Cursor.SetCursor(arrowCursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else if (Input.GetKeyUp(move_key))
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        // we check if we hold space and we drag the mouse with the left click
        if (Input.GetKey(move_key) && Input.GetMouseButton(0) && !is_moving)
        {
            is_moving = true;
        }
        else if (is_moving && (!Input.GetKey(move_key) || !Input.GetMouseButton(0)))
        {
            is_moving = false;
        }

        // we check if we are moving
        if (!is_moving) { return; }

        // we move the grid
        Vector2 delta_position = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // Debug.Log("Moving " + delta_position);
        grid.anchoredPosition += delta_position*moving_speed;
    }

}