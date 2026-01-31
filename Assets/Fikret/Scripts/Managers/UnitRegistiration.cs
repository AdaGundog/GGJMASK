using UnityEngine;
using LevelEditor;
public class UnitRegistration : MonoBehaviour
{
    [Tooltip("Bu birim oyuncunun askeri mi? (Deðilse Düþman sayýlýr)")]
    public bool isPlayerUnit = true;
    public EnemyType unitType;

    private void Start()
    {
        // Doðar doðmaz kendini listeye yazdýr
        if (UnitManager.Instance != null)
        {
            UnitManager.Instance.RegisterUnit(this.gameObject, isPlayerUnit);
        }
    }

    private void OnDestroy()
    {
        // Ölünce veya yok olunca listeden adýný sildir
        if (UnitManager.Instance != null)
        {
            UnitManager.Instance.UnregisterUnit(this.gameObject, isPlayerUnit);
        }
    }
}