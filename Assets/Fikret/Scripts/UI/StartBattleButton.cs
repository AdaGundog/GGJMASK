using UnityEngine;
using UnityEngine.UI;

public class StartBattleButton : MonoBehaviour
{
    private Button myButton;

    private void Start()
    {
        myButton = GetComponent<Button>();
        // Butona týklandýðýnda ne yapacaðýný kodla baðlýyoruz (Sürükle býrak yok!)
        myButton.onClick.AddListener(StartTheWar);
    }

    private void StartTheWar()
    {
        // GameManager sahnede nerede olursa olsun onu bulur ve çalýþtýrýr
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartBattle();
        }
        else
        {
            Debug.LogError("GameManager bulunamadý!");
        }
    }
}