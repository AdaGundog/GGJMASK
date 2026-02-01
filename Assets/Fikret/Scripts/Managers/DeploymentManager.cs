using UnityEngine;
using UnityEngine.EventSystems; // UI Tıklamasını algılamak için şart
using LevelEditor; // EnemyType ve VeteranData için

public class DeploymentManager : MonoBehaviour
{
    [Header("Settings")]
    public Collider2D spawnZoneCollider; // Asker koyulabilecek yeşil alan

    [Header("Unit Prefabs")]
    // Gerçek asker prefabları
    public GameObject infantryPrefab;
    public GameObject archerPrefab;
    public GameObject cavalryPrefab;

    [Header("Visuals")]
    public Transform ghostObject; // Mouse'u takip eden yarı saydam asker

    private GameObject selectedPrefab; // Şu an seçili olan asker türü

    [Header("Unit Costs")]
    public int infantryCost = 50;
    public int archerCost = 75;
    public int cavalryCost = 120;

    private int currentSelectedCost = 0;

    private void Start()
    {
        // Başlangıçta hayalet kapalı olsun
        if (ghostObject) ghostObject.gameObject.SetActive(false);

        // Varsa önceki bölümden kalan gazileri yerleştir
        SpawnVeterans();
    }

    private void SpawnVeterans()
    {
        if (GameManager.Instance.veterans.Count > 0)
        {
            Debug.Log("Gaziler sahneye yerleştiriliyor...");

            Vector3 spawnStartPos = spawnZoneCollider.bounds.center;
            spawnStartPos.x -= spawnZoneCollider.bounds.extents.x * 0.8f;
            int index = 0;

            foreach (VeteranData vet in GameManager.Instance.veterans)
            {
                GameObject prefabToSpawn = null;

                // EnemyType ve UnitType aynı değerlere sahip, cast yapıyoruz
                switch ((UnitType)vet.type)
                {
                    case UnitType.Infantry: prefabToSpawn = infantryPrefab; break;
                    case UnitType.Archer: prefabToSpawn = archerPrefab; break;
                    case UnitType.Cavalry: prefabToSpawn = cavalryPrefab; break;
                }

                if (prefabToSpawn != null)
                {
                    // Pozisyonu hesapla (5'li sıralar halinde dizilirler)
                    Vector3 pos = spawnStartPos + new Vector3((index % 5) * 1.5f, (index / 5) * -1.5f, 0);

                    // Objeyi yarat
                    GameObject newVet = Instantiate(prefabToSpawn, pos, Quaternion.identity);

                    // --- KİMLİK VE RÜTBE YÜKLEME ---
                    BaseUnit bUnit = newVet.GetComponent<BaseUnit>();
                    if (bUnit != null)
                    {
                        // VeteranData'da unitName yok, yeni isim oluştur
                        bUnit.currentRank = vet.rank;      // Kaydedilen rütbesini ver

                        // Görselleri (Rank ikonu ve Emission parlaması) güncelle
                        bUnit.UpdateRankVisuals();

                        // Hierarchy'de ismini düzelt
                        newVet.name = "Veteran_" + bUnit.unitFullName;
                    }
                }
                index++;
            }
        }
    }

    private void Update()
    {
        // -----------------------------------------------------------
        // 🛑 1. GÜVENLİK KAPISI: PAUSE KONTROLÜ
        // Oyun durmuşsa (Pause menüsü açıksa) hiçbir şey yapma.
        // -----------------------------------------------------------
        if (Time.timeScale == 0f) return;


        // -----------------------------------------------------------
        // 🛑 2. GÜVENLİK KAPISI: UI (BUTON) KONTROLÜ
        // Mouse şu an bir butonun veya panelin üzerinde mi?
        // -----------------------------------------------------------
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Eğer mouse UI üzerindeyse hayaleti gizle ki oyuncu kafası karışmasın
            if (ghostObject) ghostObject.gameObject.SetActive(false);

            // Tıklamayı engelle ve fonksiyondan çık
            return;
        }

        // -----------------------------------------------------------
        // ✅ 3. NORMAL OYUN AKIŞI
        // Savaş başlamadıysa (Preparation) yerleştirme yapmaya izin ver
        // -----------------------------------------------------------
        if (GameManager.Instance.CurrentState == GameState.Preparation)
        {
            HandlePlacementInput();
        }
        else
        {
            // Savaş başladıysa (Battle) hayaleti kapat ve bu scripti devre dışı bırak
            if (ghostObject) ghostObject.gameObject.SetActive(false);
            this.enabled = false;
        }
    }

    // UI Butonları (Piyade Seç, Okçu Seç) bu fonksiyonu çağırır
    public void SelectUnitToPlace(string unitType)
    {
        switch (unitType)
        {
            case "Infantry":
                selectedPrefab = infantryPrefab;
                currentSelectedCost = infantryCost;
                break;
            case "Archer":
                selectedPrefab = archerPrefab;
                currentSelectedCost = archerCost;
                break;
            case "Cavalry":
                selectedPrefab = cavalryPrefab;
                currentSelectedCost = cavalryCost;
                break;
        }

        // Seçim yapılınca hayaleti görünür yap
        if (ghostObject) ghostObject.gameObject.SetActive(true);
        // Debug.Log($"Seçildi: {unitType} - Fiyat: {currentSelectedCost}");
    }

    private void HandlePlacementInput()
    {
        // Eğer henüz bir asker seçilmediyse işlem yapma
        if (selectedPrefab == null) return;

        // 1. Mouse Pozisyonunu Dünyadaki Pozisyona Çevir
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // 2D olduğu için Z'yi sıfırla

        // 2. Hayaleti Mouse'un ucuna taşı
        if (ghostObject)
        {
            ghostObject.gameObject.SetActive(true); // UI'dan çıkınca tekrar görünür yap
            ghostObject.position = mousePos;
        }

        // 3. Sol Tık Kontrolü (Yerleştirme)
        if (Input.GetMouseButtonDown(0))
        {
            // Tıklanan yer Spawn Zone (Yeşil Alan) içinde mi?
            if (spawnZoneCollider.OverlapPoint(mousePos))
            {
                // Para yetiyor mu?
                if (GameManager.Instance.SpendMoney(currentSelectedCost))
                {
                    SpawnUnit(mousePos);
                }
                else
                {
                    Debug.Log("❌ Yetersiz Bakiye!");
                    // Buraya ileride "Bip" sesi eklenebilir
                }
            }
            else
            {
                Debug.Log("🚫 Bu alana asker koyamazsın!");
            }
        }

        // 4. Sağ Tık Kontrolü (İptal Etme)
        if (Input.GetMouseButtonDown(1))
        {
            selectedPrefab = null;
            if (ghostObject) ghostObject.gameObject.SetActive(false);
        }
    }

    private void SpawnUnit(Vector3 position)
    {
        Instantiate(selectedPrefab, position, Quaternion.identity);
    }
}