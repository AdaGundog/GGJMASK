using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyUnitAI : MonoBehaviour
{
    [Header("Kimlik")]
    public UnitType unitType;
    public UnitRank currentRank;

    [Header("Bile�enler")]
    public NavMeshAgent agent;

    private Transform myCastle;
    private Transform enemyCastle;
    private Transform currentTarget;

    [Header("Durum")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    private bool isRetreating = false;
    private bool isHealing = false;

    public void Initialize(UnitRank rank, Transform myBase, Transform enemyBase)
    {
        currentRank = rank;
        myCastle = myBase;
        enemyCastle = enemyBase;

        if (rank == UnitRank.Veteran) maxHealth *= 1.2f;
        if (rank == UnitRank.Elite) maxHealth *= 1.5f;
        currentHealth = maxHealth;

        // Agent yoksa otomatik bul
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }

    void Update()
    {
        if (currentHealth <= 0 || isHealing) return;

        if (currentRank == UnitRank.Elite && !isRetreating)
        {
            if (currentHealth < maxHealth * 0.10f) 
            {
                StartRetreat();
                return;
            }
        }

        if (isRetreating)
        {
            if (Vector3.Distance(transform.position, myCastle.position) < 2.0f)
            {
                StartCoroutine(HealRoutine());
            }
            return;
        }

        FindAndEngageTarget();
        FaceTarget();
    }

    void FindAndEngageTarget()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = ScanForTarget();
        }

        if (currentTarget == null)
        {
            agent.SetDestination(enemyCastle.position);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToTarget <= attackRange)
        {
            agent.ResetPath();
            Attack(currentTarget);
        }
        else
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    Transform ScanForTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerUnit");

        if (enemies.Length == 0) return enemyCastle;

        Transform bestTarget = null;
        float highestPriority = -Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            PlayerUnit pUnit = enemy.GetComponent<PlayerUnit>();
            if (pUnit == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            
            // Çok uzaktaki hedefleri atla
            if (distance > 20f) continue;

            // ÖNCELİK HESAPLAMA
            float priority = 0;

            // 1. MESAFE (Yakın = İyi) - %40
            priority += (20f - distance) * 2f;

            // 2. TİP AVANTAJI (Avantajlı hedef = İyi) - %40
            if (IsCounterTarget(pUnit.data.type))
            {
                priority += 40f; // Avantajlı hedefe öncelik ver
            }
            else if (IsWeakAgainst(pUnit.data.type))
            {
                priority -= 30f; // Dezavantajlı hedeften kaç
            }

            // 3. CAN DURUMU (Düşük can = Kolay hedef) - %20
            if (pUnit.currentHealth < pUnit.data.maxHealth * 0.3f)
            {
                priority += 20f; // Ölmek üzere olan hedefi bitir
            }

            // 4. RANK BONUSU
            if (currentRank != UnitRank.Rookie)
            {
                // Veteran ve Elite daha akıllı hedef seçer
                if (IsCounterTarget(pUnit.data.type))
                {
                    priority += 10f;
                }
            }

            if (priority > highestPriority)
            {
                highestPriority = priority;
                bestTarget = enemy.transform;
            }
        }

        return bestTarget != null ? bestTarget : enemyCastle;
    }

    // Yeni fonksiyon: Dezavantajlı mı?
    bool IsWeakAgainst(UnitType enemyType)
    {
        if (unitType == UnitType.Infantry && enemyType == UnitType.Archer) return true;
        if (unitType == UnitType.Cavalry && enemyType == UnitType.Infantry) return true;
        if (unitType == UnitType.Archer && enemyType == UnitType.Cavalry) return true;
        return false;
    }

    bool IsCounterTarget(UnitType enemyType)
    {

        if (unitType == UnitType.Infantry && enemyType == UnitType.Cavalry) return true;

        if (unitType == UnitType.Cavalry && enemyType == UnitType.Archer) return true;

        if (unitType == UnitType.Archer && enemyType == UnitType.Infantry) return true;

        return false;
    }

    void StartRetreat()
    {
        isRetreating = true;
        agent.speed *= 1.2f; 
        agent.SetDestination(myCastle.position);
    }

    IEnumerator HealRoutine()
    {
        isRetreating = false;
        isHealing = true;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        agent.isStopped = true;

        yield return new WaitForSeconds(5.0f); 

        currentHealth = maxHealth;

        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        agent.isStopped = false;
        agent.speed /= 1.2f; 

        isHealing = false;
    }

    void Attack(Transform target)
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            // Gerçek hasar ver (BaseUnit sistemini kullan)
            PlayerUnit playerUnit = target.GetComponent<PlayerUnit>();
            if (playerUnit != null)
            {
                // Taş-Kağıt-Makas hasarı
                float baseDamage = 10f; // Temel hasar
                playerUnit.TakeDamage(baseDamage, unitType, (int)currentRank);
                Debug.Log($"{name} ({unitType}) vurdu: {target.name} - Hasar: {baseDamage}");
            }
            
            lastAttackTime = Time.time;
        }
    }

    void FaceTarget()
    {
        if (agent.velocity.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (agent.velocity.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}