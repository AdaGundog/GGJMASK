using UnityEngine;
using Unity.Cinemachine;

public class RTSCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float smoothness = 0.1f; // DÝKKAT: Bu deðer artýk daha küçük olmalý (0.1 - 0.3 arasý iyidir)

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 3f;
    public float maxZoom = 15f;

    private float targetZoom;
    private Vector3 currentVelocity; // SmoothDamp için gerekli
    private CinemachineCamera vCam;

    void Start()
    {
        vCam = GetComponent<CinemachineCamera>();
        if (vCam != null)
        {
            targetZoom = vCam.Lens.OrthographicSize;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        // 1. Girdileri Al
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(x, y, 0).normalized;

        // 2. Girdiyi Yumuþat (SmoothDamp ile)
        // Bu yöntem, duvara çarptýðýnda "hedef" biriktirmez, o yüzden kayma yapmaz.
        // transform.position'ý direkt etkiliyoruz.

        // Eðer hiç tuþa basýlmýyorsa anýnda durmasý için kontrol (Opsiyonel, daha keskin duruþ saðlar)
        if (inputDir.magnitude < 0.01f)
        {
            // Tuþu býraktýðýnda çok az kayýp dursun
        }

        // Hareketi uygula
        Vector3 targetMove = inputDir * moveSpeed;

        // Bu fonksiyon pozisyonu deðil, hýzý yumuþatýr. Confiner dostudur.
        transform.position = Vector3.SmoothDamp(transform.position, transform.position + targetMove, ref currentVelocity, smoothness);
    }

    void HandleZoom()
    {
        if (vCam == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // Zoom için Lerp kullanmaya devam edebiliriz, orada sýnýr sorunu yok
        float newSize = Mathf.Lerp(vCam.Lens.OrthographicSize, targetZoom, Time.deltaTime * 5f);
        vCam.Lens.OrthographicSize = newSize;
    }
}