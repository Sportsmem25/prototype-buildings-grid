using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveService : MonoBehaviour
{
    private SaveData saveData = new SaveData();
    private string path;

    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "SaveData.json");
        Debug.Log($"Save path initialized: {path}");
    }

    /// <summary>
    /// Метод, регистрирующий новый экземпляр объекта
    /// </summary>
    /// <param name="bi"></param>
    /// <param name="instanceName"></param>
    public void RegisterBuildingInstance(BuildingInstance bi, string instanceName)
    {
        var sb = new SavedBuildingWithName
        {
            id = bi.id,
            x = bi.gridX,
            y = bi.gridY,
            w = bi.width,
            h = bi.height,
            instanceName = instanceName
        };
        saveData.buildingsWithNames.Add(sb);
        Debug.Log($"[SaveService] Added building: {bi.id} at ({bi.gridX},{bi.gridY})");
        Save();
    }

    /// <summary>
    /// Метод поиска здания
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public SavedBuildingWithName FindBuildingAt(int x, int y)
    {
        if (saveData == null || saveData.buildingsWithNames == null)
            return null;

        return saveData.buildingsWithNames.Find(b =>x >= b.x && x < b.x + b.w && y >= b.y && y < b.y + b.h);
    }

    /// <summary>
    /// Метод, удаляющий здание по имени экземпляра
    /// </summary>
    /// <param name="instanceName"></param>
    public void RemoveBuildingByName(string instanceName)
    {
        var found = saveData.buildingsWithNames.Find(b => b.instanceName == instanceName);
        if (found != null)
        {
            saveData.buildingsWithNames.Remove(found);
            Save();
        }
    }

    /// <summary>
    /// Сохранение зданий в JSON
    /// </summary>
    public void Save()
    {
        if (saveData == null)
        {
            Debug.LogError("[SaveService] saveData is NULL!");
            return;
        }

        var json = JsonUtility.ToJson(saveData, true);
        Debug.Log($"[SaveService] JSON content before save:\n{json}");

        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogError("[SaveService] JsonUtility returned empty string!");
            return;
        }

        File.WriteAllText(path, json, System.Text.Encoding.UTF8);
        Debug.Log($"[SaveService] Saved {saveData.buildingsWithNames.Count} buildings to {path}");
    }

    /// <summary>
    /// Загрузка зданий из JSON и размещение их на сетке
    /// </summary>
    public void Load()
    {
        if (!File.Exists(path)) return;

        try
        {
            var json = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(json);

            // Миграция со старого формата
            if (saveData.buildings != null && saveData.buildings.Count > 0 && (saveData.buildingsWithNames == null || saveData.buildingsWithNames.Count == 0))
            {
                Debug.Log("Старый формат сохранения найден");
                var migrated = new List<SavedBuildingWithName>();
                foreach (var b in saveData.buildings)
                {
                    migrated.Add(new SavedBuildingWithName
                    {
                        id = b.id,
                        x = b.x,
                        y = b.y,
                        w = b.w,
                        h = b.h,
                        instanceName = b.id + "_" + Guid.NewGuid().ToString()
                    });
                }
                saveData.buildingsWithNames = migrated;
                saveData.buildings = null; 
                Save();
            }

            // Создание зданий на сетке
            ConfigLoader configLoader = FindObjectOfType<ConfigLoader>();
            GridManager grid = FindObjectOfType<GridManager>();

            foreach (var sb in saveData.buildingsWithNames)
            {
                var cfg = System.Array.Find(configLoader.Config.buildings, c => c.id == sb.id);
                if (cfg == null) continue;
                grid.SetOccupied(sb.x, sb.y, sb.w, sb.h, true);

                // Создание объекта в сцене с именем instanceName
                GameObject go = new GameObject(sb.instanceName);
                BuildingInstance buildingInstance = go.AddComponent<BuildingInstance>();
                buildingInstance.id = sb.id; buildingInstance.gridX = sb.x; buildingInstance.gridY = sb.y; buildingInstance.width = sb.w; buildingInstance.height = sb.h;
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>(cfg.spritePath);
                go.transform.position = grid.CellToWorld(sb.x, sb.y);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to load save file: " + ex.Message);
        }
        Debug.Log($"Loaded {saveData.buildingsWithNames.Count} buildings from {path}");
    }
}

[System.Serializable]
public class SaveBuilding
{
    public string id;
    public int x;
    public int y;
    public int w;
    public int h;
}

[System.Serializable]
public class SaveData
{
    public List<SavedBuildingWithName> buildingsWithNames = new List<SavedBuildingWithName>();
    public List<SaveBuilding> buildings;
}

[System.Serializable]
public class SavedBuildingWithName : SaveBuilding
{
    public string instanceName;
}