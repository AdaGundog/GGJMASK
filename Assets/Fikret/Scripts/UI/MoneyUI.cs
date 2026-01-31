using UnityEngine;
using TMPro; // TextMeshPro kullanýyoruz (Unity'nin modern yazý sistemi)

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText; // Inspector'dan sürükle

    private void Start()
    {
        // Oyun açýlýnca parayý yaz
        UpdateMoneyUI(GameManager.Instance.CurrentMoney);

        // GameManager'daki para deðiþim eventine abone ol
        GameManager.Instance.OnMoneyChanged += UpdateMoneyUI;
    }

    private void OnDestroy()
    {
        // Obje yok olunca aboneliði iptal et (Hata almamak için þart)
        if (GameManager.Instance != null)
            GameManager.Instance.OnMoneyChanged -= UpdateMoneyUI;
    }

    private void UpdateMoneyUI(int amount)
    {
        moneyText.text = "Gold: " + amount.ToString();
    }
}