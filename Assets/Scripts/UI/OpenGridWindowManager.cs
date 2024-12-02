using SFB;
using UnityEngine;
using TMPro;

public class OpenGridWindowManager : MonoBehaviour
{
    [Header("Components & References")]
    public GridHandler grid_handler;
    public GameObject open_grid_window;
    public GameObject grid_window;
    public GridBodyWindowManager grid_body_window_manager;
    public TextMeshProUGUI grid_size_text;

    // START
    void Start()
    {
        // on désactive le grid handler
        grid_window.SetActive(false);

        // on active la fenetre d'ouverture de grille
        open_grid_window.SetActive(true);
    }

    // GRID OPENING / CLOSING
    public void OpenGridFile()
    {
        // we open a window dialog to select a file
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Ouvrir une grille", "", "json", false);
        if (paths.Length == 0)
        {
            Debug.LogWarning("No file selected");
            return;
        }
        string path = paths[0];

        Debug.Log(path);

        // we check if the path is empty
        if (path == "")
        {
            Debug.LogWarning("No file selected");
            return;
        }

        // on désactive la fenetre d'ouverture de grille
        open_grid_window.SetActive(false);

        // on active le grid handler
        grid_window.SetActive(true);

        // we ouvre le fichier
        grid_handler.OpenGridFromFile(path);

        Debug.Log("Grid opened from file: " + path);
    }

    public void CreateNewGrid()
    {
        // on désactive la fenetre d'ouverture de grille
        open_grid_window.SetActive(false);

        // on active le grid handler
        grid_window.SetActive(true);

        // on récupère les dimensions de la grille
        string grid_size = grid_size_text.text;
        grid_size = grid_size.Remove(grid_size.Length - 1).Replace(" ", "");
        string[] grid_size_parts = grid_size.Split('x');
        int columns = int.Parse(grid_size_parts[0]);
        int rows = int.Parse(grid_size_parts[1]);

        // we generate a new grid
        string grid_name = grid_handler.GenerateEmptyGrid(columns, rows);

        Debug.Log("New grid created: " + grid_name);
    }

    public void CloseGrid()
    {
        string grid_name = grid_handler.grid_name;

        grid_handler.Clear();

        // on désactive le grid handler
        grid_window.SetActive(false);

        // on reset le zoom du grid body window manager
        grid_body_window_manager.ResetZoom();

        // on active la fenetre d'ouverture de grille
        open_grid_window.SetActive(true);

        Debug.Log("Grid " + grid_name + " closed");
    }
}