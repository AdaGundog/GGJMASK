using UnityEngine;

public class Projectile : MonoBehaviour // Dosya adýn ProjectileArrow ise sýnýf adýný kontrol et
{
    public float speed = 15f;
    private BaseUnit target;
    private float damage;
    private UnitType projectileOwnerType;
    private int shooterRank; // EKSÝK OLAN DEÐÝÞKEN BU!

    // Setup fonksiyonunu rütbe bilgisini alacak þekilde güncelliyoruz
    public void Setup(BaseUnit _target, float _damage, UnitType _ownerType, int _rank)
    {
        target = _target;
        damage = _damage;
        projectileOwnerType = _ownerType;
        shooterRank = _rank; // Rütbeyi kaydet

        if (target != null)
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;
            transform.right = direction;
        }

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
        {
            // BURASI ARTIK HATA VERMEYECEK
            target.TakeDamage(damage, projectileOwnerType, shooterRank);
            Destroy(gameObject);
        }
    }
}