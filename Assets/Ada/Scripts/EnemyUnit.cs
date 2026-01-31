using UnityEngine;

public class EnemyUnit : BaseUnit
{
    [Header("Enemy Combat")]
    public GameObject arrowPrefab; // Inspector'dan ok prefabýný buraya sürükle!
    private float lastAttackTime;

    void Update()
    {
        // 1. Hedef yoksa yeni birini ara ve dur
        if (target == null)
        {
            FindBestTarget();
            if (agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        // 2. KAÇMA MANTIÐI: Eðer hedef avantajlýysa ve çok yakýnsa zýt yöne git
        if (CheckAdvantage(target.data.type, data.type) && distance < 2.5f)
        {
            Vector2 runDirection = (transform.position - target.transform.position).normalized;
            Vector2 escapePoint = (Vector2)transform.position + runDirection * 3f;

            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(escapePoint);
            }
            return;
        }

        // 3. SALDIRI VEYA TAKÝP MANTIÐI (Eksik olan kýsým burasýydý)
        if (distance <= data.attackRange)
        {
            // Menzildeysek dur ve saldýr
            if (agent.isOnNavMesh) agent.isStopped = true;
            TryAttack();
        }
        else
        {
            // Menzil dýþýndaysak hedefe doðru yürü
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
            }
        }
    }

    void FindBestTarget()
    {
        PlayerUnit[] players = FindObjectsOfType<PlayerUnit>();
        BaseUnit bestTarget = null;
        float highestPriority = -Mathf.Infinity;

        foreach (PlayerUnit p in players)
        {
            float distance = Vector2.Distance(transform.position, p.transform.position);

            // Çok uzaktaysa bu birimi deðerlendirmeye bile alma
            if (distance > 20f) continue;

            float currentPriority = 0;

            // 1. ÖNCELÝK: Mesafe (Temel puan: Yakýnlýk iyidir)
            // Mesafeyi ters çeviriyoruz ki yakýn olan daha çok puan alsýn
            currentPriority += (20f - distance);

            // 2. ÖNCELÝK: Avantajlý olduðu birim (+10 Puan)
            if (CheckAdvantage(data.type, p.data.type))
            {
                currentPriority += 10f;
            }

            // 3. ÖNCELÝK: Ona saldýran birim (+15 Puan)
            // (Eðer Player'ýn hedefi bu düþmansa, player ona saldýrýyor demektir)
            if (p.target == this)
            {
                currentPriority += 15f;
            }

            // 4. DEZAVANTAJ DURUMU: Kaçma Mantýðý
            // Eðer dezavantajlý olduðu birim çok yakýnsa (örn: 3 birim), önceliði düþür
            if (CheckAdvantage(p.data.type, data.type) && distance < 3f)
            {
                currentPriority -= 20f;
                // Eðer çok tehlikeliyse kaçma moduna geç (Aþaðýda açýklayacaðým)
            }

            if (currentPriority > highestPriority)
            {
                highestPriority = currentPriority;
                bestTarget = p;
            }
        }

        target = bestTarget;
    }

    // Yardýmcý fonksiyon: Tip avantajýný kontrol eder
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
            // Eðer düþman verisinde tipi okçu olarak ayarlandýysa
            if (data.type == UnitType.Archer && arrowPrefab != null)
            {
                // Oku yarat
                GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

                // Oku hedefe (oyuncuya) odakla
                arrowObj.GetComponent<Projectile>().Setup(target, data.attackDamage, data.type, currentRank);
                Debug.Log(gameObject.name + " ok fýrlattý!");
            }
            else
            {
                // Yakýn dövüþçü ise doðrudan hasar ver
                target.TakeDamage(data.attackDamage, data.type, currentRank);
            }

            lastAttackTime = Time.time;
        }
    }
}
