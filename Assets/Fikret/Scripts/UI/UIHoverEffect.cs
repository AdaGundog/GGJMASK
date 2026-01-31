using UnityEngine;
using UnityEngine.EventSystems; // Mouse olaylarý için þart

public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Ayarlar")]
    public float hoverScale = 1.1f;   // Üzerine gelince ne kadar büyüsün? (%10)
    public float clickScale = 0.95f;  // Týklayýnca ne kadar küçülsün?
    public float speed = 10f;         // Animasyon hýzý

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        // Yumuþak geçiþ (Lerp) ile boyutu deðiþtir
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    // Mouse üzerine gelince
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;

        // Ses çalmak istersen buraya ekleyebilirsin:
        // AudioManager.Instance.PlaySFX("Hover_Sound");
    }

    // Mouse üzerinden gidince
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    // Týklayýnca (Basýlý tutunca)
    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * clickScale;
    }

    // Týklamayý býrakýnca
    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale; // Veya hoverScale'e dönebilirsin
    }
}