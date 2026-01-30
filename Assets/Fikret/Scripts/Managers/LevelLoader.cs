using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Header("Level Settings")]
    public string levelFileNameToLoad = "Level_1"; 

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
        // 1. Dosya Yolunu Bul (Editör scriptindeki mantýðýn aynýsý)
        string savePath = Path.Combine(Application.dataPath, "Fikret/Levels");
        string fullPath = Path.Combine(savePath, levelFileNameToLoad + ".json");

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"KRÝTÝK HATA: Bölüm dosyasý bulunamadý! Yol: {fullPath}");
            return;
        }

        // 2. Dosyayý Oku
        string json = File.ReadAllText(fullPath);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        Debug.Log($"Bölüm Yükleniyor: {data.levelName} - Mürekkep: {data.startingInkAmount}");

        // 3. Askerleri Doður (Spawn)
        foreach (var enemyData in data.enemies)
        {
            SpawnUnit(enemyData);
        }

        // Developer B'ye haber ver: "Mürekkep miktarýný ayarla"
        // Örn: InkManager.Instance.SetInk(data.startingInkAmount);
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
            // Zemin Z=0 olduðu için pozisyonu garantile
            Vector3 spawnPos = new Vector3(data.position.x, data.position.y, 0);

            GameObject unit = Instantiate(prefabToUse, spawnPos, Quaternion.identity, gameUnitsParent);

            // Ýsimlendirme (Debug kolaylýðý için)
            unit.name = $"GameUnit_{data.type}";

            // BURASI ÖNEMLÝ:
            // Developer A'nýn yazdýðý "UnitHealth" veya "UnitCombat" scriptleri 
            // bu prefablarýn üzerinde olacaðý için otomatik çalýþmaya baþlayacaklar!
        }
    }
}