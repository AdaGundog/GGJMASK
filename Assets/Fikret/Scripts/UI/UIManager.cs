using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Oyun Ýçi Panelleri")]
    public GameObject infoPanel;      // Para vb. bilgilerin olduðu panel
    public GameObject actionsPanel;   // Butonlarýn olduðu panel

    [Header("Oyun Sonu Panelleri")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    [Header("Duraklatma (Pause)")]
    public GameObject pausePanel;     // Pause Paneli buraya
    private bool isPaused = false;

    private void Start()
    {
        // 1. Oyun Baþlarken Temizle:
        if (infoPanel) infoPanel.SetActive(true);
        if (actionsPanel) actionsPanel.SetActive(true);

        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false); // Pause kapalý baþlasýn

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

    private void Update()
    {
        // ESC tuþuna basýlýnca Pause aç/kapa
        // Ama oyun bittiyse (Zafer/Bozgun) Pause açýlmasýn
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (victoryPanel.activeSelf || defeatPanel.activeSelf) return;

            TogglePause();
        }
    }

    // --- PAUSE SÝSTEMÝ ---

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Duraklat
            if (pausePanel) pausePanel.SetActive(true);
            Time.timeScale = 0f; // Zamaný durdur
        }
        else
        {
            // Devam Et
            if (pausePanel) pausePanel.SetActive(false);
            Time.timeScale = 1f; // Zamaný akýt
        }
    }

    public void ResumeGame() // Butonla devam etmek için
    {
        isPaused = false;
        if (pausePanel) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // --- OLAY YÖNETÝMÝ ---

    private void OnBattleStarted()
    {
        if (infoPanel) infoPanel.SetActive(true);        // Para KALIYOR
        if (actionsPanel) actionsPanel.SetActive(false); // Butonlar GÝDÝYOR
    }

    private void OnVictory()
    {
        if (infoPanel) infoPanel.SetActive(false);
        if (actionsPanel) actionsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false); // Pause açýksa kapat

        if (victoryPanel) victoryPanel.SetActive(true);
    }

    private void OnDefeat()
    {
        if (infoPanel) infoPanel.SetActive(false);
        if (actionsPanel) actionsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false); // Pause açýksa kapat

        if (defeatPanel) defeatPanel.SetActive(true);
    }

    // --- BUTON FONKSÝYONLARI ---

    public void OnNextLevelClicked()
    {
        Time.timeScale = 1f; // Emin olmak için zamaný düzelt
        GameManager.Instance.LevelCompleted();
    }

    public void OnRetryClicked()
    {
        Time.timeScale = 1f; // Zamaný düzelt
        GameManager.Instance.RetryLevel();
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f; // Zamaný düzelt ki menü donuk baþlamasýn
        SceneManager.LoadScene(0);
    }
    public void OnQuitClicked()
    {
        Application.Quit();
    }
}