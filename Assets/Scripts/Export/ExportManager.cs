using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SFB;

public class ExportManager : MonoBehaviour
{

    [Header("Export Parameters")]
    public int px_per_cell = 100;

    [Header("Components")]
    public CanvasScreenShot screenshoter;
    public GridHandler grid;

    [Header("GameObjects to disable during screenshot")]
    public GameObject[] to_disable;

    // START
    void Start()
    {
        // we get the components
        screenshoter = GetComponent<CanvasScreenShot>();

        // we subscribe to the event
        CanvasScreenShot.OnPictureTaken += OnScreenshotTaken;
    }

    // ON SCREENSHOT TAKEN
    void OnScreenshotTaken(byte[] pngArray)
    {
        Debug.Log("Screenshot taken");

        // we get the path
        string grid_name = grid.grid_name.Replace(".json", "");
        string path = StandaloneFileBrowser.SaveFilePanel("Exporter la grille", "", grid_name, "png");
        if (path == "") { return; }

        // we save the picture to a file
        // string path = "/CanvasScreenShot.png";
        System.IO.File.WriteAllBytes(path, pngArray);
        Debug.Log("Exported grid at " + path);

        // we delete all exporter children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // we re-enable the gameobjects
        foreach (GameObject go in to_disable)
        {
            go.SetActive(true);
        }
    }

    public void ExportGrid()
    {
        // we get the target canvas
        Canvas target_canvas = grid.transform.parent.GetComponent<Canvas>();

        // we disable the gameobjects
        foreach (GameObject go in to_disable)
        {
            go.SetActive(false);
        }

        // we take a screenshot
        screenshoter.Gridshot(target_canvas, px_per_cell);

    }

}