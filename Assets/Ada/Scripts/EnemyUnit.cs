using UnityEngine;

public class EnemyUnit : BaseUnit
{
    [Header("Enemy Combat")]
    public GameObject arrowPrefab;
    private float lastAttackTime;

    [Header("Weapon Visuals")]
    public Transform spearTransform;
    public float pokeDistance = 0.6f;
    public float pokeSpeed = 0.05f;
    private Vector3 spearOriginalPos;

    // --- SES DEĞİŞKENİ ---
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        if (spearTransform != null)
        {
            spearOriginalPos = spearTransform.localPosition;
            spearTransform.gameObject.SetActive(false);
        }

        // --- SES AYARLARI ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
        }
    }

    void Update()
    {
        // --- SES KONTROLÜ ---
        HandleMovementSound();

        //  Hazırlık aşamasındaysak dur
        if (GameManager.Instance.CurrentState != GameState.Battle)
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        if (agent != null) agent.isStopped = false;

        // 1. Hedef kontrolü
        if (target == null || target.currentHealth <= 0)
        {
            FindBestTarget();
            if (target == null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        // 2. SALDIRI VEYA TAKİP
        if (distance <= data.attackRange)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            TryAttack();
        }
        else
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
            }
        }
    }

    // --- YENİ EKLENEN SES FONKSİYONU ---
    void HandleMovementSound()
    {
        if (audioSource == null || agent == null) return;

        // NavMeshAgent hızıyla kontrol ediyoruz
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.time = Random.Range(0f, audioSource.clip.length);
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    private System.Collections.IEnumerator SpearPokeRoutine()
    {
        if (target == null || spearTransform == null) yield break;

        spearTransform.gameObject.SetActive(true);

        Vector3 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spearTransform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 startPos = spearOriginalPos;
        Vector3 punchPos = spearOriginalPos + new Vector3(pokeDistance, 0, 0);

        float elapsed = 0;
        while (elapsed < pokeSpeed)
        {
            spearTransform.localPosition = Vector3.Lerp(startPos, punchPos, elapsed / pokeSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        while (elapsed < pokeSpeed * 2)
        {
            spearTransform.localPosition = Vector3.Lerp(punchPos, startPos, elapsed / (pokeSpeed * 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        spearTransform.localPosition = spearOriginalPos;
        spearTransform.gameObject.SetActive(false);
    }

    public void FindBestTarget()
    {
        PlayerUnit[] players = FindObjectsOfType<PlayerUnit>();
        if (players.Length == 0) { target = null; return; }

        BaseUnit bestTarget = null;
        float minDistance = Mathf.Infinity;

        // GRUP 1: Bana yakından saldıranlar
        foreach (PlayerUnit p in players)
        {
            if (p == null || p.currentHealth <= 0) continue;
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist > 15f) continue;

            if (p.target == this && dist < 5f)
            {
                if (dist < minDistance) { minDistance = dist; bestTarget = p; }
            }
        }
        if (bestTarget != null) { target = bestTarget; return; }

        // GRUP 2: Avantajlı olduğum birimler
        minDistance = Mathf.Infinity;
        foreach (PlayerUnit p in players)
        {
            if (p == null || p.currentHealth <= 0) continue;
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist > 15f) continue;

            if (CheckAdvantage(data.type, p.data.type))
            {
                if (dist < minDistance) { minDistance = dist; bestTarget = p; }
            }
        }
        if (bestTarget != null) { target = bestTarget; return; }

        // GRUP 3: En yakın birim
        minDistance = Mathf.Infinity;
        foreach (PlayerUnit p in players)
        {
            if (p == null || p.currentHealth <= 0) continue;
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist > 15f) continue;

            if (dist < minDistance) { minDistance = dist; bestTarget = p; }
        }

        target = bestTarget;
    }

    bool CheckAdvantage(UnitType attacker, UnitType defender)
    {
        if (attacker == UnitType.Archer && defender == UnitType.Infantry) return true;
        if (attacker == UnitType.Infantry && defender == UnitType.Cavalry) return true;
        if (attacker == UnitType.Cavalry && defender == UnitType.Archer) return true;
        return false;
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + data.attackRate)
        {
            if (data.type == UnitType.Archer && arrowPrefab != null)
            {
                GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                arrowObj.GetComponent<Projectile>().Setup(target, data.attackDamage, data.type, currentRank);
            }
            else
            {
                if (spearTransform != null) StartCoroutine(SpearPokeRoutine());
                target.TakeDamage(data.attackDamage, data.type, currentRank);
            }
            lastAttackTime = Time.time;
        }
    }
}