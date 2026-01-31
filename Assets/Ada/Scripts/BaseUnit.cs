using UnityEngine;
using UnityEngine.AI;

public abstract class BaseUnit : MonoBehaviour
{
    [Header("Base Settings")]
    public UnitData data;
    public float currentHealth;
    protected NavMeshAgent agent;
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
    private string unitFullName;

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
        if (healthBarFill != null)
        {
            // Canvas'ý aktif et (Eðer kapalýysa)
            GameObject canvasObj = healthBarFill.canvas != null ?
                                   healthBarFill.canvas.gameObject :
                                   healthBarFill.transform.parent.gameObject;

            if (canvasObj != null && !canvasObj.activeSelf)
            {
                canvasObj.SetActive(true);
            }

            // Doluluk oranýný hesapla (Float bölmesi)
            healthBarFill.fillAmount = (float)currentHealth / (float)data.maxHealth;

            Debug.Log($"{gameObject.name} (Rank {currentRank}) hasar aldý. Vuran Rank: {attackerRank}. Kalan Can: {currentHealth}");
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
        if (rankIconImage == null) return;

        switch (currentRank)
        {
            case 1: rankIconImage.sprite = rank1Sprite; break;
            case 2: rankIconImage.sprite = rank2Sprite; break;
            case 3: rankIconImage.sprite = rank3Sprite; break;
        }

        // Rütbe 1'de simgeyi gizlemek istersen:
        rankIconImage.gameObject.SetActive(currentRank > 1);
    }

    protected virtual void Die()
    {
        // SelectionManager listesinden kendini temizlemesi için bir event veya doðrudan eriþim
        FindObjectOfType<SelectionManager>().selectedUnits.Remove(this as PlayerUnit);
        Destroy(gameObject);
    }
}