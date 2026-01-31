using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Oyun Ýçi Panelleri")]
    public GameObject infoPanel;      // Para vb. bilgilerin olduðu panel (Info_Panel)
    public GameObject actionsPanel;   // Butonlarýn olduðu panel (Actions_Panel)

    [Header("Oyun Sonu Panelleri")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    private void Start()
    {
        // 1. Oyun Baþlarken:
        if (infoPanel) infoPanel.SetActive(true);       // Para görünsün
        if (actionsPanel) actionsPanel.SetActive(true); // Butonlar görünsün

        // Oyun sonu ekranlarý gizli olsun
        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);

        // 2. Olaylara Abone Ol
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBattleStarted += OnBattleStarted;
            GameManager.Instance.OnVictory += OnVictory;
            GameManager.Instance.OnDefeat += OnDefeat;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBattleStarted -= OnBattleStarted;
            GameManager.Instance.OnVictory -= OnVictory;
            GameManager.Instance.OnDefeat -= OnDefeat;
        }
    }

    // --- OLAY YÖNETÝMÝ ---

    private void OnBattleStarted()
    {
        // ÝSTEK 1: Savaþ baþlayýnca sadece para kalsýn, butonlar gitsin.
        if (infoPanel) infoPanel.SetActive(true);       // Para KALIYOR
        if (actionsPanel) actionsPanel.SetActive(false); // Butonlar GÝDÝYOR
    }

    private void OnVictory()
    {
        // ÝSTEK 2: Oyun bitince sadece sonuç paneli kalsýn.
        if (infoPanel) infoPanel.SetActive(false);     // Para GÝDÝYOR
        if (actionsPanel) actionsPanel.SetActive(false); // Butonlar zaten yoktu ama garanti olsun

        if (victoryPanel) victoryPanel.SetActive(true); // Sadece ZAFER AÇILIYOR
    }

    private void OnDefeat()
    {
        // ÝSTEK 2: Oyun bitince sadece sonuç paneli kalsýn.
        if (infoPanel) infoPanel.SetActive(false);     // Para GÝDÝYOR
        if (actionsPanel) actionsPanel.SetActive(false);

        if (defeatPanel) defeatPanel.SetActive(true);   // Sadece BOZGUN AÇILIYOR
    }

    // --- BUTON FONKSÝYONLARI ---
    public void OnNextLevelClicked()
    {
        GameManager.Instance.LevelCompleted();
    }

    public void OnRetryClicked()
    {
        GameManager.Instance.RetryLevel();
    }

    public void OnMainMenuClicked()
    {
        SceneManager.LoadScene(0);
    }
}