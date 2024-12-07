using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SFB;

using System.IO;
using System;

// PDFSharp
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;

public class ExportManager : MonoBehaviour
{

    [Header("Export Parameters")]
    public int px_per_cell = 100;
    public float minimum_margin = 28.35f; // 1 cm
    public bool show_solutions = false;

    [Header("Components")]
    public CanvasScreenShot screenshoter;
    public GridHandler grid;
    public ExportWindow export_window;

    [Header("GameObjects to disable during screenshot")]
    public GameObject[] to_disable;

    [Header("PDF Parameters")]
    // public string font_path = "Assets/Fonts/arial.ttf";
    public string font_name = "Arial";
    public XFontStyleEx font_style = XFontStyleEx.Regular;
    public int font_size = 20;
    public XFont pdf_font;

    // START
    void Start()
    {
        // we get the components
        screenshoter = GetComponent<CanvasScreenShot>();

        // we subscribe to the event
        CanvasScreenShot.OnPictureTaken += OnScreenshotTaken;

        // we launch the pdf library
        if (Capabilities.Build.IsCoreBuild)
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();

        // we get the font
        pdf_font = new XFont(font_name, 20, font_style);
    }

    void Reset()
    {
        show_solutions = false;
    }

    // EXPORT GRID
    public void ExportGrid(bool leave_solutions = true)
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

        // we save the solutions
        show_solutions = leave_solutions;

        // we take a screenshot
        screenshoter.Gridshot(canva, px_per_cell, leave_solutions);

    }

    // ON SCREENSHOT TAKEN
    void OnScreenshotTaken(byte[] pngArray)
    {
        Debug.Log("Screenshot taken");

        // we export to pdf
        SaveToPDF(pngArray);

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

        screenshoter.Reset();
    }


    // CONVERT TO PDF
    void SaveToPDF(byte[] png)
    {
        // we get the name
        string grid_name = grid.grid_name.Replace(".json", "");
        grid_name += show_solutions ? " (soluces)" : "";

        // we get the path
        string path = StandaloneFileBrowser.SaveFilePanel("Exporter la grille", "", grid_name, "pdf");
        if (path == "") { return; }

        // we create a new PDF document
        var pdf = new PdfDocument();
        pdf.Info.Title = grid_name;
        var page = pdf.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Define A4 page size in points
        const double A4Width = 595.0;  // Width in points (8.27 inches at 72 DPI)
        const double A4Height = 842.0; // Height in points (11.69 inches at 72 DPI)

        // Set the page size to A4
        page.Width = new XUnit(A4Width);
        page.Height = new XUnit(A4Height);

        // Calculate drawable area (A4 size minus margins)
        double drawableWidth = A4Width - 2 * minimum_margin;
        double drawableHeight = A4Height - 2 * minimum_margin;

        double marginX = 0;
        double marginY = 0;

        // Step 3: Load the image from byte array
        using (MemoryStream originalStream = new MemoryStream(png))
        {
            // Create a new MemoryStream that allows buffer access
            using (MemoryStream compatibleStream = new MemoryStream())
            {
                originalStream.CopyTo(compatibleStream);    
                compatibleStream.Position = 0; // Reset the position

                XImage image = XImage.FromStream(compatibleStream);

                // Calculate image placement
                double imageWidth = image.PointWidth;  // Image width in points
                double imageHeight = image.PointHeight; // Image height in points

                // Calculate scaling to fit within the drawable area
                double scale = Math.Min(drawableWidth / imageWidth, drawableHeight / imageHeight);

                // New image dimensions after scaling
                double scaledWidth = imageWidth * scale;
                double scaledHeight = imageHeight * scale;

                // Calculate position within drawable area, maintaining minimum margins
                marginX = minimum_margin + (drawableWidth - scaledWidth) / 2;
                marginY = minimum_margin + (drawableHeight - scaledHeight) / 2;

                // Draw the image on the page, centered
                gfx.DrawImage(image, marginX, marginY, scaledWidth, scaledHeight);

                // Dispose of the image
                image.Dispose();
                compatibleStream.Dispose();
            }
            originalStream.Dispose();
        }

        // we write the name of the grid (if needed)
        if (export_window.write_name_toggle.On)
        {
            // XFont font = new XFont("Arial", 12);
            gfx.DrawString(grid_name.ToUpper(), pdf_font, XBrushes.Black, new XRect(0, 0, A4Width, marginY), XStringFormats.Center);
        }

        // Step 6: Save the PDF to the specified output path
        pdf.Save(path);

        // Step 7: Optionally, dispose of the PDF document
        pdf.Dispose();

        Debug.Log($"PDF created successfully at {path}");

        Reset();
    }
}