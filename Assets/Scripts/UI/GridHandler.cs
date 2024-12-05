using SFB;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GridHandler : MonoBehaviour
{
    
    [Header("Global grid settings")]
    public int columns = 13;
    public int rows = 17;
    public GameObject case_prefab;
    public GameObject def_prefab;
    // public GameObject right_def_prefab;

    [Header("Opened grid")]
    public Cell[,] grid;
    public string grid_name = "";
    public string grid_folder_path = "";

    [Header("Components & References")]
    public GridLayoutGroup grid_layout_group;
    public TextMeshProUGUI grid_name_text;
    public GridSaver loader;

    [Header("Memory")]
    public Dictionary<Vector2Int, string> case_memory = new Dictionary<Vector2Int, string>();
    public Dictionary<Vector2Int, string> def_memory = new Dictionary<Vector2Int, string>();

    // START
    void Start()
    {
        // we get the components
        grid_layout_group = GetComponent<GridLayoutGroup>();
        loader = GetComponent<GridSaver>();
    }

    void OnEnable()
    {
        // we get the components
        grid_layout_group = GetComponent<GridLayoutGroup>();
    }

    // GRID GENERATION
    public string GenerateEmptyGrid(int columns=13, int rows=17)
    {
        Clear();
        if (grid_layout_group == null) { OnEnable(); }

        // we set the grid size
        this.columns = columns;
        this.rows = rows;

        // we set the grid layout group settings
        grid_layout_group.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid_layout_group.constraintCount = columns;

        // we create the grid
        grid = new Cell[columns, rows];
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (x == 0 && y%2 == 0)
                {
                    CreateDef(x, y);
                }
                else if (y == 0 && x%2 == 0)
                {
                    CreateDef(x, y);
                }
                else
                {
                    CreateCase(x, y);
                }
            }
        }

        // we set the grid name
        grid_name = loader.GenerateGridName(grid_folder_path);
        grid_name_text.text = grid_name.Replace(".json", "");

        // we log
        Debug.Log("Empty grid generated with " + columns + " columns & " + rows + " rows");

        return grid_name;
    }
    public void OpenGridFromFile(string path)
    {
        // we load the grid
        loader.LoadGridFromFile(path);

        // we set the grid name
        string[] path_parts = path.Split('\\').Length > 1 ? path.Split('\\') : path.Split('/');
        grid_name = path_parts[path_parts.Length - 1];
        grid_folder_path = path.Replace(grid_name, "");
        grid_name_text.text = grid_name.Replace(".json", "");
    }

    // GRID OPERATIONS
    public void VoidSave()
    {
        Save();
    }
    public bool Save()
    {
        string path = grid_folder_path + grid_name;
        Debug.Log("Saving grid to " + path);

        // we check if we have a folder_path & a grid_name
        if (grid_folder_path == "" || grid_name == "" || !System.IO.File.Exists(path))
        {
            // we get the new path
            string new_path = StandaloneFileBrowser.SaveFilePanel("Sauvegarder la grille", "", grid_name, "json");
            if (new_path == "") { return false; }

            // we get the new name
            string new_name = loader.GetNameFromPath(new_path);
            grid_name = new_name;
            grid_folder_path = new_path.Replace(new_name, "");
            grid_name_text.text = grid_name.Replace(".json", "");

            // we change the path
            path = new_path;
        }

        // on sauvegarde la grille
        bool saved = loader.SaveGridToFile(path);

        Debug.Log("Grid saved to " + path);
        return saved;
    }
    public void Clear()
    {
        // we destroy all the cases
        foreach (Transform case_instance in transform)
        {
            Destroy(case_instance.gameObject);
        }

        // we reset the grid
        grid = new Cell[0, 0];
    }

    // CELLS CREATION
    public Cell CreateCell(int x, int y, GameObject prefab, bool replace_if_exists = true)
    {
        // we check if the case already exists
        if (!replace_if_exists && grid[x, y] != null) { return null; }
        

        // we get the index with x & y
        int index = x * rows + y;

        // we check if their is already a cell at this position
        Cell existing_cell = grid[x, y];
        if (existing_cell != null)
        {
            // we get the index of the existing cell
            index = existing_cell.transform.GetSiblingIndex();

            // we destroy the existing cell
            Destroy(existing_cell.gameObject);
        }

        // we check if the index is valid
        if (index >= transform.childCount) { index = transform.childCount; }
        


        // we create the cell
        GameObject cell_instance = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
        cell_instance.transform.SetParent(this.transform);
        cell_instance.transform.SetSiblingIndex(index);
        cell_instance.name = "Cell_" + x + "_" + y;
        cell_instance.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        // we get the cell component
        Cell cell_component = cell_instance.GetComponent<Cell>();
        cell_component.SetGridPosition(x, y);

        // we add the cell to the grid
        grid[x, y] = cell_component;

        return cell_component;
    }
    public Case CreateCase(int x, int y, string content = "", bool replace_if_exists = true)
    {
        // we create the case
        Case case_instance = CreateCell(x, y, case_prefab, replace_if_exists) as Case;
        if (case_instance == null) { return null; }
        case_instance.name = "Case_" + x + "_" + y;

        // we set the content
        case_instance.SetContent(content);

        return case_instance;
    }
    public MotherCell CreateDef(int x, int y, string content = "", bool replace_if_exists = true)
    {
        // we create the case
        MotherCell def_instance = CreateCell(x, y, def_prefab, replace_if_exists) as MotherCell;
        if (def_instance == null) { return null; }
        def_instance.name = "Def_" + x + "_" + y;

        // we set the content
        if (content != "" ) { def_instance.SetContent(content); }
        else
        {
            def_instance.SetContent("%_%"); // default content -> "" & ""
        }

        // we reset the lines on the up & left
        GetCell(x, y - 1)?.ResetLine(false);
        GetCell(x - 1, y)?.ResetLine(true);

        return def_instance;
    }

    // CELLS MANAGEMENT
    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= columns || y < 0 || y >= rows) { return null; }
        Cell target = grid[x, y];

        // we return the case
        return target;
    }
    public Cell SwitchCaseDef(Cell target, bool remember_content = true, bool restore_content = true)
    {
        // we check if the case is already defined
        if (target == null) { return null; }
        
        // Debug.Log("Switching " + target.name + " to " + (target is Case ? "Definition" : "Case"));

        // we remember the content
        if (remember_content) { RememberCellContent(target); }

        // we want to know if the Cell is a Case or a Defintion
        bool is_case = target is Case;

        // we create the new case
        Cell new_cell = is_case ? CreateDef(target.x, target.y) : CreateCase(target.x, target.y);

        // we restore the content
        if (restore_content) { RestoreCellContent(new_cell); }

        // if we just created a def we force it to update
        if (is_case) { new_cell.ForceUpdate(); }

        // we return the new case
        return new_cell;
    }


    // MEMORY MANAGEMENT
    public void RememberCellContent(Cell cell)
    {
        // we check if the cell is a case or a definition
        bool is_case = cell is Case;

        // we get the content
        string content = cell.GetContent();

        // we check if the content is empty
        if (content == "") { return; }

        // Debug.Log("Remembering " + cell.name + "("+ cell.x +"/"+ cell.y +") : " + content);

        // we create the memory if it doesn't exist
        if (!HasMemory(cell))
        {
            // we add the content to the memory
            if (is_case)
            {
                case_memory.Add(new Vector2Int(cell.x, cell.y), content);
            }
            else
            {
                def_memory.Add(new Vector2Int(cell.x, cell.y), content);
            }
            return;
        }

        // or we just replace it in the memory
        if (is_case)
        {
            case_memory[new Vector2Int(cell.x, cell.y)] = content;
        }
        else
        {
            def_memory[new Vector2Int(cell.x, cell.y)] = content;
        }
    }
    public void RestoreCellContent(Cell cell)
    {
        // we check if we have the memory
        if (!HasMemory(cell)) { return; }

        // we check if the cell is a case or a definition
        bool is_case = cell is Case;

        // we get the content
        string content = is_case ? case_memory[new Vector2Int(cell.x, cell.y)] : def_memory[new Vector2Int(cell.x, cell.y)];

        // we set the content
        cell.SetContent(content);

        // we remove the content from the memory
        if (is_case) { case_memory.Remove(new Vector2Int(cell.x, cell.y)); }
        else { def_memory.Remove(new Vector2Int(cell.x, cell.y)); }
    }
    public bool HasMemory(Cell cell)
    {
        // we check if the cell is a case or a definition
        bool is_case = cell is Case;

        // we check if the memory contains the key
        return is_case ? case_memory.ContainsKey(new Vector2Int(cell.x, cell.y)) : def_memory.ContainsKey(new Vector2Int(cell.x, cell.y));
    }

}