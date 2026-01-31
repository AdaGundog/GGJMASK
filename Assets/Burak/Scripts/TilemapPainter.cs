using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
public class TilemapPainter : MonoBehaviour
{
    [Header("Tilemap Ayarlarý")]
    public Tilemap obstacleTilemap;
    public TileBase wallTile;
    public Camera mainCamera;

    [Header("Mürekkep Ayarlarý")]
    public float inkAmount = 100f;
    public float costPerTile = 1f;

    [Header("Fýrça Ayarlarý")]
    public int brushRadius = 2;

    [Header("Ýmleç Ayarlarý")]
    public Texture2D brushCursor;
    public Vector2 cursorHotspot = Vector2.zero;

    [Header("Yaþam Süresi Ayarlarý")]
    public float tileLifetime = 30f; 
    public float fadeDuration = 5f;  

    private bool isPaintingMode = false;

    private class PaintedTile
    {
        public Vector3Int position;
        public float creationTime;
    }

    private List<PaintedTile> activeTiles = new List<PaintedTile>();

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) TogglePaintMode();

        if (isPaintingMode && Input.GetMouseButton(0) && inkAmount > 0)
        {
            Paint();
        }

        if (isPaintingMode && Input.GetMouseButtonDown(1)) TogglePaintMode();
        UpdateTilesLifecycle();
    }

    void TogglePaintMode()
    {
        isPaintingMode = !isPaintingMode;
        if (isPaintingMode) Cursor.SetCursor(brushCursor, cursorHotspot, CursorMode.Auto);
        else Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void Paint()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int centerCell = obstacleTilemap.WorldToCell(mouseWorldPos);

        for (int x = -brushRadius; x <= brushRadius; x++)
        {
            for (int y = -brushRadius; y <= brushRadius; y++)
            {
                Vector3Int currentCell = new Vector3Int(centerCell.x + x, centerCell.y + y, centerCell.z);

                if (Vector3Int.Distance(centerCell, currentCell) <= brushRadius)
                {
                    if (obstacleTilemap.GetTile(currentCell) == null && inkAmount > 0)
                    {
                        CreateTile(currentCell);
                    }
                }
            }
        }
    }

    void CreateTile(Vector3Int cellPos)
    {
        obstacleTilemap.SetTile(cellPos, wallTile);

        obstacleTilemap.SetTileFlags(cellPos, TileFlags.None);

        inkAmount -= costPerTile;

        activeTiles.Add(new PaintedTile
        {
            position = cellPos,
            creationTime = Time.time
        });
    }

    void UpdateTilesLifecycle()
    {
        for (int i = activeTiles.Count - 1; i >= 0; i--)
        {
            PaintedTile tile = activeTiles[i];
            float age = Time.time - tile.creationTime; 

            if (age >= tileLifetime)
            {
                obstacleTilemap.SetTile(tile.position, null);
                activeTiles.RemoveAt(i);
            }
            else if (age >= (tileLifetime - fadeDuration))
            {
                float remainingTime = tileLifetime - age;
                float alpha = remainingTime / fadeDuration;

                obstacleTilemap.SetColor(tile.position, new Color(1, 1, 1, alpha));
            }
        }
    }
}