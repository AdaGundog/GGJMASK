using UnityEngine;
using System;
using System.Collections.Generic;
using LevelEditor; // EnemyType için
using UnityEngine.SceneManagement; // Sahne yenilemek için şart

public enum GameState
{
    Preparation, // Asker yerleştirme
    Battle,      // Savaş başladı
    Victory,     // Kazandık
    Defeat       // Kaybettik
}

[Serializable]
public class VeteranData
{
    public EnemyType type;
    public int rank;
    public string unitName; // Karakterin adını burada tutacağız

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
    // --- YENİ: LEVEL SAYACI ---
    public int currentLevelIndex = 1;

    public event Action OnBattleStarted;
    public event Action<int> OnMoneyChanged;
    public event Action OnVictory;
    public event Action OnDefeat;

    [Header("Economy")]
    public int startingMoney = 1000;
    public int CurrentMoney { get; private set; }

    // Gaziler Listesi
    public List<VeteranData> veterans = new List<VeteranData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ölümsüz GameManager
        }
        else
        {
            Destroy(gameObject);
        }

        CurrentState = GameState.Preparation;
        CurrentMoney = startingMoney;
    }

    // --- YENİ: LEVEL BİTİRME FONKSİYONU ---
    public void LevelCompleted()
    {
        Debug.Log("🎉 LEVEL TAMAMLANDI!");
        currentLevelIndex++;
        CurrentState = GameState.Preparation;

        // --- BU SATIRI SİLDİK VEYA YORUMA ALDIK ---
        // CurrentMoney = startingMoney; 

        // Listeleri temizle ve sahneyi yeniden yükle
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
        // UI'daki yazıların (Text) güncellenmesi için event'i tetikle
        OnMoneyChanged?.Invoke(CurrentMoney);
    }

    public bool SpendMoney(int amount)
    {
        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            OnMoneyChanged?.Invoke(CurrentMoney);
            return true;
        }
        return false;
    }

    // GameManager.cs içinde güncelle
    public void SaveSurvivors(List<GameObject> survivors)
    {
        veterans.Clear();
        foreach (GameObject unit in survivors)
        {
            BaseUnit baseUnit = unit.GetComponent<BaseUnit>();
            UnitRegistration reg = unit.GetComponent<UnitRegistration>();

            if (baseUnit != null && reg != null)
            {
                // RÜTBE ATLATMA
                if (baseUnit.currentRank < 4) // Sınırı 4'e çektik
                {
                    baseUnit.currentRank++;
                }

                // Artık ismiyle beraber kaydediyoruz
                // Not: Eğer birimin Inspector'da yazdığın bir adı varsa 'baseUnit.unitFullName' kullan
                string nameToSave = string.IsNullOrEmpty(baseUnit.unitFullName) ? unit.name : baseUnit.unitFullName;

                veterans.Add(new VeteranData((EnemyType)reg.unitType, baseUnit.currentRank, nameToSave));

                Debug.Log($"{nameToSave} gazi olarak kaydedildi. Yeni Rütbe: {baseUnit.currentRank}");
            }
        }
    }
    // UnitManager buradan çağıracak
    public void TriggerVictory()
    {
        if (CurrentState == GameState.Victory) return; // Zaten kazandık
        CurrentState = GameState.Victory;

        Debug.Log("🏆 ZAFER DUYURULDU!");
        OnVictory?.Invoke(); // UI bunu duyup paneli açacak
    }

    public void TriggerDefeat()
    {
        if (CurrentState == GameState.Defeat) return;
        CurrentState = GameState.Defeat;

        Debug.Log("❌ BOZGUN DUYURULDU!");
        OnDefeat?.Invoke(); // UI bunu duyup paneli açacak
    }

    public void RetryLevel()
    {
        // Kaybedince tekrar deneme mantığı
        CurrentState = GameState.Preparation;
        CurrentMoney = startingMoney;
        OnMoneyChanged?.Invoke(CurrentMoney);

        if (UnitManager.Instance != null)
        {
            UnitManager.Instance.activePlayerUnits.Clear();
            UnitManager.Instance.activeEnemyUnits.Clear();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}