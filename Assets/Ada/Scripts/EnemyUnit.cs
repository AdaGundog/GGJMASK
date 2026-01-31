using UnityEngine;

public class EnemyUnit : BaseUnit
{
    [Header("Enemy Combat")]
    public GameObject arrowPrefab; // Inspector'dan ok prefabını buraya sürükle!
    private float lastAttackTime;

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Battle)
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        if (agent != null) agent.isStopped = false;

        // Hedef yoksa veya öldüyse yeni hedef bul
        if (target == null || target.currentHealth <= 0)
        {
            FindBestTarget();
            if (target == null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        // KAÇMA VE YENİDEN HEDEFLEME MANTIĞI
        // Eğer mevcut hedef bize karşı avantajlıysa VE çok yakınsa:
        if (CheckAdvantage(target.data.type, data.type) && distance < 3f)
        {
            // Önce etrafta daha güvenli başka biri var mı diye bak (Anlık kontrol)
            BaseUnit oldTarget = target;
            FindBestTarget();

            // Eğer hala aynı (tehlikeli) hedefe kilitliysek (yani başka çare yoksa) KAÇ
            if (target == oldTarget)
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
            // Eğer FindBestTarget bize daha güvenli birini bulduysa, kaçmayı bırakıp ona yönelecek (Update devam edecek)
        }

        // SALDIRI VE TAKİP
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

    void FindBestTarget()
    {
        PlayerUnit[] players = FindObjectsOfType<PlayerUnit>();
        BaseUnit bestTarget = null;
        float highestPriority = -Mathf.Infinity;

        foreach (PlayerUnit p in players)
        {
            if (p == null || p.currentHealth <= 0) continue;

            float distance = Vector2.Distance(transform.position, p.transform.position);
            if (distance > 20f) continue; // Görüş mesafesi dışı

            float currentPriority = 0;

            // 1. TEMEL PUAN: Mesafe (Yakınlık her zaman çok önemli)
            // Yakın olan birimlere 0-20 arası puan verir.
            currentPriority += (20f - distance);

            // 2. STRATEJİK PUAN: Avantaj Durumu
            // Sadece birim makul bir mesafedeyse (örn: 10 birim) avantaj puanı ekle. 
            // Çok uzaktaki avantajlı birim için tüm orduyu yarıp geçmesin.
            if (CheckAdvantage(data.type, p.data.type))
            {
                if (distance < 10f) currentPriority += 15f; // Yakındaysa büyük öncelik
                else currentPriority += 5f; // Uzaktaysa küçük öncelik
            }

            // 3. TEHLİKE DURUMU: Dezavantaj
            // Eğer hedef bize karşı avantajlıysa puanı ciddi oranda düşür.
            // Bu, düşmanın "en son çare" olarak bu birime saldırmasını sağlar.
            if (CheckAdvantage(p.data.type, data.type))
            {
                currentPriority -= 25f;
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
