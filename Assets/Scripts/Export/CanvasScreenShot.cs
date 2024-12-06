using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScreenShot : MonoBehaviour
{
    /*
    CanvasScreenShot by programmer.
    http://stackoverflow.com/questions/36555521/unity3d-build-png-from-panel-of-a-unity-ui#36555521
    http://stackoverflow.com/users/3785314/programmer
    */

    /*
    adapted from original by deltasfer.
    because i needed to take a screenshot of multiples canvas at once 
    */

    // [Header("Events")]
    public delegate void takePictureHandler(byte[] pngArray);
    public static event takePictureHandler OnPictureTaken;
    public delegate void takePartOfPicture(byte[] pngArray);
    public static event takePartOfPicture OnScreenshotTaken;

    [Header("Parametres globaux")]
    RectTransform gridRect;
    RenderTexture gridshot;
    public int spacing_offset_x = 0;
    public int spacing_offset_y = 0;
    public int export_width = 0;
    public int export_height = 0;

    [Header("Suivi de la progression")]
    public int current_x = 0;
    public int current_y = 0;
    public int last_shot_width = 0;
    public int last_shot_height = 0;


    // Start is called before the first frame update
    void Start()
    {
        // we subscribe to the event
        OnScreenshotTaken += OnPartOfScreenshotTaken;
    }

    // takes global screenshot (lol it's a gridshot bcz we take a screenshot of the grid)
    public void Gridshot(Canvas canva, int px_per_cell, bool solutions)
    {
        
        // we get the grid
        GridHandler grid = canva.transform.GetChild(0).GetComponent<GridHandler>();

        // we prepare the grid
        grid = PrepareGrid(grid,solutions);


        // we apply the scale and move the grid to the top left corner
        ApplyScaleAndMoveGridToTopLeftCorner(grid, canva, px_per_cell);

        RebuildLayout(gridRect);

        // we create the render texture
        CreateRenderTexture();

        // we calculate the size of the first screenshot
        Vector2Int size = CalculateNextShotSize();

        // we take the first screenshot
        StartCoroutine(_Screenshot(size.x, size.y));
    }


    // preparing exportation
    public void Reset()
    {
        // we reset all the variables
        current_x = 0;
        current_y = 0;
        last_shot_width = 0;
        last_shot_height = 0;
        spacing_offset_x = 0;
        spacing_offset_y = 0;
        export_width = 0;
        export_height = 0;
        gridRect = null;
        gridshot = null;
    }
    private void RebuildLayout(RectTransform rectTransform)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        Canvas.ForceUpdateCanvases();
    }

    private GridHandler PrepareGrid(GridHandler grid, bool solutions)
    {
        gridRect = grid.GetComponent<RectTransform>();

        // we delete the scripts we don't need
        Destroy(grid.GetComponent<GridSaver>());
        Destroy(grid.GetComponent<CaseNavigator>());
        Debug.Log("We have duplicated the canva & deleted the scripts of the grid child");

        // we rebuild the layout
        RebuildLayout(gridRect);

        // we freeze the grid
        grid.Freeze();
        Debug.Log("We have frozen the grid");

        // we hide the solutions
        if (!solutions) {grid.DeleteSolutions();}
        if (!solutions) {Debug.Log("solutions deleted");}

        return grid;
    }

    private void ApplyScaleAndMoveGridToTopLeftCorner(GridHandler grid, Canvas canva, int px_per_cell)
    {
        // we get the size of the cell
        RectTransform cellRect = grid.transform.GetChild(0).GetComponent<RectTransform>();
        if (cellRect != null)
        {
            Debug.Log("cell is : " + cellRect + " ("+cellRect.name+")" + " and its size is : " + cellRect.rect.size);
        }
        else
        {
            Debug.Log("Cell is null");
        }
        Vector2 cellScreenSize = GetRectScreenSize(cellRect, canva);
        Debug.Log("Cell appears on screen with those dimensions : " + cellScreenSize);

        // we apply a scale to the grid to reach the desired size
        int scale = (int)(px_per_cell / cellScreenSize.x);
        grid.transform.localScale = new Vector3(scale, scale, 1);
        Debug.Log("We applied the scale " + scale + " to the grid, so that the cell appears on screen with those dimensions : " + GetRectScreenSize(cellRect, canva));

        // we get the size of the spacing
        GridLayoutGroup gridLayoutGroup = grid.GetComponent<GridLayoutGroup>();
        int spacing = ((int)gridLayoutGroup.spacing.x) * scale;

        // we move the grid to put the first cell in the top left
        gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        gridRect.pivot = new Vector2(0, 1);
        gridRect.anchorMin = new Vector2(0, 1);
        gridRect.anchorMax = new Vector2(0, 1);
        gridRect.anchoredPosition = new Vector2(spacing, -spacing);
        spacing_offset_x = spacing;
        spacing_offset_y = -spacing;

        // we calculate the size of the grid
        export_width = grid.columns * (px_per_cell + spacing) + spacing;
        export_height = grid.rows * (px_per_cell + spacing) + spacing;
    }

    private Vector2 GetRectScreenSize(RectTransform rectTransform, Canvas canvas = null)
    {
        // Get the size of the rect in local space
        Vector2 localSize = rectTransform.rect.size;

        // Get the scale of the RectTransform
        Vector3 worldScale = rectTransform.lossyScale;

        // Multiply to get the world size
        Vector2 screenSize = new Vector2(
            localSize.x * worldScale.x,
            localSize.y * worldScale.y
        );

        // If the canvas has a scaler, consider its scale factor
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            float scaleFactor = canvas.scaleFactor;
            screenSize /= scaleFactor;
        }

        return screenSize;
    }

    private void CreateRenderTexture()
    {
        // Create the RenderTexture
        gridshot = new RenderTexture(export_width, export_height, 24);
        gridshot.Create();
    }





    // MAIN DURING EXPORTATION
    private IEnumerator _Screenshot(int width = 0, int height = 0)
    {
        //////////////////////////////////////Finally Take ScreenShot///////////////////////////////
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases();
        List<Canvas> canvases = new List<Canvas>();

        foreach (Transform child in gridRect)
        {
            Cell cell = child.GetComponent<Cell>();
            Canvas canvas = null;
            if (cell is MotherCell)
            {
                // we refresh all the canvas
                canvas = child.GetComponent<Canvas>();
                Debug.Log("Canvas found for def : " + cell.name + " - "+ canvas.gameObject.name);
            }
            else if (cell is Case)
            {
                // we refresh the case
                canvas = child.GetComponent<Case>().lines.GetComponent<Canvas>();
                Debug.Log("Canvas found for case " + cell.name + " - "+ canvas.gameObject.name);
            }
            if (!canvas)
            {
                Debug.Log("No canvas found for " + cell.name);
                continue;
            }
            canvases.Add(canvas);

            canvas.enabled = false;
            canvas.enabled = true;
        }

        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(width, height);
        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, Screen.height - height, width, height), 0, 0);
        screenImage.Apply();

        Debug.Log("Took screenshot, grid is at " + current_x + ", " + current_y);

        //Convert to png
        byte[] pngBytes = screenImage.EncodeToPNG();

        // string path = "/CanvasScreenShot.png";

        // FOR TESTING/DEBUGGING PURPOSES ONLY. COMMENT THIS
        /* string path = Application.persistentDataPath + "/gridhsot " + current_x + ", " + current_y + ".png";
        System.IO.File.WriteAllBytes(path, pngBytes);
        Debug.Log("partial gridshot is at " + path); */

        // we set the last shot size
        last_shot_width = width;
        last_shot_height = height;

        //Notify functions that are subscribed to this event that picture is taken then pass in image bytes as png
        OnScreenshotTaken?.Invoke(pngBytes);

        ///////////////////////////////////RE-ENABLE OBJECTS
        ///
    }

    private void OnPartOfScreenshotTaken(byte[] pngArray)
    {
        // we create a texture from the screenshot
        Texture2D screenImage = new Texture2D(last_shot_width, last_shot_height);
        screenImage.LoadImage(pngArray);

        // we add the part of the screenshot to the gridshot
        BlitTextureToRenderTexture(screenImage, gridshot, new Vector2(current_x,current_y), new Vector2(last_shot_width, last_shot_height));

        // we check if we have finished a row
        if (current_x + last_shot_width >= export_width)
        {
            // we check if we have finished the grid
            if (current_y + last_shot_height >= export_height)
            {
                // we take the final screenshot
                Texture2D finalImage = new Texture2D(export_width, export_height);
                RenderTexture.active = gridshot;
                finalImage.ReadPixels(new Rect(0, 0, export_width, export_height), 0, 0);
                finalImage.Apply();
                byte[] finalPngBytes = finalImage.EncodeToPNG();
                OnPictureTaken?.Invoke(finalPngBytes);
                return;
            }
            else
            {
                // we do the next row
                current_x = 0;
                current_y += last_shot_height;

                Debug.Log("Next row, grid is at " + current_x + ", " + current_y);
            }
        }
        else
        {
            // we do the next cell
            current_x += last_shot_width;

            Debug.Log("Next column, grid is at " + current_x + ", " + current_y);
        }

        // we move the grid
        gridRect.anchoredPosition = new Vector2(-current_x + spacing_offset_x, current_y + spacing_offset_y);

        Vector2Int size = CalculateNextShotSize();

        Debug.Log("Moving grid to " + (-current_x + spacing_offset_x) + ", " + (current_y + spacing_offset_y)
            + " for a screenshot of " + size.x + "x" + size.y);

        // we take the next screenshot
        StartCoroutine(_Screenshot(size.x, size.y));
    }


    // during exportation
    private Vector2Int CalculateNextShotSize()
    {
        // calculate the size of the next screenshot
        int width = export_width - current_x;
        int height = export_height - current_y;
        if (width > Screen.width) { width = Screen.width; }
        if (height > Screen.height) { height = Screen.height; }

        return new Vector2Int(width, height);
    }

    public void BlitTextureToRenderTexture(Texture2D texture, RenderTexture target, Vector2 position, Vector2 size)
    {
        // Set the RenderTexture as active
        RenderTexture.active = target;

        // Get the RenderTexture dimensions
        int target_width = target.width;
        int target_height = target.height;

        // Convert position and size to UV coordinates (0 to 1 range)
        Rect uvRect = new Rect(0, 1, 1, -1);

        // setup the GL context
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, target_width, 0, target_height); // Set up pixel-perfect rendering

        // get the rect where we will write on the target
        Rect target_rect = new Rect(position.x, target_height - position.y - size.y, size.x, size.y);

        // Draw the texture
        Graphics.DrawTexture(target_rect, texture, uvRect, 0,0,0,0);

        // Reset the active RenderTexture
        GL.PopMatrix();

        // Reset the active RenderTexture
        RenderTexture.active = null;
    }

}
