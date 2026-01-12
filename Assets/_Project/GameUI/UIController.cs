using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject buildingsPanel;
    [SerializeField] private BuildingConfig[] buildingConfigs;
    private PlacementController placementController;
    private DeleteController deleteController;
    private bool deleteMode = false;

    void Start()
    {
        buildingsPanel.SetActive(false);
        placementController = FindObjectOfType<PlacementController>();
        deleteController = FindObjectOfType<DeleteController>();
    }

    /// <summary>
    /// Метод, вызывающийся при нажатии на кнопку разместить здание
    /// </summary>
    public void OnPlaceButtonPressed()
    {
        if (deleteMode)
        {
            deleteMode = false;
            if (deleteController != null)
                deleteController.Deactivate();
        }

        // Переключаем видимость панели выбора зданий
        if (buildingsPanel != null)
            buildingsPanel.SetActive(!buildingsPanel.activeSelf);
    }

    /// <summary>
    /// Метод, вызывающийся при выборе здания
    /// </summary>
    /// <param name="index"></param>
    public void SelectBuilding(int index)
    {
        if (index < 0 || index >= buildingConfigs.Length) return;
        placementController.StartPlacement(buildingConfigs[index]);
        buildingsPanel.SetActive(false);
    }

    /// <summary>
    /// Метод, вызывающийся при нажатии на кнопку удаления
    /// </summary>
    public void OnDeleteButtonPressed()
    {
        if (deleteController == null)
        {
            Debug.LogWarning("DeleteController не привязан в UIController!");
            return;
        }

        // Переключаем режим удаления
        deleteMode = !deleteMode;

        if (deleteMode)
        {
            deleteController.Activate();
            if (buildingsPanel != null)
                buildingsPanel.SetActive(false);
            Debug.Log("Режим удаления включен");
        }
        else
        {
            deleteController.Deactivate();
            Debug.Log("Режим удаления выключен");
        }
    }

    public void OnLoadBuildingsButtonPressed()
    {
        var saveService = FindObjectOfType<SaveService>();
        if (saveService != null)
        {
            saveService.Load();
            Debug.Log("Здания загружены вручную через кнопку");
        }
    }
}
