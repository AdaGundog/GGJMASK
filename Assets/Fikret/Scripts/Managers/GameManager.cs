using UnityEngine;
using System;
using System.Collections.Generic;
using LevelEditor; // EnemyType için
using UnityEngine.SceneManagement;

public enum GameState
{
    Preparation,
    Battle,
    Victory,
    Defeat
}

[Serializable]
public class VeteranData
{
    public EnemyType type;
    public int rank;
    public string unitName;

    public VeteranData(EnemyType t, int r, string n)
    {
        type = t;
        rank = r;
        unitName = n;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState CurrentState { get; private set; }
    public int currentLevelIndex = 1;

    public event Action OnBattleStarted;
    public event Action<int> OnMoneyChanged; // UI int beklediği için int kaldı
    public event Action OnVictory;
    public event Action OnDefeat;

    [Header("Economy")]
    public int startingMoney = 1000;
    public float CurrentMoney { get; private set; } // Hassas hesaplama için float

    public List<VeteranData> veterans = new List<VeteranData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CurrentState = GameState.Preparation;
        CurrentMoney = startingMoney;
    }
    // GameManager.cs içine ekle:

    public void ResetGameData()
    {
        // 1. Oyun Durumunu "Hazırlık" Moduna Çek
        CurrentState = GameState.Preparation;

        // 2. Parayı Sıfırla (Başlangıç parası neyse onu yaz, örn: 100)
        CurrentMoney = startingMoney;

        // 3. Eski Gazileri Sil (Yoksa önceki oyundan kalanlar tekrar doğar)
        veterans.Clear();

        // 4. Bölüm Sayacını Sıfırla
        currentLevelIndex = 1;

        // 5. Zamanı Düzelt (Eğer Pause modunda çıktıysan zaman donuk kalmış olabilir!)
        Time.timeScale = 1f;

        Debug.Log("GameManager verileri sıfırlandı. Yeni oyuna hazır.");
    }
    public void LevelCompleted()
    {
        Debug.Log("🎉 LEVEL TAMAMLANDI!");
        currentLevelIndex++;
        CurrentState = GameState.Preparation;

        // CurrentMoney = startingMoney; // Birikimli sistem için kapalı

        if (UnitManager.Instance != null)
        {
            UnitManager.Instance.activePlayerUnits.Clear();
            UnitManager.Instance.activeEnemyUnits.Clear();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartBattle()
    {
        if (CurrentState != GameState.Preparation) return;

        CurrentState = GameState.Battle;
        Debug.Log("SAVAŞ BAŞLADI! ⚔️");
        OnBattleStarted?.Invoke();
    }

    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        // Float'ı int'e çevirip UI'a gönderiyoruz
        OnMoneyChanged?.Invoke((int)CurrentMoney);
    }

    public bool SpendMoney(float amount)
    {
        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            OnMoneyChanged?.Invoke((int)CurrentMoney);
            return true;
        }
        return false;
    }

    public void SaveSurvivors(List<GameObject> survivors)
    {
        veterans.Clear();
        foreach (GameObject unit in survivors)
        {
            BaseUnit baseUnit = unit.GetComponent<BaseUnit>();
            UnitRegistration reg = unit.GetComponent<UnitRegistration>();

            if (baseUnit != null && reg != null)
            {
                if (baseUnit.currentRank < 4) // Efsanevi rütbe sınırı
                {
                    baseUnit.currentRank++;
                }

                string nameToSave = string.IsNullOrEmpty(baseUnit.unitFullName) ? unit.name : baseUnit.unitFullName;
                veterans.Add(new VeteranData((EnemyType)reg.unitType, baseUnit.currentRank, nameToSave));

                Debug.Log($"{nameToSave} rütbe aldı: {baseUnit.currentRank}");
            }
        }
    }

    public void TriggerVictory()
    {
        if (CurrentState == GameState.Victory) return;
        CurrentState = GameState.Victory;
        OnVictory?.Invoke();
    }

    public void TriggerDefeat()
    {
        if (CurrentState == GameState.Defeat) return;
        CurrentState = GameState.Defeat;
        OnDefeat?.Invoke();
    }

    public void RetryLevel()
    {
        CurrentState = GameState.Preparation;
        CurrentMoney = startingMoney;
        OnMoneyChanged?.Invoke((int)CurrentMoney);

        if (UnitManager.Instance != null)
        {
            UnitManager.Instance.activePlayerUnits.Clear();
            UnitManager.Instance.activeEnemyUnits.Clear();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}