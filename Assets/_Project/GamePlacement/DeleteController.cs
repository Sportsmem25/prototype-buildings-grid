using UnityEngine;

public class DeleteController : MonoBehaviour
{
    [SerializeField] private GameObject deleteOverlay;
    private GridManager grid;
    private SaveService saveService;
    private bool isActive = false;

    private void Awake()
    {
        if (deleteOverlay != null)
            deleteOverlay.SetActive(false);
    }

    private void Start()
    {
        grid = FindObjectOfType<GridManager>();
        saveService = FindObjectOfType<SaveService>();
    }

    private void Update()
    {
        if(!isActive) return;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int cell = grid.WorldToCell(mouse);

        if (Input.GetMouseButtonDown(0))
        {
            // проверка есть ли здание в этой клетке
            var found = saveService.FindBuildingAt(cell.x, cell.y);
            if (found != null)
            {
                // очистка сетки
                grid.SetOccupied(found.x, found.y, found.w, found.h, false);

                // удаление объекта в сцене (по имени/координатам)
                GameObject go = GameObject.Find(found.instanceName);
                if (go != null) Destroy(go);

                // удаление из сохранения
                saveService.RemoveBuildingByName(found.instanceName);
            }
            else
            {
                Debug.Log("Ни одного здания в этой клетке");
            }
        }

        // Правый клик — выйти из режима удаления
        if (Input.GetMouseButtonDown(1))
            Deactivate();
    }

    /// <summary>
    /// Метод, активирующий режим удаления
    /// </summary>
    public void Activate()
    {
        isActive = true;
        deleteOverlay.SetActive(true);
    }

    /// <summary>
    /// Метод, деактивирующий режим удаления
    /// </summary>
    public void Deactivate()
    {
        isActive = false;
        deleteOverlay.SetActive(false);
    }
}