using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;

/// <summary>
/// NavMesh ve Tilemap ayarlarini kontrol eden debug scripti
/// TilemapPainter objesine ekle ve Play'e bas
/// </summary>
public class NavMeshDebugHelper : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;
    public Tilemap obstacleTilemap;

    void Start()
    {
        Debug.Log("=== NAVMESH DEBUG BASLADI ===");
        CheckNavMeshSurface();
        CheckTilemap();
        CheckLayers();
        Debug.Log("=== NAVMESH DEBUG BITTI ===");
    }

    void CheckNavMeshSurface()
    {
        Debug.Log("\n--- NavMeshSurface Kontrolu ---");
        
        if (navMeshSurface == null)
        {
            Debug.LogError("❌ NavMeshSurface referansi YOK! TilemapPainter'a ata!");
            return;
        }

        Debug.Log("✅ NavMeshSurface referansi var");
        Debug.Log($"   Agent Type: {navMeshSurface.agentTypeID}");
        Debug.Log($"   Collect Objects: {navMeshSurface.collectObjects}");
        Debug.Log($"   Use Geometry: {navMeshSurface.useGeometry}");
        
        // Layer mask kontrolu
        string layerNames = "";
        for (int i = 0; i < 32; i++)
        {
            if ((navMeshSurface.layerMask & (1 << i)) != 0)
            {
                layerNames += LayerMask.LayerToName(i) + ", ";
            }
        }
        Debug.Log($"   Include Layers: {layerNames}");

        if (navMeshSurface.useGeometry.ToString() != "PhysicsColliders")
        {
            Debug.LogWarning("⚠️ Use Geometry 'Physics Colliders' olmali!");
        }
    }

    void CheckTilemap()
    {
        Debug.Log("\n--- Tilemap Kontrolu ---");
        
        if (obstacleTilemap == null)
        {
            Debug.LogError("❌ ObstacleTilemap referansi YOK!");
            return;
        }

        Debug.Log("✅ ObstacleTilemap referansi var");
        Debug.Log($"   Layer: {LayerMask.LayerToName(obstacleTilemap.gameObject.layer)}");
        Debug.Log($"   Order in Layer: {obstacleTilemap.GetComponent<TilemapRenderer>().sortingOrder}");

        // Collider kontrolu
        TilemapCollider2D tilemapCollider = obstacleTilemap.GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null)
        {
            Debug.LogError("❌ TilemapCollider2D YOK! ObstacleTilemap'e ekle!");
        }
        else
        {
            Debug.Log("✅ TilemapCollider2D var");
            Debug.Log($"   Used By Composite: {tilemapCollider.usedByComposite}");
        }

        // Composite Collider kontrolu
        CompositeCollider2D compositeCollider = obstacleTilemap.GetComponent<CompositeCollider2D>();
        if (compositeCollider != null)
        {
            Debug.Log("✅ CompositeCollider2D var (Performans icin iyi!)");
        }
        else
        {
            Debug.LogWarning("⚠️ CompositeCollider2D yok (Performans icin ekle)");
        }

        // Rigidbody kontrolu
        Rigidbody2D rb = obstacleTilemap.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Debug.Log($"✅ Rigidbody2D var - Body Type: {rb.bodyType}");
            if (rb.bodyType != RigidbodyType2D.Static)
            {
                Debug.LogWarning("⚠️ Rigidbody2D 'Static' olmali!");
            }
        }
    }

    void CheckLayers()
    {
        Debug.Log("\n--- Layer Uyumluluk Kontrolu ---");
        
        if (navMeshSurface == null || obstacleTilemap == null)
        {
            Debug.LogError("❌ Referanslar eksik, layer kontrolu yapilamiyor");
            return;
        }

        int tilemapLayer = obstacleTilemap.gameObject.layer;
        bool layerIncluded = (navMeshSurface.layerMask & (1 << tilemapLayer)) != 0;

        if (layerIncluded)
        {
            Debug.Log($"✅ Tilemap layer'i ({LayerMask.LayerToName(tilemapLayer)}) NavMesh'e dahil");
        }
        else
        {
            Debug.LogError($"❌ KRITIK: Tilemap layer'i ({LayerMask.LayerToName(tilemapLayer)}) NavMesh'e dahil DEGIL!");
            Debug.LogError("   Cozum: NavMeshSurface'te Include Layers'a bu layer'i ekle!");
        }
    }

    [ContextMenu("Manuel Kontrol")]
    public void ManualCheck()
    {
        Start();
    }
}
