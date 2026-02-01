using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject mainMenuPanel; // Play, Settings, Credits, Quit butonlarýnýn olduðu ana grup
    public GameObject settingsPanel; // Ayarlar paneli
    public GameObject creditsPanel;  // Emeði geçenler paneli

    private void Start()
    {
        // Sahne açýldýðýnda garanti olsun diye sadece Ana Menüyü aç, diðerlerini kapat
        BackToMainMenu();
    }

    // --- PANEL YÖNETÝMÝ ---

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false); // Ana menüyü gizle
        settingsPanel.SetActive(true);  // Ayarlarý aç
        creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false); // Ana menüyü gizle
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);   // Credits'i aç
    }

    // Settings ve Credits içindeki "Geri Dön" (Back) butonuna bunu baðla
    public void BackToMainMenu()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);  // Ana menüyü tekrar göster
    }

    // --- OYUN FONKSÝYONLARI ---

    public void PlayGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLevelIndex = 1;
        }

        // Build Settings'deki sýraya göre 1. sahneyi yükler
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan Çýkýldý!");
        Application.Quit();
    }
}