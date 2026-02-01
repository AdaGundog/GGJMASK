using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    private void Start()
    {
        BackToMainMenu();

        // Menü açýldýðýnda mouse imlecini görünür yap ve kilidini aç (Garanti olsun)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayGame()
    {
        // --- KRÝTÝK NOKTA BURASI ---
        // Eðer GameManager yaþýyorsa (ki DontDestroy olduðu için yaþýyordur),
        // ona "Hafýzaný temizle" emrini veriyoruz.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameData();
        }
        else
        {
            // Eðer oyun ilk defa açýlýyorsa ve GameManager sahnede yoksa endiþelenme,
            // Oyun sahnesine gidince GameManager kendi Awake fonksiyonunda zaten sýfýrdan kurulacak.
        }

        // Þimdi tertemiz bir sayfayla oyuna girebiliriz
        SceneManager.LoadScene(1);
    }

    // ... Diðer panel fonksiyonlarýn (OpenSettings, QuitGame vs.) aynen kalsýn ...

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Çýkýþ yapýlýyor...");
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}