using UnityEngine;

public class EnemyUnit : BaseUnit
{
    [Header("Enemy Combat")]
    public GameObject arrowPrefab; // Inspector'dan ok prefabını buraya sürükle!
    private float lastAttackTime;

    [Header("Weapon Visuals")]
    public Transform spearTransform; // Inspector'dan düşmanın mızrağını buraya koy
    public float pokeDistance = 0.6f;
    public float pokeSpeed = 0.05f;
    private Vector3 spearOriginalPos;

    protected override void Start()
    {
        base.Start();
        // Mızrağı başta gizle ve yerini kaydet
        if (spearTransform != null)
        {
            spearOriginalPos = spearTransform.localPosition;
            spearTransform.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        //  Hazırlık aşamasındaysak dur
        if (GameManager.Instance.CurrentState != GameState.Battle)
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        if (agent != null) agent.isStopped = false;

        // 1. Hedef kontrolü: Hedef yoksa veya öldüyse yeni hedef bul
        if (target == null || target.currentHealth <= 0)
        {
            FindBestTarget(); // Senin istediğin sıralama burada çalışıyor
            if (target == null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        // 2. SALDIRI VEYA TAKİP (Kaçış kodu silindi!)
        if (distance <= data.attackRange)
        {
            // Menzile girdiyse dur ve vur
            if (agent.isOnNavMesh) agent.isStopped = true;
            TryAttack();
        }
        else
        {
            // Menzil dışındaysa korkmadan üstüne git
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
            }
        }
    }

    private System.Collections.IEnumerator SpearPokeRoutine()
    {
        if (target == null || spearTransform == null) yield break;

        spearTransform.gameObject.SetActive(true);

        // Düşmanın mızrağını bizim birliğimize doğru döndür
        Vector3 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spearTransform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 startPos = spearOriginalPos;
        Vector3 punchPos = spearOriginalPos + new Vector3(pokeDistance, 0, 0);

        // İleri
        float elapsed = 0;
        while (elapsed < pokeSpeed)
        {
            spearTransform.localPosition = Vector3.Lerp(startPos, punchPos, elapsed / pokeSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Geri
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
        if (bestTarget != null) { target = bestTarget; return; } // Bulduysak fonksiyondan çık!

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

        // GRUP 3: En yakın birim (Dezavantajlılar dahil)
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

    // Yardımcı fonksiyon: Tip avantajını kontrol eder
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
                // --- OKÇU SALDIRISI ---
                GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                arrowObj.GetComponent<Projectile>().Setup(target, data.attackDamage, data.type, currentRank);
            }
            else
            {
                // --- PİYADE/ATLI MIZRAK EFEKTİ ---
                if (spearTransform != null) StartCoroutine(SpearPokeRoutine());

                target.TakeDamage(data.attackDamage, data.type, currentRank);
            }

            lastAttackTime = Time.time;
        }
    }
}
