using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    private void Start()
    {
        // GameManager.Instance.CurrentMoney float olduðu için UI'a gönderirken int'e çeviriyoruz
        UpdateMoneyUI((int)GameManager.Instance.CurrentMoney);

        // GameManager'daki event 'int' beklediði için fonksiyonumuzun parametresi de 'int' olmalý
        GameManager.Instance.OnMoneyChanged += UpdateMoneyUI;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnMoneyChanged -= UpdateMoneyUI;
    }

    // PARAMETREYÝ TEKRAR 'int' YAPTIK (Hatanýn çözümü burasý)
    private void UpdateMoneyUI(int amount)
    {
        moneyText.text = "Gold: " + amount.ToString();
    }
}