using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Tilemap ve Tile collider ayarlarini kontrol eden script
/// ObstacleTilemap objesine ekle
/// </summary>
public class TileColliderTest : MonoBehaviour
{
    public TileBase testTile; // WallTile'i buraya ata

    void Start()
    {
        Debug.Log("=== TILE COLLIDER TESTI ===");
        
        Tilemap tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("❌ Bu objede Tilemap yok!");
            return;
        }

        // Tilemap Collider kontrolu
        TilemapCollider2D tilemapCollider = GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null)
        {
            Debug.LogError("❌ TilemapCollider2D YOK! Ekle!");
        }
        else
        {
            Debug.Log("✅ TilemapCollider2D var");
            Debug.Log($"   Used By Composite: {tilemapCollider.usedByComposite}");
        }

        // Composite Collider kontrolu
        CompositeCollider2D compositeCollider = GetComponent<CompositeCollider2D>();
        if (compositeCollider != null)
        {
            Debug.Log("✅ CompositeCollider2D var");
            Debug.Log($"   Geometry Type: {compositeCollider.geometryType}");
            Debug.Log($"   Generation Type: {compositeCollider.generationType}");
        }

        // Rigidbody kontrolu
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Debug.Log($"✅ Rigidbody2D var - Body Type: {rb.bodyType}");
            if (rb.bodyType != RigidbodyType2D.Static)
            {
                Debug.LogWarning("⚠️ Rigidbody2D 'Static' olmali!");
            }
        }

        // Tile kontrolu
        if (testTile != null)
        {
            Debug.Log($"\n--- Test Tile Kontrolu: {testTile.name} ---");
            
            // Tile'i bir hucreye koy ve collider'ini kontrol et
            Vector3Int testCell = new Vector3Int(0, 0, 0);
            tilemap.SetTile(testCell, testTile);
            
            // Bir frame bekle ki collider olusabilsin
            Invoke("CheckTileCollider", 0.1f);
        }
        else
        {
            Debug.LogWarning("⚠️ Test Tile atanmamis! WallTile'i ata.");
        }
    }

    void CheckTileCollider()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        TilemapCollider2D tilemapCollider = GetComponent<TilemapCollider2D>();
        
        if (tilemapCollider != null)
        {
            // Collider sayisini kontrol et
            int colliderCount = 0;
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (var col in colliders)
            {
                if (col.enabled)
                {
                    colliderCount++;
                    Debug.Log($"   Collider {colliderCount}: {col.GetType().Name}");
                }
            }

            if (colliderCount == 0)
            {
                Debug.LogError("❌ KRITIK: Tile kondu ama collider olusturulmadi!");
                Debug.LogError("   Sebep: WallTile'in Collider Type'i 'None' olabilir!");
                Debug.LogError("   Cozum: WallTile asset'ini sec ve Collider Type'i 'Sprite' yap!");
            }
            else
            {
                Debug.Log($"✅ {colliderCount} collider aktif");
            }
        }

        // Test tile'i temizle
        tilemap.SetTile(new Vector3Int(0, 0, 0), null);
    }

    void OnDrawGizmos()
    {
        // Tilemap collider'larini gizmos ile goster
        TilemapCollider2D tilemapCollider = GetComponent<TilemapCollider2D>();
        if (tilemapCollider != null)
        {
            Gizmos.color = Color.green;
            // Collider bounds'unu ciz
            Bounds bounds = tilemapCollider.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
