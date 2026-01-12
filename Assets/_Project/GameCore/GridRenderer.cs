using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GridRenderer : MonoBehaviour
{
    [SerializeField] private Material lineMaterial;

    private GridManager gridManager;
    private Color gridColor = new Color(0.8f, 0.8f, 0.8f);
    private float lineWidth = 0.02f;
    private GameObject parentLines;

    void Start()
    {
        if (gridManager == null) gridManager = FindObjectOfType<GridManager>();
        DrawGrid();
    }

    /// <summary>
    /// Метод для постройки сетки
    /// </summary>
    public void DrawGrid()
    {
        if (gridManager == null || gridManager.UnityGrid == null) return;

        if (parentLines != null) DestroyImmediate(parentLines);
        parentLines = new GameObject("GridLines");
        parentLines.transform.SetParent(transform, false);
        Material mat = lineMaterial ?? new Material(Shader.Find("Sprites/Default"));
        mat.color = gridColor;
        int cols = gridManager.Columns;
        int rows = gridManager.Rows;
        var g = gridManager.UnityGrid;
        Vector3 cellSize = g.cellSize;

        // вертикальные линии
        for (int x = 0; x <= cols; x++)
        {
            GameObject go = new GameObject("GridLine_V_" + x);
            go.transform.SetParent(parentLines.transform, false);
            LineRenderer lr = go.AddComponent<LineRenderer>();
            lr.material = mat;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            Vector3Int startCell = new Vector3Int(x, 0, 0);
            Vector3 start = g.CellToWorld(startCell);
            Vector3Int endCell = new Vector3Int(x, rows, 0);
            Vector3 end = g.CellToWorld(endCell);

            // подвинем линии по центру клетки, чтобы совпадало с CellToWorld(center)
            start += new Vector3(cellSize.x * 0.0f, cellSize.y * 0.0f, 0f);
            end += new Vector3(cellSize.x * 0.0f, 0f, 0f);
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.sortingOrder = 100;
        }

        // горизонтальные линии
        for (int y = 0; y <= rows; y++)
        {
            GameObject go = new GameObject("GridLine_H_" + y);
            go.transform.SetParent(parentLines.transform, false);
            LineRenderer lr = go.AddComponent<LineRenderer>();
            lr.material = mat;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            Vector3Int startCell = new Vector3Int(0, y, 0);
            Vector3 start = g.CellToWorld(startCell);
            Vector3Int endCell = new Vector3Int(cols, y, 0);
            Vector3 end = g.CellToWorld(endCell);
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.sortingOrder = 100;
        }
    }
}