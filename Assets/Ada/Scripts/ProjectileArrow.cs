using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    private BaseUnit target;
    private float damage;
    public UnitType projectileOwnerType;

    // Oku yaratan okçu bu fonksiyonu çaðýracak
    public void Setup(BaseUnit _target, float _damage, UnitType _ownerType)
    {
        target = _target;
        damage = _damage;
        projectileOwnerType = _ownerType;

        // Okun hedefe doðru bakmasýný saðlar (Saða çizildiði varsayýmýyla)
        Vector2 direction = (target.transform.position - transform.position).normalized;
        transform.right = direction;

        // Ok havada asýlý kalmasýn diye 5 saniye sonra kendi kendini yok eder
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Hedefe doðru ilerle
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        // Hedefe çarptý mý?
        if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
        {
            // HEDEF HALA ORADA MI? (Kritik kontrol)
            if (target != null)
            {
                target.TakeDamage(damage, projectileOwnerType);
            }

            Destroy(gameObject); // Vursa da vurmasa da ok yok olmalý
        }
    }
}