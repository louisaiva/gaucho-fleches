using System.Collections.Generic;
using UnityEngine;

public class GridSaver : MonoBehaviour
{
    // parses a grid into json from a grid handler
    // here's an example of the json format:
    /*
    {
    "grid": {
        "width": 10,
        "height": 10
    },

    "cells":{
        "0_0": {
            "type": "case",
            "content": "A"
        },
        "0_1": {
            "type": "def",
            "content": "B",
            "arrows": {
                "right": true,
                "down": true,
                "right_down": false,
                "down_right": false
            }
        }
    }
    }*/

    [Header("Saving Loading Settings")]
    public string base_name = "grille n°";
    public bool debug = true;


    [Header("References")]
    public GridHandler grid;
    public HelpHandler helper;

    // Start is called before the first frame update
    void Start()
    {
        // we get the grid handler
        grid = GetComponent<GridHandler>();

        // we get the helper
        helper = GameObject.Find("ui/help").GetComponent<HelpHandler>();
    }

    // SAVING
    public string ParseGrid()
    {
        string json = "{\n\t";
        json += "\"grid\": {\n\t\t";
        json += "\"width\": " + grid.columns + ",\n\t\t";
        json += "\"height\": " + grid.rows + "\n\t";
        json += "},\n\t";

        json += "\"cells\": {\n\t\t";

        for (int x = 0; x < grid.columns; x++)
        {
            for (int y = 0; y < grid.rows; y++)
            {
                Cell cell = grid.GetCell(x, y);
                if (cell == null) { continue; }

                // get the type
                bool is_case = cell is Case;

                string cell_id = x + "_" + y;

                json += "\"" + cell_id + "\": {\n\t\t\t";
                json += "\"type\": \"" + (is_case ? "case" : "def") + "\",\n\t\t\t";
                json += "\"content\": \"" + cell.GetContent() + "\",\n\t\t\t";

                if (is_case)
                {
                    json += "\"expanded\": \"" + (cell as Case).GetExpandedLines() + "\"";
                }
                else
                {
                    json += "\"horizontal\": \"" + (cell as MotherCell).GetDefHorizontals() + "\"";
                }

                json += "\n\t\t}";

                if (x != grid.columns - 1 || y != grid.rows - 1)
                {
                    json += ",\n\t\t";
                }
            }
        }

        json += "\n\t}\n}";

        return json;
    }
    public bool SaveGridToFile(string path)
    {
        // on regarde si le fichier existe
        if (System.IO.File.Exists(path))
        {
            // on vérifie si le fichier est accessible
            bool is_accessible = false;
            float start_time = Time.time;
            float max_time = 5f;
            while (!is_accessible)
            {
                // on vérifie si le temps est écoulé
                if (Time.time - start_time > max_time)
                {
                    LogWarning("(GridSaver - SaveGrid) Timeout while accessing the file: " + path);
                    return false;
                }

                // on essaie d'ouvrir le fichier
                try
                {
                    System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite).Close();
                    is_accessible = true;
                }
                catch (System.Exception e)
                {
                    LogWarning("(GridSaver - SaveGrid) Error while accessing the file: " + e.Message);
                }

                // on attend un peu
                System.Threading.Thread.Sleep(100);
            }
        }

        // on sauvegarde la grille
        System.IO.StreamWriter file = new System.IO.StreamWriter(path);
        
        // on écrit le json
        file.WriteLine(ParseGrid());

        // on ferme le fichier
        file.Close();

        // log
        Log("Grid saved to file: " + path);
        helper.RegisterLog("Grille sauvegardée dans le fichier: " + GetNameFromPath(path));

        return true;
    }

    // LOADING
    public void LoadGridFromFile(string path)
    {
        // on vérifie si le fichier existe
        if (!System.IO.File.Exists(path))
        {
            LogError("File does not exist: " + path);
            return;
        }

        // on essaie de lire le fichier
        bool is_reading = false;
        float start_time = Time.time;
        float max_time = 5f;
        while (!is_reading)
        {

            // on vérifie si le temps est écoulé
            if (Time.time - start_time > max_time)
            {
                LogWarning("Timeout while reading the file: " + path);
                return;
            }

            // on essaie de lire le fichier
            try
            {
                System.IO.File.ReadAllLines(path);
                is_reading = true;
            }
            catch (System.Exception e)
            {
                LogWarning("Error while reading the file: " + e.Message);
            }

            // on attend un peu
            System.Threading.Thread.Sleep(100);
        }

        // on prépare les variables
        int width = 0;
        // int x,y;
        bool is_case = false;

        // on lit le fichier
        string[] lines = System.IO.File.ReadAllLines(path);
        List<string> current_path_in_json = new List<string>();
        foreach (string line in lines)
        {
            // we check how many /t do we have at the beginning of the line
            int nb_tabs = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\t') { nb_tabs++; }
                else { break; }
            }
            if (nb_tabs == 0) { continue; }

            // on récupère où on en est dans le json
            string[] parts = line.Split(':');
            if (parts.Length < 2) { continue; }
            string parent = parts[0].Split('"')[1];

            // on regarde si on doit remonter dans le json
            if (nb_tabs < current_path_in_json.Count)
            {
                current_path_in_json.RemoveRange(nb_tabs, current_path_in_json.Count - nb_tabs);
                current_path_in_json[nb_tabs - 1] = parent;
            }
            else if (nb_tabs > current_path_in_json.Count)
            {
                current_path_in_json.Add(parent);
            }
            else if (nb_tabs == current_path_in_json.Count)
            {
                current_path_in_json[nb_tabs - 1] = parent;
            }

            Log("Current path in json: " + string.Join(" -> ", current_path_in_json));

            // on récupère ce qu'on a besoin
            if (current_path_in_json[0] == "grid" && current_path_in_json.Count == 2)
            {
                if (current_path_in_json[1] == "width")
                {
                    width = int.Parse(parts[1].Replace(",", "").Replace(" ", ""));
                    Log("Width: " + width);
                }
                else if (current_path_in_json[1] == "height")
                {
                    int height = int.Parse(parts[1].Replace(",", "").Replace(" ", ""));
                    Log("Height: " + height);
                    grid.GenerateEmptyGrid(width, height);
                }
            }
            else if (current_path_in_json[0] == "cells" && current_path_in_json.Count == 3)
            {
                string[] cell_id = current_path_in_json[1].Split('_');
                int x = int.Parse(cell_id[0]);
                int y = int.Parse(cell_id[1]);
                Log("Cell: " + x + "_" + y);

                if (current_path_in_json[2] == "type")
                {
                    string type = parts[1].Replace(",", "").Replace(" ", "").Replace("\"", "");
                    Log("Type: " + type + " (" + x + "_" + y + ")");

                    // on regarde si c'est une case ou une définition
                    if (type == "case") { is_case = true; }
                    else if (type == "def") { is_case = false; }

                    // we check the type of the existing cell
                    Cell existing_cell = grid.GetCell(x, y);
                    if (existing_cell == null)
                    {
                        LogWarning("(GridSaver - Loading - type) Cell does not exist: " + x + "_" + y);
                        continue;
                    }
                    
                    // we switch the cell type
                    if (is_case == existing_cell is Case) { continue; }
                    grid.SwitchCaseDef(existing_cell, false, false);
                }
                else if (current_path_in_json[2] == "content")
                {
                    string content = parts[1].Replace(",", "").Replace(" ", "").Replace("\"", "");
                    Log("Content: " + content + " (" + x + "_" + y + ")");

                    // on check si la case existe
                    Cell existing_cell = grid.GetCell(x, y);
                    if (existing_cell == null)
                    {
                        LogWarning("(GridSaver - Loading - content) Cell does not exist: " + x + "_" + y);
                        continue;
                    }
                    
                    // we set the content
                    existing_cell.SetContent(content);
                }
                else if (current_path_in_json[2] == "expanded")
                {
                    string expanded = parts[1].Replace(",", "").Replace(" ", "").Replace("\"", "");
                    Log("Expanded: " + expanded);

                    // on check si la case existe
                    Cell existing_cell = grid.GetCell(x, y);
                    if (existing_cell == null)
                    {
                        LogWarning("(GridSaver - Loading - lines) Cell does not exist: " + x + "_" + y);
                        continue;
                    }

                    // on set les lignes
                    existing_cell.SetExpandedLines(expanded);
                }
                else if (current_path_in_json[2] == "horizontal")
                {
                    string horizontal = parts[1].Replace(",", "").Replace(" ", "").Replace("\"", "");
                    Log("Horizontal: " + horizontal);

                    // on check si la case existe
                    Cell existing_cell = grid.GetCell(x, y);
                    if (existing_cell == null)
                    {
                        LogWarning("(GridSaver - Loading - horizontal) Cell does not exist: " + x + "_" + y);
                        continue;
                    }

                    // on set les horizontales
                    (existing_cell as MotherCell).SetDefHorizontals(horizontal);
                }
            }
        }


        // log
        Log("Grid loaded from file: " + path);
    }

    // OTHER METHODS
    public string GetNameFromPath(string path)
    {
        string[] path_parts = path.Split('\\').Length > 1 ? path.Split('\\') : path.Split('/');
        return path_parts[path_parts.Length - 1];
    }
    public string GenerateGridName(string folder_path)
    {
        // we check if the folder path is empty
        if (folder_path == "") { return base_name + "1"; }

        // we get the last grid number
        int last_grid_number = 0;
        string[] grid_files = System.IO.Directory.GetFiles(folder_path, "*.json");
        foreach (string grid_file in grid_files)
        {
            string grid_name = System.IO.Path.GetFileNameWithoutExtension(grid_file);

            // on regarde si le nom de la grille commence par "grille n°"
            if (!grid_name.StartsWith(base_name)) { continue; }
            string grid_number = grid_name.Replace(base_name, "");
            int grid_number_int = int.Parse(grid_number);
            if (grid_number_int > last_grid_number)
            {
                last_grid_number = grid_number_int;
            }
        }

        return base_name + (last_grid_number + 1);
    }


    // LOGGING DEBUG
    void Log(string message)
    {
        if (debug) { Debug.Log(message); }
    }
    void LogWarning(string message)
    {
        if (debug) { Debug.LogWarning(message); }
    }
    void LogError(string message)
    {
        if (debug) { Debug.LogError(message); }
    }

}