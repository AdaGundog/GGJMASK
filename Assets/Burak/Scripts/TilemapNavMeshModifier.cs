using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;

/// <summary>
/// Tilemap'e NavMeshModifier ekleyerek NavMesh'in Tilemap'i engellemesini saglar
/// ObstacleTilemap objesine ekle
/// </summary>
[RequireComponent(typeof(Tilemap))]
public class TilemapNavMeshModifier : MonoBehaviour
{
    void Start()
    {
        // NavMeshModifier ekle (yoksa)
        NavMeshModifier modifier = GetComponent<NavMeshModifier>();
        if (modifier == null)
        {
            modifier = gameObject.AddComponent<NavMeshModifier>();
            Debug.Log("[TilemapNavMeshModifier] NavMeshModifier eklendi");
        }

        // Ayarlar
        modifier.overrideArea = true;
        modifier.area = 1; // Not Walkable area (1 = Not Walkable)
        
        Debug.Log("[TilemapNavMeshModifier] NavMeshModifier ayarlandi - Area: Not Walkable");
    }
}
