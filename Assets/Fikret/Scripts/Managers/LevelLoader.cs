using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LevelEditor;

public class LevelLoader : MonoBehaviour
{
    [Header("Game Prefabs (Real Units)")]
    public GameObject realInfantryPrefab;
    public GameObject realArcherPrefab;
    public GameObject realCavalryPrefab;

    [Header("Runtime References")]
    public Transform gameUnitsParent;

    private void Start()
    {
        LoadLevelAndSpawn();
    }

    private void LoadLevelAndSpawn()
    {
        string currentFileName = "Level_" + GameManager.Instance.currentLevelIndex + ".json";

        // --- DEÐÝÞÝKLÝK BURADA ---
        // Artýk StreamingAssets/Levels klasörüne bakýyor
        string fullPath = Path.Combine(Application.streamingAssetsPath, "Levels", currentFileName);
        // -------------------------

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"KRÝTÝK HATA: Bölüm dosyasý bulunamadý! Yol: {fullPath}");
            return;
        }

        // 1. Dosyayý Oku
        string json = File.ReadAllText(fullPath);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        // Parayý Oyuncuya Ver
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(data.levelStartBonus);
            Debug.Log($"[LevelLoader] {currentFileName} yüklendi. Bonus Para: {data.levelStartBonus}");
        }

        // Askerleri Doður (Spawn)
        foreach (var enemyData in data.enemies)
        {
            SpawnUnit(enemyData);
        }
    }

    private void SpawnUnit(EnemySpawnData data)
    {
        GameObject prefabToUse = null;

        switch (data.type)
        {
            case EnemyType.Infantry: prefabToUse = realInfantryPrefab; break;
            case EnemyType.Archer: prefabToUse = realArcherPrefab; break;
            case EnemyType.Cavalry: prefabToUse = realCavalryPrefab; break;
        }

        if (prefabToUse != null)
        {
            Vector3 spawnPos = new Vector3(data.position.x, data.position.y, 0);
            GameObject unit = Instantiate(prefabToUse, spawnPos, Quaternion.identity, gameUnitsParent);

            unit.name = $"Enemy_{data.type}";

            // Kimlik Zorlamasý
            UnitRegistration reg = unit.GetComponent<UnitRegistration>();
            if (reg != null)
            {
                reg.isPlayerUnit = false;
                reg.unitType = (UnitType)data.type;
            }
        }
    }
}