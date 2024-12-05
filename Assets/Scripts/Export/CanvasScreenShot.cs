using System.Collections;
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
    public int beggining_x = 0;
    public int beggining_y = 0;
    public int grid_max_x = 0;
    public int grid_max_y = 0;

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
    public void Gridshot(Canvas input_canva, int px_per_cell)
    {
        //Reset the position so that both UI will be in the-same place if we make the duplicate a child
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        // we duplicate the canva & set the new parent
        GameObject canva_go = Instantiate(input_canva.gameObject);
        canva_go.transform.SetParent(gameObject.transform);

        // we change the parameters of the duplicated canva
        Canvas canva = canva_go.GetComponent<Canvas>();
        canva.renderMode = RenderMode.ScreenSpaceOverlay;


        // we get the grid
        GridHandler grid = canva_go.transform.GetChild(0).GetComponent<GridHandler>();
        gridRect = grid.GetComponent<RectTransform>();


        // we get the size of the cell
        RectTransform rectTransform = grid.transform.GetChild(0).GetComponent<RectTransform>();
        Vector2 cellScreenSize = GetScreenSize(rectTransform, canva);
        Debug.Log("Cell Screen Size: " + cellScreenSize);

        // we apply a scale to the grid to reach the desired size
        int scale = (int)(px_per_cell / cellScreenSize.x);
        grid.transform.localScale = new Vector3(scale, scale, 1);

        // we get the size of the spacing
        GridLayoutGroup gridLayoutGroup = grid.GetComponent<GridLayoutGroup>();
        int spacing = ((int) gridLayoutGroup.spacing.x) * scale;

        // we move the grid to put the first cell in the top left
        gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        gridRect.pivot = new Vector2(0, 1);
        gridRect.anchorMin = new Vector2(0, 1);
        gridRect.anchorMax = new Vector2(0, 1);
        gridRect.anchoredPosition = new Vector2(spacing, -spacing);
        beggining_x = spacing;
        beggining_y = -spacing;

        // we calculate the size of the grid
        grid_max_x = grid.columns * (px_per_cell + spacing) + spacing;
        grid_max_y = grid.rows * (px_per_cell + spacing) + spacing;

        // we create the render texture
        int texture_width_x = 0;
        while (texture_width_x < grid_max_x)
        {
            texture_width_x += Screen.width;
        }
        int texture_width_y = 0;
        while (texture_width_y < grid_max_y)
        {
            texture_width_y += Screen.height;
        }
        gridshot = new RenderTexture(texture_width_x, texture_width_y, 24);


        // we take the screenshot
        StartCoroutine(_Screenshot(Screen.width, Screen.height));

        // we delete the duplicated canva
        // Destroy(canva_go);
    }

    private IEnumerator _Screenshot(int width = 0, int height = 0)
    {
        //////////////////////////////////////Finally Take ScreenShot///////////////////////////////
        yield return new WaitForEndOfFrame();
        Texture2D screenImage = new Texture2D(width, height);
        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, width, height), 0, Screen.height - height);
        screenImage.Apply();

        Debug.Log("Took screenshot, grid is at " + current_x + ", " + current_y);

        //Convert to png
        byte[] pngBytes = screenImage.EncodeToPNG();

        // string path = "/CanvasScreenShot.png";

        // FOR TESTING/DEBUGGING PURPOSES ONLY. COMMENT THIS
        string path = Application.persistentDataPath + "/gridhsot " + current_x + ", " + current_y + ".png";
        System.IO.File.WriteAllBytes(path, pngBytes);
        Debug.Log("partial gridshot is at "+path);

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

        // we calculate the position of the screenshot
        /* int x = current_x + beggining_x;
        int y = current_y + beggining_y; 

        // we add the part of the screenshot to the gridshot
        BlitTextureToRenderTexture(screenImage, gridshot,
            new Vector2(x, y), new Vector2(last_shot_width, last_shot_height)); */

        // we check if we have finished a row
        if (current_x + last_shot_width >= grid_max_x)
        {
            // we check if we have finished the grid
            if (current_y + last_shot_height >= grid_max_y)
            {
                // we take the final screenshot
                Texture2D finalImage = new Texture2D(grid_max_x, grid_max_y);
                RenderTexture.active = gridshot;
                finalImage.ReadPixels(new Rect(0, 0, grid_max_x, grid_max_y), 0, 0);
                finalImage.Apply();
                byte[] finalPngBytes = finalImage.EncodeToPNG();
                // OnPictureTaken?.Invoke(finalPngBytes);
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
        gridRect.anchoredPosition = new Vector2(-current_x + beggining_x, current_y + beggining_y);

        // calculate the size of the next screenshot
        int width = grid_max_x - current_x;
        int height = grid_max_y - current_y;
        if (width > Screen.width) { width = Screen.width; }
        if (height > Screen.height) { height = Screen.height; }

        Debug.Log("Moving grid to " + (-current_x + beggining_x) + ", " + (current_y + beggining_y)
            + " for a screenshot of " + width + "x" + height);

        // we take the next screenshot
        StartCoroutine(_Screenshot(width, height));
    }

    // Utilities

    public Vector2 GetScreenSize(RectTransform rectTransform, Canvas canvas = null)
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

    public void BlitTextureToRenderTexture(Texture2D texture, RenderTexture target, Vector2 position, Vector2 size)
    {
        // Set the RenderTexture as active
        RenderTexture.active = target;

        // Get the RenderTexture dimensions
        int rtWidth = target.width;
        int rtHeight = target.height;

        // Convert position and size to UV coordinates (0 to 1 range)
        Rect uvRect = new Rect(
            position.x / rtWidth,
            1 - (position.y + size.y) / rtHeight,  // Flip Y because UVs are bottom-left origin
            size.x / rtWidth,
            size.y / rtHeight
        );

        // Draw the texture to the RenderTexture
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rtWidth, 0, rtHeight); // Set up pixel-perfect rendering
        Graphics.DrawTexture(
            new Rect(position.x, rtHeight - position.y - size.y, size.x, size.y),
            texture,
            uvRect,
            0, 0, 0, 0
        );
        GL.PopMatrix();

        // Reset the active RenderTexture
        RenderTexture.active = null;
    }
}
