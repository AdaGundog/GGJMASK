using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        // GameManager yaþýyorsa onu sýfýrla ki Level 1'den baþlasýn
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLevelIndex = 1;
            // Diðer resetlemeler GameManager'ýn Start'ýnda veya Level yüklendiðinde yapýlýr
        }

        // Oyun Sahnesini Yükle (Sahne adýn "GameLevel_1" ise onu yaz, deðilse Build Settings'den bak)
        // Seninle "GameLevel1" veya benzeri bir isimde karar kýlmýþtýk.
        // Eðer build settings'de index 1 ise direkt 1 de yazabilirsin.
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan Çýkýldý!");
        Application.Quit();
    }
}