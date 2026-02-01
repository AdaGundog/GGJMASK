using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("General Cursors")]
    public Texture2D normalCursor;
    public Texture2D clickCursor; // Týklayýnca (Sað veya Sol) görünecek imleç
    public Vector2 hotspot = Vector2.zero;

    private TilemapPainter painter;
    private bool isClicking = false;

    void Awake()
    {
        Instance = this;
        painter = FindObjectOfType<TilemapPainter>();
    }

    void Update()
    {
        // Eðer boyama modundaysak, genel imleç mantýðýný devre dýþý býrak
        if (painter != null && painter.isPaintingMode) return;

        HandleInput();
    }

    void HandleInput()
    {
        // Sol veya Sað týk basýldý mý?
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            SetClickCursor();
        }

        // Týk býrakýldý mý?
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            SetNormalCursor();
        }
    }

    public void SetNormalCursor()
    {
        Cursor.SetCursor(normalCursor, hotspot, CursorMode.Auto);
    }

    public void SetClickCursor()
    {
        Cursor.SetCursor(clickCursor, hotspot, CursorMode.Auto);
    }

    // Boyama modundan çýkýnca TilemapPainter bu fonksiyonu çaðýracak
    public void ResetToDefault()
    {
        SetNormalCursor();
    }
}