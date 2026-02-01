using UnityEngine;
using System.Collections.Generic;

public class LegendaryZone : MonoBehaviour
{
    public float healthRegen = 2f;
    public float damageBonus = 2f;
    public float regenInterval = 2f;

    // Alanýn içindeki birimleri takip etmek için liste
    private List<BaseUnit> unitsInZone = new List<BaseUnit>();

    private void Start()
    {
        // 2 saniyede bir can yenileme döngüsünü baþlat
        InvokeRepeating("ApplyRegenToAll", regenInterval, regenInterval);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseUnit unit = other.GetComponent<BaseUnit>();
        if (unit != null && !unitsInZone.Contains(unit))
        {
            unitsInZone.Add(unit);

            // --- +2 SALDIRI BONUSU (Giriþte bir kerelik) ---
            // BaseUnit içinde 'extraDamageFromLegendary' deðiþkenini kullanýyoruz
            // Not: Eðer bu deðiþken yoksa BaseUnit'e eklemelisin.
            unit.AddLegendaryDamage(damageBonus);

            Debug.Log($"{unit.unitFullName} alana girdi, +2 hasar kazandý.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        BaseUnit unit = other.GetComponent<BaseUnit>();
        if (unit != null && unitsInZone.Contains(unit))
        {
            // --- HASAR BONUSUNU GERÝ AL (Çýkýþta) ---
            unit.RemoveLegendaryDamage(damageBonus);

            unitsInZone.Remove(unit);
            Debug.Log($"{unit.unitFullName} alandan çýktý, hasar bonusu silindi.");
        }
    }

    void ApplyRegenToAll()
    {
        foreach (BaseUnit unit in unitsInZone)
        {
            if (unit != null)
            {
                // Can yenileme fonksiyonunu çaðýr
                unit.Heal(healthRegen);
            }
        }
    }
}