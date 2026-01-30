using UnityEngine;
using UnityEngine.AI;


public class PlayerUnit : BaseUnit
{
    public bool isSelected;

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
                // Attack(target); // Buraya saldýrý hýzý kontrolü gelecek
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
            }
        }
    }
}