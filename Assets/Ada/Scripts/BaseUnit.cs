using UnityEngine;
using UnityEngine.AI;

public abstract class BaseUnit : MonoBehaviour
{
    [Header("Base Settings")]
    public UnitData data;
    public float currentHealth;
    protected NavMeshAgent agent;
    [SerializeField] private GameObject healthBarGroup;
    public UnityEngine.UI.Image healthBarFill; // Inspector'dan yeþil resmi buraya sürükle

    [Header("Combat State")]
    public BaseUnit target;

    [Header("Rank System")]
    [Range(1, 3)]
    public int currentRank = 1; // 1: Er, 2: Çavuþ, 3: Seçkin
    public UnityEngine.UI.Image rankIconImage; // Baþýndaki rütbe simgesi

    [Header("Rank Sprites")]
    public Sprite rank1Sprite; // Boþ veya basit bir simge
    public Sprite rank2Sprite; // Ýki þeritli simge
    public Sprite rank3Sprite; // Üç þeritli/Yýldýzlý simge

    [Header("Unit Info")]
    public string unitFullName;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = data.maxHealth;

        // 2D Ayarlarý
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = data.moveSpeed;
    }
    public virtual void Start()
    {
        // Eðer isim atanmamýþsa NamingSystem'dan çek
        if (string.IsNullOrEmpty(unitFullName))
        {
            unitFullName = NamingSystem.Instance.GenerateRandomName();
            gameObject.name = unitFullName; // Hierarchy'de de ismi deðiþsin
        }

        UpdateRankVisuals();
    }
    public void TakeDamage(float amount, UnitType attackerType, int attackerRank)
    {
        float finalDamage = amount;

        // 1. SALDIRGAN RÜTBE BONUSU (Vuran birim rütbeliyse hasarý artýrýr)
        // Rank 1: x1.0 | Rank 2: x1.2 | Rank 3: x1.4 hasar verir.
        float attackerMultiplier = 1f + ((attackerRank - 1) * 0.2f);
        finalDamage *= attackerMultiplier;

        // 2. TAÞ-KAÐIT-MAKAS DENGESÝ (Avantajlý tip %50 fazla vurur)
        if (attackerType == UnitType.Archer && data.type == UnitType.Infantry) finalDamage *= 1.5f;
        else if (attackerType == UnitType.Infantry && data.type == UnitType.Cavalry) finalDamage *= 1.5f;
        else if (attackerType == UnitType.Cavalry && data.type == UnitType.Archer) finalDamage *= 1.5f;

        // 3. SAVUNMA RÜTBE BONUSU (Hasar alan birim rütbeliyse daha az hasar alýr)
        // Rank 1: %0 koruma | Rank 2: %10 koruma | Rank 3: %20 koruma.
        float defenseMultiplier = 1f - ((currentRank - 1) * 0.1f);
        finalDamage *= defenseMultiplier;

        // Caný azalt
        currentHealth -= finalDamage;

        // UI GÜNCELLEME
        if (healthBarGroup != null)
    {
        // Rütbe simgesine dokunmadan sadece can barýný gösteriyoruz
        healthBarGroup.SetActive(true);
        
        // Can barý oranýný güncelle
        healthBarFill.fillAmount = currentHealth / data.maxHealth;
        
        // 3 saniye sonra can barýný kapatmasý için (Opsiyonel)
        CancelInvoke("HideHealthBar");
        Invoke("HideHealthBar", 3f);
    }

        if (currentHealth <= 0) Die();
    }

    void ApplyRankBonuses()
    {
        // Her rütbe atladýðýnda caný %20, hasarý %10 artýr gibi:
        currentHealth += data.maxHealth * 0.2f;
        // Not: Saldýrý bonusu için TakeDamage'da rütbeyi de hesaba katabiliriz
    }

    public void UpdateRankVisuals()
    {
        // Eðer resim atanmamýþsa hata vermemesi için kontrol
        if (rankIconImage == null) return;

        // Rütbeye göre Sprite (resim) deðiþtir
        if (currentRank == 1) rankIconImage.sprite = rank1Sprite;
        else if (currentRank == 2) rankIconImage.sprite = rank2Sprite;
        else if (currentRank == 3) rankIconImage.sprite = rank3Sprite;

        // Rank 1 ise simgeyi gizleyebiliriz (isteðe baðlý)
        rankIconImage.gameObject.SetActive(currentRank > 1);
    }

    protected virtual void Die()
    {
        // SelectionManager listesinden kendini temizlemesi için bir event veya doðrudan eriþim
        FindObjectOfType<SelectionManager>().selectedUnits.Remove(this as PlayerUnit);
        Destroy(gameObject);
    }
}