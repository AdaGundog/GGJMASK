using UnityEngine;
using UnityEngine.AI;

public abstract class BaseUnit : MonoBehaviour
{
    [Header("Base Settings")]
    public UnitData data;
    public float currentHealth;
    protected NavMeshAgent agent;

    [Header("Combat State")]
    public BaseUnit target;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = data.maxHealth;

        // 2D Ayarlarý
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = data.moveSpeed;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        // Ortak ölüm mantýðý (Efekt çýkarma vb.)
        Destroy(gameObject);
    }
}