using UnityEngine;
using Unity.Cinemachine;

public class CameraMpvement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float smoothness = 5f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 3f;
    public float maxZoom = 15f;

    private float targetZoom;
    private Vector3 targetPosition;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        // WASD Inputlarýný al
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // Yeni hedef pozisyonu hesapla
        Vector3 moveDir = new Vector3(x, y, 0).normalized;
        targetPosition += moveDir * moveSpeed * Time.deltaTime;

        // Yumuþak hareket (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothness);
    }

    void HandleZoom()
    {
        // Fare tekerleði giriþini al
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // Yumuþak Zoom (Lerp)
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * smoothness);
    }
}