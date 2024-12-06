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
        if (path != "")
        {
            // we save the picture to a file
            System.IO.File.WriteAllBytes(path, pngArray);
            Debug.Log("Exported grid at " + path);
        }

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

    public void ExportGrid(bool leave_solutions=true)
    {
        // we reset the screenshoter
        screenshoter.Reset();

        Debug.Log("Exporting grid " + (leave_solutions ? "with" : "without") + " solutions");

        // we get the target canvas
        Canvas target_canvas = grid.transform.parent.GetComponent<Canvas>();

        //Reset the position so that both UI will be in the-same place if we make the duplicate a child
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        // we duplicate the canva & set the new parent
        GameObject canva_go = Instantiate(target_canvas.gameObject);
        canva_go.transform.SetParent(gameObject.transform);

        // we disable the gameobjects
        foreach (GameObject go in to_disable)
        {
            go.SetActive(false);
        }

        // we change the parameters of the duplicated canva
        Canvas canva = canva_go.GetComponent<Canvas>();
        canva.renderMode = RenderMode.ScreenSpaceOverlay;


        // we take a screenshot
        screenshoter.Gridshot(canva, px_per_cell, leave_solutions);

    }

}