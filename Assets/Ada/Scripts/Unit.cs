using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [Header("Settings")]
    public UnitData data; // Yarattýðýn ScriptableObject'i buraya sürükleyeceksin

    [Header("State")]
    public Unit targetEnemy; // Saldýrýlacak hedef
    public bool isSelected = false;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // 2D RTS ayarlarý
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Verileri ScriptableObject'ten çek
        if (data != null)
        {
            agent.speed = data.moveSpeed;
        }
    }
    private void Start()
    {
    }
    void Update()
    {
        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distance <= data.attackRange)
            {
                // Menzildeyiz, dur ve saldýr
                agent.isStopped = true;
            }
            else
            {
                // Menzil dýþýndayýz, takibe devam et ve hareketin açýk olduðundan emin ol
                agent.isStopped = false;
                agent.SetDestination(targetEnemy.transform.position);
            }
        }
    }

    public void MoveTo(Vector3 destination)
    {
        targetEnemy = null; // Hedefi býrak

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false; // DURDURMA MODUNDAN ÇIK (Kritik satýr!)
            agent.SetDestination(destination);
        }
    }
    public void SetTarget(Unit enemy)
    {
        targetEnemy = enemy;
    }
}