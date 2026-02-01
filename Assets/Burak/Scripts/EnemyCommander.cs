using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCommander : MonoBehaviour
{
    [Header("Manager")]
    public GameManager gameManager;

    [Header("Ayarlar")]
    public Transform spawnPoint; 
    public Transform myCastleTransform;   
    public Transform enemyCastleTransform; 

    public float decisionInterval = 1f; 
    public int currentLevel = 1; 

    [Header("Ekonomi")]
    // Düşman AI'ın kendi gold'u (GameManager'dan bağımsız)
    private float currentGold = 0f;
    public float incomeRate = 5f;

    [Header("Prefablar")]
    public GameObject infantryPrefab;
    public GameObject archerPrefab;
    public GameObject cavalryPrefab;

    [Header("Maliyetler - DeploymentManager ile aynı")]
    public float infantryCost = 50f;
    public float archerCost = 75f;
    public float cavalryCost = 120f;

    void Start()
    {
        // Başlangıçta GameManager ile aynı miktarda gold
        if(gameManager != null)
        {
            currentGold = gameManager.startingMoney;
        }
        StartCoroutine(ThinkAndActRoutine());
    }

    void Update()
    {
        // Düşman AI'ın gelir sistemi
        currentGold += incomeRate * Time.deltaTime;
    }

    IEnumerator ThinkAndActRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(decisionInterval);
            MakeDecision();
        }
    }

    void MakeDecision()
    {
        // Oyuncu kompozisyonunu analiz et
        Dictionary<UnitType, int> playerComposition = AnalyzePlayerComposition();
        
        // En çok olan tipi bul
        UnitType dominantType = GetDominantType(playerComposition);
        
        // Karşı strateji belirle
        UnitType unitToSpawn = GetCounterUnit(dominantType);

        // Spawn et
        TrySpawnUnit(unitToSpawn);
    }

    // YENİ: Oyuncu kompozisyonunu analiz et
    Dictionary<UnitType, int> AnalyzePlayerComposition()
    {
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
        
        Dictionary<UnitType, int> composition = new Dictionary<UnitType, int>
        {
            { UnitType.Infantry, 0 },
            { UnitType.Archer, 0 },
            { UnitType.Cavalry, 0 }
        };

        foreach (GameObject unit in playerUnits)
        {
            PlayerUnit pUnit = unit.GetComponent<PlayerUnit>();
            if (pUnit != null && pUnit.data != null)
            {
                composition[pUnit.data.type]++;
            }
        }

        Debug.Log($"[AI Commander] Oyuncu Güçleri: {composition[UnitType.Infantry]} Piyade, {composition[UnitType.Archer]} Okçu, {composition[UnitType.Cavalry]} Atlı");
        
        return composition;
    }

    // YENİ: En baskın tipi bul
    UnitType GetDominantType(Dictionary<UnitType, int> composition)
    {
        UnitType dominant = UnitType.Infantry;
        int maxCount = 0;

        foreach (var pair in composition)
        {
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
                dominant = pair.Key;
            }
        }

        return dominant;
    }

    UnitType GetCounterUnit(UnitType threat)
    {
        switch (threat)
        {
            case UnitType.Cavalry: return UnitType.Infantry;
            case UnitType.Archer: return UnitType.Cavalry;
            case UnitType.Infantry: return UnitType.Archer;
            default: return UnitType.Infantry;
        }
    }

    void TrySpawnUnit(UnitType type)
    {
        GameObject prefabToSpawn = null;
        float cost = 0;

        switch (type)
        {
            case UnitType.Infantry: prefabToSpawn = infantryPrefab; cost = infantryCost; break;
            case UnitType.Archer: prefabToSpawn = archerPrefab; cost = archerCost; break;
            case UnitType.Cavalry: prefabToSpawn = cavalryPrefab; cost = cavalryCost; break;
        }

        // Kendi gold'undan kontrol
        if (currentGold >= cost && prefabToSpawn != null)
        {
            Spawn(prefabToSpawn, cost, type);
        }
        else
        {
            Debug.Log($"[AI Commander] Yetersiz altın! İhtiyaç: {cost}, Mevcut: {currentGold:F0}");
        }
    }

    void Spawn(GameObject prefab, float cost, UnitType type)
    {
        // Kendi gold'undan harca
        currentGold -= cost;
        
        // 2D için spawn pozisyonu (Z=0)
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.z = 0;
        
        GameObject newUnit = Instantiate(prefab, spawnPos, Quaternion.identity);
        newUnit.tag = "EnemyUnit"; // Tag'i ayarla

        EnemyUnitAI aiScript = newUnit.GetComponent<EnemyUnitAI>();
        if (aiScript != null)
        {
            aiScript.unitType = type;
            aiScript.Initialize((UnitRank)(currentLevel - 1), myCastleTransform, enemyCastleTransform);
        }
        
        Debug.Log($"[AI Commander] {type} üretildi! Kalan Altın: {currentGold:F0}");
    }
}