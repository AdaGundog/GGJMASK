using UnityEngine;
using UnityEngine.AI;


public class PlayerUnit : BaseUnit
{
    public bool isSelected;
    private float lastAttackTime;
    public GameObject arrowPrefab; // Inspector'dan hazýrladýðýn prefab'ý buraya sürükle
    public void MoveTo(Vector3 destination)
    {
        target = null; // Manuel hareket hedefleri iptal eder
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    public void SetTarget(BaseUnit enemy)
    {
        target = enemy;
        agent.isStopped = false;
    }

    void Update()
    {
        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance <= data.attackRange)
            {
                agent.isStopped = true;
                TryAttack();
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
            }
        }
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + data.attackRate)
        {
            // UnitData'da bir enum veya bool ile okçu olup olmadýðýný kontrol et
            if (data.type == UnitType.Archer)
            {
                // Oku yarat
                GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                // Okun içindeki Setup fonksiyonunu çalýþtýr
                arrowObj.GetComponent<Projectile>().Setup(target, data.attackDamage, data.type);
            }
            else
            {
                // Piyadeyse eskisi gibi doðrudan hasar ver
                target.TakeDamage(data.attackDamage, data.type);
            }

            lastAttackTime = Time.time;
        }
    }
}