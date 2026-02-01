using UnityEngine;
using UnityEngine.AI;

public class PlayerUnit : BaseUnit
{
    [Header("Selection Visuals")]
    public SpriteRenderer selectionCircle;
    public Color defaultColor = Color.green;
    public Color selectedColor = Color.blue;

    [Header("Combat Visuals")]
    public GameObject arrowPrefab; // Okçu için
    public Transform spearTransform; // Piyade mızrağı için (Inspector'dan sürükle)
    public float pokeDistance = 0.6f;
    public float pokeSpeed = 0.05f;

    public bool isSelected;
    private bool isAutoAttacking = false;
    private float lastAttackTime;
    private Vector3 spearOriginalPos;

    protected override void Start()
    {
        base.Start();
        if (selectionCircle != null) selectionCircle.color = defaultColor;

        // Mızrağın başlangıç yerini kaydet
        if (spearTransform != null) spearOriginalPos = spearTransform.localPosition;

        if (spearTransform != null) spearTransform.gameObject.SetActive(false);
    }

    public void MoveTo(Vector3 destination)
    {
        if (this == null || agent == null) return;
        target = null;
        isAutoAttacking = false;
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
        }
    }

    public void SetTarget(BaseUnit enemy)
    {
        target = enemy;
        isAutoAttacking = true;
        if (agent != null) agent.isStopped = false;
    }

    public void SetSelection(bool state)
    {
        isSelected = state;
        if (selectionCircle != null)
            selectionCircle.color = isSelected ? selectedColor : defaultColor;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Battle)
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

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
        else if (isAutoAttacking)
        {
            target = FindNearestEnemy();
            if (target == null)
            {
                isAutoAttacking = false;
                if (agent.isOnNavMesh) agent.isStopped = true;
            }
        }
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + data.attackRate)
        {
            if (data.type == UnitType.Archer)
            {
                GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                arrowObj.GetComponent<Projectile>().Setup(target, data.attackDamage, data.type, currentRank);
            }
            else
            {
                // --- PİYADE GÖRSEL EFEKT (POKE) ---
                if (spearTransform != null) StartCoroutine(SpearPokeRoutine());

                target.TakeDamage(data.attackDamage, data.type, currentRank);
            }

            lastAttackTime = Time.time;
        }
    }

    private System.Collections.IEnumerator SpearPokeRoutine()
    {
        if (target == null) yield break;

        // 1. MIZRAĞI GÖRÜNÜR YAP
        spearTransform.gameObject.SetActive(true);

        // 2. DÜŞMANA DOĞRU DÖNDÜR
        // İki birim arasındaki yönü hesapla
        Vector3 direction = (target.transform.position - transform.position).normalized;
        // Açıyı hesapla (Atan2 ile radyanı dereceye çeviriyoruz)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Mızrağı o yöne çevir
        spearTransform.rotation = Quaternion.Euler(0, 0, angle);

        // 3. SAPLAMA HAREKETİ
        // Mızrağın "ileri" gitmesi için kendi yerel sağ (right) yönünü kullanıyoruz
        Vector3 startPos = spearOriginalPos;
        Vector3 punchPos = spearOriginalPos + new Vector3(pokeDistance, 0, 0);

        float elapsed = 0;
        while (elapsed < pokeSpeed)
        {
            spearTransform.localPosition = Vector3.Lerp(startPos, punchPos, elapsed / pokeSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. GERİ ÇEKİLME
        elapsed = 0;
        while (elapsed < pokeSpeed * 2)
        {
            spearTransform.localPosition = Vector3.Lerp(punchPos, startPos, elapsed / (pokeSpeed * 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 5. MIZRAĞI TEKRAR GİZLE
        spearTransform.localPosition = spearOriginalPos;
        spearTransform.gameObject.SetActive(false);
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

    
}