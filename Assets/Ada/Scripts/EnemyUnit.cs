using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : BaseUnit
{
    void Update()
    {
        if (target == null)
        {
            SearchForTarget();
        }
        else
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= data.attackRange)
            {
                agent.isStopped = true;
                // Attack(target);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
            }
        }
    }

    void SearchForTarget()
    {
        // En yakýn oyuncu birimini bul
        PlayerUnit[] players = FindObjectsOfType<PlayerUnit>();
        float minDistance = Mathf.Infinity;
        PlayerUnit closestPlayer = null;

        foreach (PlayerUnit p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestPlayer = p;
            }
        }

        if (closestPlayer != null && minDistance < 10f) // 10f fark etme menzilidir
        {
            target = closestPlayer;
        }
    }
}
