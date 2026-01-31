using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    [Header("Live Units")]
    // Sahnede o an yaşayan tüm birimlerin listesi
    public List<GameObject> activePlayerUnits = new List<GameObject>();
    public List<GameObject> activeEnemyUnits = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    // Birimler doğunca bu fonksiyonu çağıracak
    public void RegisterUnit(GameObject unit, bool isPlayer)
    {
        if (isPlayer)
        {
            if (!activePlayerUnits.Contains(unit)) activePlayerUnits.Add(unit);
        }
        else
        {
            if (!activeEnemyUnits.Contains(unit)) activeEnemyUnits.Add(unit);
        }
    }

    // Birimler ölünce bu fonksiyonu çağıracak
    public void UnregisterUnit(GameObject unit, bool isPlayer)
    {
        if (isPlayer)
        {
            if (activePlayerUnits.Contains(unit)) activePlayerUnits.Remove(unit);
        }
        else
        {
            if (activeEnemyUnits.Contains(unit)) activeEnemyUnits.Remove(unit);
        }

        CheckGameEndCondition();
    }

    private void CheckGameEndCondition()
    {
        if (GameManager.Instance.CurrentState != GameState.Battle) return;

        if (activePlayerUnits.Count == 0 && activeEnemyUnits.Count > 0)
        {
            GameManager.Instance.TriggerDefeat();
        }
        else if (activeEnemyUnits.Count == 0 && activePlayerUnits.Count > 0)
        {
            // Kazandık -> Önce kaydet, sonra bildir
            GameManager.Instance.SaveSurvivors(activePlayerUnits);
            GameManager.Instance.TriggerVictory();
        }
    }
}