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
        if (healthBarGroup != null)
        {
            healthBarGroup.SetActive(true);

            // Rütbe bonusuyla artan caný doðru orantýlamak için:
            float totalMaxHP = data.maxHealth + ((currentRank - 1) * hpBonusPerRank);
            healthBarFill.fillAmount = currentHealth / totalMaxHP;

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
    protected virtual void Die()
    {
        // SelectionManager listesinden kendini temizlemesi için bir event veya doðrudan eriþim
        FindObjectOfType<SelectionManager>().selectedUnits.Remove(this as PlayerUnit);
        Destroy(gameObject);
    }
}