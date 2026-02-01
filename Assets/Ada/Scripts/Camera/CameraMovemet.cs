using UnityEngine;
using Unity.Cinemachine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float smoothness = 5f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 3f;
    public float maxZoom = 15f;

    [Header("Boundary Settings")]
    public BoxCollider2D mapBounds; // Sürükle býrak yapacaðýmýz alan
    private float minX, maxX, minY, maxY;

    private float targetZoom;
    private Vector3 targetPosition;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
        targetPosition = transform.position;

        if (mapBounds != null)
        {
            CalculateBoundaries();
        }
    }

    void CalculateBoundaries()
    {
        // Collider'ýn sýnýrlarýný al
        minX = mapBounds.bounds.min.x;
        maxX = mapBounds.bounds.max.x;
        minY = mapBounds.bounds.min.y;
        maxY = mapBounds.bounds.max.y;
    }

    void LateUpdate() // Hareketten sonra sýnýr kontrolü yapmak için LateUpdate daha iyidir
    {
        HandleMovement();
        HandleZoom();
        
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // 1. Hedef pozisyonu hesapla
        Vector3 moveDir = new Vector3(x, y, 0).normalized;
        targetPosition += moveDir * moveSpeed * Time.deltaTime;

        // 2. HEDEF POZÝSYONU SINIRLA (Kamerayý deðil)
        if (mapBounds != null)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            // Hedefin gidebileceði sýnýrlarý belirle
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX + camWidth, maxX - camWidth);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY + camHeight, maxY - camHeight);
        }

        // 3. Kamerayý sýnýrlanmýþ hedefe doðru yumuþakça kaydýr
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothness);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * smoothness);
    }

    
}