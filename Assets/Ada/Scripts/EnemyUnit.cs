using UnityEngine;

public class EnemyUnit : BaseUnit
{
    [Header("Enemy Combat")]
    public GameObject arrowPrefab; // Inspector'dan ok prefabını buraya sürükle!
    private float lastAttackTime;

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

    public void FindBestTarget()
    {
        PlayerUnit[] players = FindObjectsOfType<PlayerUnit>();
        BaseUnit bestTarget = null;
        float minDistance = Mathf.Infinity;

        // Önce en kritik grubu bulalım (Bana saldıranlar)
        // Eğer okçuysak bu grubu atlayıp direkt avantaj grubuna bakabiliriz (isteğe bağlı)
        foreach (PlayerUnit p in players)
        {
            if (p == null || p.currentHealth <= 0) continue;
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist > 15f) continue;

            // GRUP 1: Bana yakından saldıranlar (En yüksek öncelik)
            if (p.target == this && dist < 5f)
            {
                if (dist < minDistance) { minDistance = dist; bestTarget = p; }
            }
        }

        // Eğer bana saldıran yoksa GRUP 2'ye bak: Avantajlı olduğum birimler
        if (bestTarget == null)
        {
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
        }

        // O da yoksa GRUP 3: Geri kalan her şey (Dezavantajlı dahil en yakın birim)
        if (bestTarget == null)
        {
            minDistance = Mathf.Infinity;
            foreach (PlayerUnit p in players)
            {
                if (p == null || p.currentHealth <= 0) continue;
                float dist = Vector2.Distance(transform.position, p.transform.position);
                if (dist > 15f) continue;

                if (dist < minDistance) { minDistance = dist; bestTarget = p; }
            }
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
            // Eğer düşman verisinde tipi okçu olarak ayarlandıysa
            if (data.type == UnitType.Archer && arrowPrefab != null)
            {
                // Oku yarat
                GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

                // Oku hedefe (oyuncuya) odakla
                arrowObj.GetComponent<Projectile>().Setup(target, data.attackDamage, data.type, currentRank);
                Debug.Log(gameObject.name + " ok fırlattı!");
            }
            else
            {
                // Yakın dövüşçü ise doğrudan hasar ver
                target.TakeDamage(data.attackDamage, data.type, currentRank);
            }

            lastAttackTime = Time.time;
        }
    }
}
