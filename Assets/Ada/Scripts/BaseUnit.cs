using UnityEngine;
using UnityEngine.AI;

public abstract class BaseUnit : MonoBehaviour
{
    [Header("Base Settings")]
    public UnitData data;
    public float currentHealth;
    protected NavMeshAgent agent;
    [SerializeField] private GameObject healthBarGroup;
    public UnityEngine.UI.Image healthBarFill;

    [Header("Combat State")]
    public BaseUnit target;

    [Header("Rank System")]
    [Range(1, 3)]
    public int currentRank = 1;
    public UnityEngine.UI.Image rankIconImage;

    [Header("Rank Sprites")]
    public Sprite rank1Sprite;
    public Sprite rank2Sprite;
    public Sprite rank3Sprite;

    [Header("Unit Info")]
    public string unitFullName;

    [Header("Rank Progression Settings")]
    [Tooltip("Her rutbede eklenecek ekstra can")]
    public float hpBonusPerRank = 20f;

    [Tooltip("Her rutbede hasar ne kadar artsin?")]
    public float damageMultiplierPerRank = 0.2f;

    [Tooltip("Her rutbede alinan hasar ne kadar azalsin?")]
    public float tankinessPerRank = 0.1f;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = data.maxHealth;

        // 2D Ayarlari
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = data.moveSpeed;
    }

    protected virtual void Start()
    {
        // Eger isim atanmamissa NamingSystem'dan cek
        if (string.IsNullOrEmpty(unitFullName))
        {
            unitFullName = NamingSystem.Instance.GenerateRandomName();
            gameObject.name = unitFullName;
        }

        UpdateRankVisuals();
    }

    public void TakeDamage(float amount, UnitType attackerType, int attackerRank)
    {
        float finalDamage = amount;

        // 1. SALDIRGAN RUTBE BONUSU
        float attackerMultiplier = 1f + ((attackerRank - 1) * damageMultiplierPerRank);
        finalDamage *= attackerMultiplier;

        // 2. TAS-KAGIT-MAKAS DENGESI & YAVASLAT
        if (attackerType == UnitType.Archer && data.type == UnitType.Infantry)
        {
            finalDamage *= 1.5f;
        }
        else if (attackerType == UnitType.Infantry && data.type == UnitType.Cavalry)
        {
            finalDamage *= 1.5f;
            SlowDown(1f, 0.5f);
        }
        else if (attackerType == UnitType.Cavalry && data.type == UnitType.Archer)
        {
            finalDamage *= 1.5f;
        }

        // 3. SAVUNMA (TANKLIK) RUTBE BONUSU
        float defenseMultiplier = 1f - ((currentRank - 1) * tankinessPerRank);
        finalDamage *= Mathf.Max(0.1f, defenseMultiplier);

        // Cani azalt
        currentHealth -= finalDamage;

        // 4. INTIKAM MANTIGI (Sadece Dusmanlar icin)
        if (currentHealth > 0 && this is EnemyUnit && target == null)
        {
            ((EnemyUnit)this).FindBestTarget();
        }

        // 5. UI GUNCELLEME - EKRAN DISI KONTROLU ILE
        if (healthBarGroup != null)
        {
            // Ekran disi kontrolu - sadece kamera gorus alanindaysa goster
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            bool isOnScreen = screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height;
            
            if (isOnScreen)
            {
                healthBarGroup.SetActive(true);

                // Rutbe bonusuyla artan cani dogru orantılamak icin:
                float totalMaxHP = data.maxHealth + ((currentRank - 1) * hpBonusPerRank);
                healthBarFill.fillAmount = currentHealth / totalMaxHP;

                CancelInvoke("HideHealthBar");
                Invoke("HideHealthBar", 3f);
            }
            else
            {
                // Ekran disindaysa health bar'i gizle
                healthBarGroup.SetActive(false);
            }
        }

        if (currentHealth <= 0) Die();
    }

    void HideHealthBar()
    {
        if (healthBarGroup != null)
        {
            healthBarGroup.SetActive(false);
        }
    }

    void ApplyRankBonuses()
    {
        currentHealth += data.maxHealth * 0.2f;
    }

    public void UpdateRankVisuals()
    {
        if (rankIconImage == null) return;

        if (currentRank == 1) rankIconImage.sprite = rank1Sprite;
        else if (currentRank == 2) rankIconImage.sprite = rank2Sprite;
        else if (currentRank == 3) rankIconImage.sprite = rank3Sprite;

        rankIconImage.gameObject.SetActive(currentRank > 1);

        float extraHP = (currentRank - 1) * hpBonusPerRank;
        currentHealth = Mathf.Min(currentHealth + hpBonusPerRank, data.maxHealth + extraHP);

        Debug.Log($"{unitFullName} yeni rutbe ile guclendi. Yeni Max Can: {data.maxHealth + extraHP}");
    }

    public void SlowDown(float amount, float duration)
    {
        if (agent == null || !agent.isOnNavMesh) return;

        float originalSpeed = data.moveSpeed;
        agent.speed = Mathf.Max(0.1f, agent.speed - amount);

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
        FindObjectOfType<SelectionManager>().selectedUnits.Remove(this as PlayerUnit);
        Destroy(gameObject);
    }
}
