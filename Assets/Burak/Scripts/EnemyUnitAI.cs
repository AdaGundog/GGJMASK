using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyUnitAI : MonoBehaviour
{
    [Header("Kimlik")]
    public UnitType unitType;
    public UnitRank currentRank;

    [Header("Bileþenler")]
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

        agent.updateRotation = false;
        agent.updateUpAxis = false;
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
        float lowestScore = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);

            float score = dist;

            if (currentRank != UnitRank.Rookie)
            {
                PlayerUnit pUnit = enemy.GetComponent<PlayerUnit>();

                if (pUnit != null)
                {
                    if (IsCounterTarget(pUnit.type))
                    {
                        score -= 5.0f; 
                    }
                }
            }

            if (score < lowestScore)
            {
                lowestScore = score;
                bestTarget = enemy.transform;
            }
        }

        return bestTarget != null ? bestTarget : enemyCastle;
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
            Debug.Log(name + " vurdu: " + target.name);
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