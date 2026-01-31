using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCommander : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform spawnPoint; 
    public Transform myCastleTransform;   
    public Transform enemyCastleTransform; 

    public float decisionInterval = 1f; 
    public int currentLevel = 1; 

    [Header("Ekonomi")]
    public float currentGold = 100f;
    public float incomeRate = 5f;

    [Header("Prefablar")]
    public GameObject infantryPrefab;
    public GameObject archerPrefab;
    public GameObject cavalryPrefab;

    [Header("Maliyetler")]
    public float infantryCost = 20f;
    public float archerCost = 30f;
    public float cavalryCost = 50f;

    void Start()
    {
        StartCoroutine(ThinkAndActRoutine());
    }

    void Update()
    {
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
        UnitType threatType = AnalyzeEnemyThreat();

        UnitType unitToSpawn = GetCounterUnit(threatType);

        TrySpawnUnit(unitToSpawn);
    }

    UnitType AnalyzeEnemyThreat()
    {
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");

        if (playerUnits.Length == 0) return UnitType.Infantry;

        return UnitType.Infantry;
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

        if (currentGold >= cost && prefabToSpawn != null)
        {
            Spawn(prefabToSpawn, cost, type);
        }
    }

    void Spawn(GameObject prefab, float cost, UnitType type)
    {
        currentGold -= cost;
        GameObject newUnit = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        EnemyUnitAI aiScript = newUnit.GetComponent<EnemyUnitAI>();
        if (aiScript != null)
        {
            aiScript.Initialize((UnitRank)(currentLevel - 1), myCastleTransform, enemyCastleTransform);
        }
        Debug.Log($"Commander: {type} üretti. Kalan Altýn: {currentGold}");
    }
}