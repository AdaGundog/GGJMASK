using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LevelEditor; // LevelData ve EnemyData'ya eriþmek için

public class LevelLoader : MonoBehaviour
{
    // public string levelFileNameToLoad = "Level_1"; <-- BU ARTIK YOK (Otomatik)

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
        // --- DÝNAMÝK DOSYA ADI ---
        // GameManager'daki sayýya göre dosya adýný oluþturuyoruz: Level_1, Level_2 vs.
        string currentFileName = "Level_" + GameManager.Instance.currentLevelIndex;

        // Dosya Yolunu Bul (Senin klasör yapýna uygun: Fikret/Levels)
        string savePath = Path.Combine(Application.dataPath, "Fikret/Levels");
        string fullPath = Path.Combine(savePath, currentFileName + ".json");

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"KRÝTÝK HATA: Bölüm dosyasý bulunamadý! Yol: {fullPath}");
            Debug.LogWarning("Oyun bitmiþ olabilir veya henüz bu level çizilmemiþ.");
            return;
        }

        // Dosyayý Oku
        string json = File.ReadAllText(fullPath);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        Debug.Log($"Bölüm Yükleniyor: {currentFileName} (Mürekkep: {data.startingInkAmount})");

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

            // --- YENÝ EKLENEN KISIM: KÝMLÝK ZORLAMASI ---
            // Yaratýlan objenin üzerindeki kimlik kartýný bul
            UnitRegistration reg = unit.GetComponent<UnitRegistration>();
            if (reg != null)
            {
                // "Sen oyuncu deðilsin, sen düþmansýn!" de.
                reg.isPlayerUnit = false;
                reg.unitType = (UnitType)data.type;
            }
            // ---------------------------------------------
        }
    }
}