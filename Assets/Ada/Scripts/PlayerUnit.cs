using UnityEngine;
using UnityEngine.AI;


public class PlayerUnit : BaseUnit
{

    [Header("Selection Visuals")]
    public SpriteRenderer selectionCircle; // Inspector'dan halkayı buraya sürükle
    public Color defaultColor = Color.green;
    public Color selectedColor = Color.blue;

    public bool isSelected;
    private bool isAutoAttacking = false; // Birim şu an otomatik modda mı?
    private float lastAttackTime;
    public GameObject arrowPrefab; // Inspector'dan hazırladığın prefab'ı buraya sürükle

    protected override void Start()
    {
        base.Start();
        // İlk başta halkayı yeşil yapıyoruz
        if (selectionCircle != null)
        {
            selectionCircle.color = defaultColor;
        }
    }
    public void MoveTo(Vector3 destination)
    {
        if (this == null || agent == null) return;

        target = null;
        isAutoAttacking = false; // Manuel hareket emri gelirse otomatiği kapat

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
        }
    }

    public void SetTarget(BaseUnit enemy)
    {
        target = enemy;
        isAutoAttacking = true; // Bir düşmana saldır dendiği an "Otomatik Mod" açılır
        if (agent != null) agent.isStopped = false;
    }

    public void SetSelection(bool state)
    {
        isSelected = state;

        if (selectionCircle != null)
        {
            // Seçiliyse Mavi, değilse Yeşil
            selectionCircle.color = isSelected ? selectedColor : defaultColor;
        }
    }

    void Update()
    {
        // 🛑 Savaş başlamadıysa hareket etme
        if (GameManager.Instance.CurrentState != GameState.Battle)
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        // 1. MANUEL HEDEF VEYA OTOMATİK HEDEF VARSA
        if (target != null && target.currentHealth > 0)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);

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
        // 2. HEDEF ÖLDÜYSE VE OTOMATİK MOD AÇIKSA
        else if (isAutoAttacking)
        {
            target = FindNearestEnemy();

            // Eğer etrafta hiç düşman kalmadıysa otomatiği kapat ve dur
            if (target == null)
            {
                isAutoAttacking = false;
                if (agent.isOnNavMesh) agent.isStopped = true;
            }
        }
    }

    BaseUnit FindNearestEnemy()
    {
        // Sahnede UnitManager aracılığıyla tüm düşmanları alalım
        var enemies = UnitManager.Instance.activeEnemyUnits;
        BaseUnit nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemyObj in enemies)
        {
            if (enemyObj == null) continue;

            float dist = Vector2.Distance(transform.position, enemyObj.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = enemyObj.GetComponent<BaseUnit>();
            }
        }

        // Sadece belirli bir görüş mesafesindeyse (Örn: 15 birim) saldırsın
        // Tüm haritayı koşup gitmemesi için bu mesafe kontrolü iyidir.
        if (minDistance > 15f) return null;

        return nearest;
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + data.attackRate)
        {
            // UnitData'da bir enum veya bool ile okçu olup olmadığını kontrol et
            if (data.type == UnitType.Archer)
            {
                // Oku yarat
                GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                // Okun içindeki Setup fonksiyonunu çalıştır
                arrowObj.GetComponent<Projectile>().Setup(target, data.attackDamage, data.type, currentRank);
            }
            else
            {
                // Piyadeyse eskisi gibi doğrudan hasar ver
                target.TakeDamage(data.attackDamage, data.type, currentRank);
            }

            lastAttackTime = Time.time;
        }
    }
}