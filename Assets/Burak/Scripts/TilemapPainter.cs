using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using NavMeshPlus.Components;
using System.Collections.Generic;
public class TilemapPainter : MonoBehaviour
{
    [Header("Game Mananeger")]
    public GameManager gameManager;

    [Header("Tilemap Ayarlari")]
    public Tilemap obstacleTilemap;
    public TileBase wallTile;
    public Camera mainCamera;

    [Header("NavMesh Ayarlari")]
    public NavMeshSurface navMeshSurface; // NavMesh'i guncellemek icin

    [Header("Murekkep Ayarlari")]
    // Artik kendi ink'i yok, GameManager'dan aliyor
    public float costPerTile = 0.25f;

    [Header("Firca Ayarlari")]
    public int brushRadius = 2;

    [Header("Imlec Ayarlari")]
    public Texture2D brushCursor;
    public Vector2 cursorHotspot = Vector2.zero;

    [Header("Yasam Suresi Ayarlari")]
    public float tileLifetime = 30f; 
    public float fadeDuration = 5f;  

    public bool isPaintingMode = false;

    private class PaintedTile
    {
        public Vector3Int position;
        public float creationTime;
        public bool lifetimeStarted; // Omur basladi mi?
    }

    private List<PaintedTile> activeTiles = new List<PaintedTile>();

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // Sahne ilk açıldığında GameManager'ı bulmaya çalış
        if (gameManager == null)
        {
            gameManager = Object.FindFirstObjectByType<GameManager>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TogglePaintMode();
        }

        if (isPaintingMode && Input.GetMouseButton(0))
        {
            // --- HATA VEREN KISMI BÖYLE GÜNCELLE ---
            // Eğer referans hala boşsa, Singleton (Instance) üzerinden son kez dene
            if (gameManager == null) gameManager = GameManager.Instance;

            if (gameManager == null)
            {
                // Eğer hala bulamadıysa (GameManager objesi sahnede yoksa) hata ver ama Update'i durdur
                return;
            }

            if (gameManager.CurrentMoney <= 0)
            {
                return;
            }

            Paint();
        }

        if (isPaintingMode && Input.GetMouseButtonDown(1)) TogglePaintMode();
        UpdateTilesLifecycle();
    }

    void TogglePaintMode()
    {
        isPaintingMode = !isPaintingMode;

        if (isPaintingMode)
        {
            // Boyama modu imleci
            Cursor.SetCursor(brushCursor, cursorHotspot, CursorMode.Auto);
        }
        else
        {
            // Boyama modundan çıkınca genel CursorManager'ı devreye sok
            if (CursorManager.Instance != null)
                CursorManager.Instance.ResetToDefault();
            else
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    void Paint()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("[TilemapPainter] Mouse UI uzerinde, boyama iptal edildi");
            return;
        }

        if (obstacleTilemap == null)
        {
            Debug.LogError("[TilemapPainter] obstacleTilemap atanmamis!");
            return;
        }

        if (wallTile == null)
        {
            Debug.LogError("[TilemapPainter] wallTile atanmamis!");
            return;
        }

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int centerCell = obstacleTilemap.WorldToCell(mouseWorldPos);

        Debug.Log($"[TilemapPainter] Boyama deneniyor: Cell {centerCell}, Para: {gameManager.CurrentMoney}");

        int tilesCreated = 0;
        for (int x = -brushRadius; x <= brushRadius; x++)
        {
            for (int y = -brushRadius; y <= brushRadius; y++)
            {
                Vector3Int currentCell = new Vector3Int(centerCell.x + x, centerCell.y + y, centerCell.z);

                if (Vector3Int.Distance(centerCell, currentCell) <= brushRadius)
                {
                    // GameManager'dan para kontrolu
                    if (obstacleTilemap.GetTile(currentCell) == null && gameManager != null && gameManager.CurrentMoney >= costPerTile)
                    {
                        CreateTile(currentCell);
                        tilesCreated++;
                    }
                }
            }
        }

        if (tilesCreated > 0)
        {
            Debug.Log($"[TilemapPainter] {tilesCreated} kare olusturuldu!");
            
            // Tum tile'lar olustuktan SONRA NavMesh'i bir kez guncelle
            UpdateNavMesh();
        }
    }

    void CreateTile(Vector3Int cellPos)
    {
        // GameManager'dan para kontrolu ve harcama
        if(gameManager != null && gameManager.CurrentMoney >= costPerTile)
        {
            obstacleTilemap.SetTile(cellPos, wallTile);
            obstacleTilemap.SetTileFlags(cellPos, TileFlags.None);

            // GameManager'dan parayi harca
            gameManager.SpendMoney(costPerTile);

            activeTiles.Add(new PaintedTile
            {
                position = cellPos,
                creationTime = Time.time,
                lifetimeStarted = false // Henuz omur baslamadi
            });

            // NavMesh guncellemeyi her tile'da degil, Paint() sonunda yapacagiz
        }
    }

    void UpdateNavMesh()
    {
        if (navMeshSurface != null)
        {
            // NavMesh'i guncelle
            navMeshSurface.BuildNavMesh();
            Debug.Log("[TilemapPainter] NavMesh guncellendi!");
            
            // TUM DUSMANLARIN PATH'LERINI YENIDEN HESAPLAT
            UpdateAllEnemyPaths();
            
            // Collider kontrolu
            TilemapCollider2D collider = obstacleTilemap.GetComponent<TilemapCollider2D>();
            if (collider == null)
            {
                Debug.LogError("[TilemapPainter] HATA: ObstacleTilemap'te TilemapCollider2D yok! Ekle!");
            }
            else
            {
                Debug.Log("[TilemapPainter] TilemapCollider2D mevcut - OK");
            }
        }
        else
        {
            Debug.LogError("[TilemapPainter] KRITIK HATA: NavMeshSurface referansi atanmamis! Dusmanlar boyali alandan gecebilir.");
            Debug.LogError("[TilemapPainter] Cozum: TilemapPainter objesine NavMeshSurface'i ata!");
        }
    }

    void UpdateAllEnemyPaths()
    {
        // Tum dusmanlari bul
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyUnit");
        int updatedCount = 0;
        int repositionedCount = 0;

        foreach (GameObject enemy in enemies)
        {
            UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                // Eger NavMesh'te degilse, en yakin NavMesh noktasina koy
                if (!agent.isOnNavMesh)
                {
                    UnityEngine.AI.NavMeshHit hit;
                    if (UnityEngine.AI.NavMesh.SamplePosition(enemy.transform.position, out hit, 5.0f, UnityEngine.AI.NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position); // Dusmani NavMesh'e isınla
                        repositionedCount++;
                        Debug.Log($"[TilemapPainter] {enemy.name} NavMesh'e yeniden yerlestirildi");
                    }
                    else
                    {
                        Debug.LogWarning($"[TilemapPainter] {enemy.name} icin yakin NavMesh noktasi bulunamadi!");
                    }
                }

                // Path'i yeniden hesaplat
                if (agent.isOnNavMesh)
                {
                    // Mevcut hedefi sakla
                    Vector3 currentDestination = agent.destination;
                    
                    // Path'i sifirla ve yeniden hesaplat
                    agent.ResetPath();
                    
                    // Eger bir hedefi varsa, yeniden ayarla (NavMesh guncellendiginde yeni path hesaplanir)
                    if (currentDestination != Vector3.zero)
                    {
                        agent.SetDestination(currentDestination);
                    }
                    
                    updatedCount++;
                }
            }
        }

        Debug.Log($"[TilemapPainter] {updatedCount} dusmanin path'i guncellendi, {repositionedCount} dusman yeniden yerlestirildi!");
    }

    void UpdateTilesLifecycle()
    {
        // Savas baslamadiysa tile'larin omrunu baslatma
        if (gameManager != null && gameManager.CurrentState != GameState.Battle)
        {
            return; // Preparation modundayken tile'lar sonsuza kadar kalir
        }

        bool needsNavMeshUpdate = false;

        for (int i = activeTiles.Count - 1; i >= 0; i--)
        {
            PaintedTile tile = activeTiles[i];
            
            // Savas baslayinca omru baslat
            if (!tile.lifetimeStarted)
            {
                tile.lifetimeStarted = true;
                tile.creationTime = Time.time; // Omru simdi baslat
            }

            float age = Time.time - tile.creationTime; 

            if (age >= tileLifetime)
            {
                obstacleTilemap.SetTile(tile.position, null);
                activeTiles.RemoveAt(i);
                needsNavMeshUpdate = true; // Tile silindi, NavMesh'i guncelle
            }
            else if (age >= (tileLifetime - fadeDuration))
            {
                float remainingTime = tileLifetime - age;
                float alpha = remainingTime / fadeDuration;

                obstacleTilemap.SetColor(tile.position, new Color(1, 1, 1, alpha));
            }
        }

        // Eger tile silindiyse NavMesh'i guncelle
        if (needsNavMeshUpdate)
        {
            UpdateNavMesh();
        }
    }
}
