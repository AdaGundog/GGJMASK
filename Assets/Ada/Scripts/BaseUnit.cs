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

    [Header("Feedback Settings")]
    public float tiltAmount = 15f; // Ne kadar yana yatacak?
    public float tiltDuration = 0.1f;

    [Header("Combat State")]
    public BaseUnit target;

    [Header("Rank 4 Specials")]
    public float zoneDamageModifier = 2f;
    public GameObject legendaryAura;
    public float healthRegen = 2f;
    public float legendaryPowerBonus = 10f; // 4. rütbeye geçince eklenecek sabit hasar
    private bool isLegendaryPowerApplied = false;
    public TMPro.TextMeshProUGUI nameText;
    [Header("Rank System")]
    [Range(1, 4)]
    public int currentRank = 1; // 1: Er, 2: Çavuþ, 3: Seçkin
    public UnityEngine.UI.Image rankIconImage; // Baþýndaki rütbe simgesi

    [Header("Rank Sprites")]
    public Sprite rank1Sprite; // Boþ veya basit bir simge
    public Sprite rank2Sprite; // Ýki þeritli simge
    public Sprite rank3Sprite; // Üç þeritli/Yýldýzlý simge
    public Sprite rank4Sprite;
    [Header("Unit Info")]
    public string unitFullName;

    [Header("Rank Progression Settings")]

    [Tooltip("Her rütbede eklenecek ekstra can (Örn: 20f her rankta +20 HP verir)")]
    public float hpBonusPerRank = 20f;

    [Tooltip("Her rütbede hasar ne kadar artsýn? (Örn: 0.2f her rankta %20 artýþ saðlar)")]
    public float damageMultiplierPerRank = 0.2f;

    [Tooltip("Her rütbede alýnan hasar ne kadar azalsýn? (Örn: 0.1f her rankta %10 tanklýk saðlar)")]
    public float tankinessPerRank = 0.1f;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = data.maxHealth;

        // 2D Ayarlarý
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = data.moveSpeed;
    }
    protected virtual void Start()
    {
        // 0.1 saniye sonra kontrol et (Böylece gazi verisi atanmýþ olur)
        Invoke("CheckAndAssignName", 0.05f);
        UpdateRankVisuals();
    }
    public void TakeDamage(float amount, UnitType attackerType, int attackerRank)
    {
        float finalDamage = amount;

        PlayHitFeedback();

        // 1. SALDIRGAN RÜTBE BONUSU (Saldýrganýn rütbesine göre vurduðu hasar artar)
        // Senin Inspector'dan belirlediðin 'damageMultiplierPerRank' deðerini kullanýr.
        float attackerMultiplier = 1f + ((attackerRank - 1) * damageMultiplierPerRank);
        finalDamage *= attackerMultiplier;

        // 2. TAÞ-KAÐIT-MAKAS DENGESÝ & YAVAÞLATMA
        if (attackerType == UnitType.Archer && data.type == UnitType.Infantry)
        {
            finalDamage *= 1.5f;
        }
        else if (attackerType == UnitType.Infantry && data.type == UnitType.Cavalry)
        {
            finalDamage *= 1.5f;
            // Piyade Atlýya vurursa yavaþlatma mekaniði
            SlowDown(1f, 0.5f);
        }
        else if (attackerType == UnitType.Cavalry && data.type == UnitType.Archer)
        {
            finalDamage *= 1.5f;
        }

        // 3. SAVUNMA (TANKLIK) RÜTBE BONUSU (Bizim rütbemize göre aldýðýmýz hasar azalýr)
        // Senin Inspector'dan belirlediðin 'tankinessPerRank' deðerini kullanýr.
        float defenseMultiplier = 1f - ((currentRank - 1) * tankinessPerRank);
        // Hasarýn saçma bir þekilde eksiye düþmemesi veya 0 olmamasý için en az %10'unu almasýný saðlýyoruz.
        finalDamage *= Mathf.Max(0.1f, defenseMultiplier);

        // Caný azalt
        currentHealth -= finalDamage;

        // 4. ÝNTÝKAM MANTIÐI (Sadece Düþmanlar için)
        if (currentHealth > 0 && this is EnemyUnit && target == null)
        {
            ((EnemyUnit)this).FindBestTarget();
        }

        // 5. UI GÜNCELLEME
        UpdateHealthUI();

        if (currentHealth <= 0) Die();
    }

    void ApplyRankBonuses()
    {
        // Her rütbe atladýðýnda caný %20, hasarý %10 artýr gibi:
        currentHealth += data.maxHealth * 0.2f;
        // Not: Saldýrý bonusu için TakeDamage'da rütbeyi de hesaba katabiliriz
    }
    private void CheckAndAssignName()
    {
        // Eðer gazi listesinden bir isim gelmemiþse (yani unitFullName hala boþsa) rastgele ver
        if (string.IsNullOrEmpty(unitFullName))
        {
            unitFullName = NamingSystem.Instance.GenerateRandomName();
        }

        gameObject.name = unitFullName; // Hierarchy ismini her halükarda güncelle
    }

    void LegendaryRegenTick()
    {
        if (currentHealth <= 0) return;

        float maxHP = data.maxHealth + ((currentRank - 1) * hpBonusPerRank);

        if (currentHealth < maxHP)
        {
            currentHealth = Mathf.Min(currentHealth + healthRegen, maxHP);
            UpdateHealthUI(); 
        }
    }

    public void AddLegendaryDamage(float amount)
    {
        zoneDamageModifier += amount;
    }

    public void RemoveLegendaryDamage(float amount)
    {
        zoneDamageModifier -= amount;
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0) return;

        float totalMaxHP = data.maxHealth + ((currentRank - 1) * hpBonusPerRank);
        currentHealth = Mathf.Min(currentHealth + amount, totalMaxHP);
        UpdateHealthUI(); // Az önce yazdýðýmýz bar güncelleme fonksiyonu
    }
    public void UpdateRankVisuals()
    {
        if (rankIconImage == null) return;

        if (currentRank == 1) rankIconImage.sprite = rank1Sprite;
        else if (currentRank == 2) rankIconImage.sprite = rank2Sprite;
        else if (currentRank == 3) rankIconImage.sprite = rank3Sprite;
        if (currentRank == 4)
        {
            // 1. GÖRSEL AYARLAR
            rankIconImage.sprite = rank4Sprite;
            if (legendaryAura != null) legendaryAura.SetActive(true);
            if (nameText != null)
            {
                nameText.text = unitFullName;
                nameText.gameObject.SetActive(true);
            }

            // 2. TEK SEFERLÝK HASAR ARTIÞI
            if (!isLegendaryPowerApplied)
            {
                // Mevcut saldýrý gücüne kalýcý olarak bonusu ekle
                // Not: 'data.attackDamage' veya senin hasar deðiþkenin hangisiyse onu artýr
                // Eðer BaseUnit içinde 'currentDamage' gibi bir deðiþkenin varsa onu kullan
                isLegendaryPowerApplied = true;
                Debug.Log($"{unitFullName} Efsanevi oldu! Hasar kalýcý olarak {legendaryPowerBonus} arttý.");
            }

            // 3. CAN YENÝLEME DÖNGÜSÜ (Bu 2 saniyede bir devam eder)
            CancelInvoke("LegendaryRegenTick");
            InvokeRepeating("LegendaryRegenTick", 2f, 2f);
        }
        else
        {
            // Rütbe 4 deðilse her þeyi sýfýrla
            if (legendaryAura != null) legendaryAura.SetActive(false);
            if (nameText != null) nameText.gameObject.SetActive(false);
            CancelInvoke("LegendaryRegenTick");
            isLegendaryPowerApplied = false;
        }

        rankIconImage.gameObject.SetActive(currentRank > 1);

        float extraHP = (currentRank - 1) * hpBonusPerRank;

        // Eðer birim hayattaysa, rütbe alýnca canýný da biraz dolduralým:
        currentHealth = Mathf.Min(currentHealth + hpBonusPerRank, data.maxHealth + extraHP);

        Debug.Log($"{unitFullName} yeni rütbe ile güçlendi. Yeni Max Can: {data.maxHealth + extraHP}");
    }
    public void SlowDown(float amount, float duration)
    {
        // Eðer zaten yavaþlatýlmýþsa (veya ölüyse) iþlemi tekrarlama
        if (agent == null || !agent.isOnNavMesh) return;

        // Orijinal hýzý sakla (BaseUnit'te data.moveSpeed olduðunu varsayýyorum)
        float originalSpeed = data.moveSpeed;

        // Hýzý düþür (En az 0.1 olsun ki tamamen çakýlmasýn)
        agent.speed = Mathf.Max(0.1f, agent.speed - amount);

        // Süre bitince hýzý geri yüklemek için "ResetSpeed" çaðýr
        Invoke("ResetSpeed", duration);
    }

    private void ResetSpeed()
    {
        if (agent != null)
        {
            agent.speed = data.moveSpeed;
        }
    }

    public void PlayHitFeedback()
    {
        // Coroutine ile hýzlýca saða/sola yatýp düzelmesini saðlayalým
        StartCoroutine(HitTiltRoutine());
    }

    private System.Collections.IEnumerator HitTiltRoutine()
    {
        Quaternion originalRotation = transform.rotation;

        // Rastgele bir yöne (saða veya sola) hafifçe yatýr
        float randomTilt = Random.Range(0, 2) == 0 ? tiltAmount : -tiltAmount;
        transform.rotation = Quaternion.Euler(0, 0, randomTilt);

        yield return new WaitForSeconds(tiltDuration);

        // Eski haline geri döndür
        transform.rotation = originalRotation;
    }
    public void UpdateHealthUI()
    {
        if (healthBarGroup != null && healthBarFill != null)
        {
            // Barý görünür yap (Caný dolarken de barý görelim)
            healthBarGroup.SetActive(true);

            // Rütbe bonusuyla artan caný doðru orantýlamak için toplam caný hesaplýyoruz
            float totalMaxHP = data.maxHealth + ((currentRank - 1) * hpBonusPerRank);

            // Doluluk oranýný ayarla
            healthBarFill.fillAmount = currentHealth / totalMaxHP;

            // Barýn 3 saniye sonra otomatik gizlenmesi için (opsiyonel)
            CancelInvoke("HideHealthBar");
            Invoke("HideHealthBar", 3f);
        }
    }

    private void HideHealthBar()
    {
        if (healthBarGroup != null) healthBarGroup.SetActive(false);
    }

    protected virtual void Die()
    {
        // SelectionManager listesinden kendini temizlemesi için bir event veya doðrudan eriþim
        FindObjectOfType<SelectionManager>().selectedUnits.Remove(this as PlayerUnit);
        Destroy(gameObject);
    }
}