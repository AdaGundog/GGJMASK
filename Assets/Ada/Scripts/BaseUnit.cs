using UnityEngine;
using UnityEngine.AI;

public abstract class BaseUnit : MonoBehaviour
{
    [Header("Base Settings")]
    public UnitData data;
    public float currentHealth;
    protected NavMeshAgent agent;
    public UnityEngine.UI.Image healthBarFill; // Inspector'dan yeþil resmi buraya sürükle

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

    public void TakeDamage(float amount, UnitType attackerType)
    {
        float finalDamage = amount;

        // TAÞ-KAÐIT-MAKAS HESABI (Bu kýsým zaten sende var)
        if (attackerType == UnitType.Archer && data.type == UnitType.Infantry) finalDamage *= 1.5f;
        else if (attackerType == UnitType.Infantry && data.type == UnitType.Cavalry) finalDamage *= 1.5f;
        else if (attackerType == UnitType.Cavalry && data.type == UnitType.Archer) finalDamage *= 1.5f;

        currentHealth -= finalDamage;

        if (healthBarFill != null)
        {
            // KANVAS GÖRÜNÜR YAPMA
            var canvasObj = healthBarFill.canvas != null ? healthBarFill.canvas.gameObject : healthBarFill.transform.parent.gameObject;
            canvasObj.SetActive(true);

            // KRÝTÝK DÜZELTME: (float) ekleyerek ondalýklý hesaplama yapmasýný saðlýyoruz
            healthBarFill.fillAmount = (float)currentHealth / (float)data.maxHealth;

            // Debug ile kontrol edelim
            Debug.Log($"{gameObject.name} Can: {currentHealth}, Oran: {healthBarFill.fillAmount}");
        }

        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        // SelectionManager listesinden kendini temizlemesi için bir event veya doðrudan eriþim
        FindObjectOfType<SelectionManager>().selectedUnits.Remove(this as PlayerUnit);
        Destroy(gameObject);
    }
}