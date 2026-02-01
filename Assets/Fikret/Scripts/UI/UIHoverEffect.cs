using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio; // Mixer Grubu tanýmlamak için bu þart!

[RequireComponent(typeof(AudioSource))]
public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Mixer Ayarý (Önemli!)")]
    public AudioMixerGroup sfxGroup; // Inspector'da buraya SFX grubunu sürükleyeceksin

    [Header("Görsel Ayarlar")]
    public float hoverScale = 1.1f;
    public float clickScale = 0.95f;
    public float speed = 10f;

    [Header("Ses Dosyalarý")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovered = false;
    private AudioSource myAudioSource;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.playOnAwake = false;
        myAudioSource.spatialBlend = 0;

        // --- ÝÞTE SÝHÝRLÝ KOD BURASI ---
        // Eðer Inspector'dan bir grup seçtiysen, AudioSource'u o gruba baðlar.
        if (sfxGroup != null)
        {
            myAudioSource.outputAudioMixerGroup = sfxGroup;
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        targetScale = originalScale * hoverScale;
        if (hoverSound != null) myAudioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * clickScale;
        if (clickSound != null) myAudioSource.PlayOneShot(clickSound);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = isHovered ? (originalScale * hoverScale) : originalScale;
    }
}