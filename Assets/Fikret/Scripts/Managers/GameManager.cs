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

    public VeteranData(EnemyType t)
    {
        type = t;
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

    [Header("Economy")]
    public int startingMoney = 500;
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
        Debug.Log("🎉 LEVEL TAMAMLANDI! Sonraki level yükleniyor...");

        // 1. Level sayısını artır
        currentLevelIndex++;

        // 2. Modu Hazırlığa çek
        CurrentState = GameState.Preparation;

        // 3. Parayı Sıfırla (Her bölüm yeniden 500 altın verelim)
        CurrentMoney = startingMoney;

        // 4. Listeleri Temizle
        if (UnitManager.Instance != null)
        {
            UnitManager.Instance.activePlayerUnits.Clear();
            UnitManager.Instance.activeEnemyUnits.Clear();
        }

        // 5. Sahneyi Yeniden Yükle (Aynı sahne ama LevelLoader yeni dosyayı okuyacak)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartBattle()
    {
        if (CurrentState != GameState.Preparation) return;

        CurrentState = GameState.Battle;
        Debug.Log("SAVAŞ BAŞLADI! ⚔️");
        OnBattleStarted?.Invoke();
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

    public void SaveSurvivors(List<GameObject> survivors)
    {
        veterans.Clear();

        foreach (GameObject unit in survivors)
        {
            UnitRegistration reg = unit.GetComponent<UnitRegistration>();
            if (reg != null)
            {
                veterans.Add(new VeteranData(reg.unitType));
            }
        }
        Debug.Log($"Oyun Kaydedildi! {veterans.Count} asker bir sonraki bölüme aktarılıyor.");
    }

    // --- TEST İÇİN RESETLEME ---
    private void Update()
    {
        // 'R' tuşuna basınca her şeyi sıfırla ve Level 1'e veya mevcut levele dön
        if (Input.GetKeyDown(KeyCode.R))
        {
            CurrentState = GameState.Preparation;
            CurrentMoney = startingMoney; // Parayı resetle

            // UI Güncellensin diye event tetikle
            OnMoneyChanged?.Invoke(CurrentMoney);

            if (UnitManager.Instance != null)
            {
                UnitManager.Instance.activePlayerUnits.Clear();
                UnitManager.Instance.activeEnemyUnits.Clear();
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}