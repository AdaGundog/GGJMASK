using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public UnitType type;
    public float maxHealth;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float attackRate;
}