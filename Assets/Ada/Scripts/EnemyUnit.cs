using UnityEngine;

public class EnemyUnit : BaseUnit
{
    [Header("Enemy Combat")]
    public GameObject arrowPrefab; // Inspector'dan ok prefabını buraya sürükle!
    private float lastAttackTime;

    void Update()
    {
        // 🛑 GÜVENLİK KİLİDİ: Eğer Savaş Modunda değilsek hiçbir şey yapma!
        if (GameManager.Instance.CurrentState != GameState.Battle)
        {
            // Eğer NavMeshAgent kullanıyorsan, onu da zorla durdur ki kaymasın
            if (agent != null) agent.isStopped = true;
            return;
        }

        // NavMeshAgent'ın kilidini aç (Savaş başladıysa koşsun)
        if (agent != null) agent.isStopped = false;

        // 1. Hedef yoksa yeni birini ara ve dur
        if (target == null)
        {
            FindBestTarget();
            if (agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        // 2. KAÇMA MANTIĞI: Eğer hedef avantajlıysa ve çok yakınsa zıt yöne git
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

        // 3. SALDIRI VEYA TAKİP MANTIĞI (Eksik olan kısım burasıydı)
        if (distance <= data.attackRange)
        {
            // Menzildeysek dur ve saldır
            if (agent.isOnNavMesh) agent.isStopped = true;
            TryAttack();
        }
        else
        {
            // Menzil dışındaysak hedefe doğru yürü
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

            // Çok uzaktaysa bu birimi değerlendirmeye bile alma
            if (distance > 20f) continue;

            float currentPriority = 0;

            // 1. ÖNCELİK: Mesafe (Temel puan: Yakınlık iyidir)
            // Mesafeyi ters çeviriyoruz ki yakın olan daha çok puan alsın
            currentPriority += (20f - distance);

            // 2. ÖNCELİK: Avantajlı olduğu birim (+10 Puan)
            if (CheckAdvantage(data.type, p.data.type))
            {
                currentPriority += 10f;
            }

            // 3. ÖNCELİK: Ona saldıran birim (+15 Puan)
            // (Eğer Player'ın hedefi bu düşmansa, player ona saldırıyor demektir)
            if (p.target == this)
            {
                currentPriority += 15f;
            }

            // 4. DEZAVANTAJ DURUMU: Kaçma Mantığı
            // Eğer dezavantajlı olduğu birim çok yakınsa (örn: 3 birim), önceliği düşür
            if (CheckAdvantage(p.data.type, data.type) && distance < 3f)
            {
                currentPriority -= 20f;
                // Eğer çok tehlikeliyse kaçma moduna geç (Aşağıda açıklayacağım)
            }

            if (currentPriority > highestPriority)
            {
                highestPriority = currentPriority;
                bestTarget = p;
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
