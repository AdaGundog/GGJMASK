using UnityEngine;
using System.Collections.Generic;

public class NamingSystem : MonoBehaviour
{
    public static NamingSystem Instance;

    [Header("Names")]
    public List<string> titles = new List<string>
    {
        "Knight", "St.", "Captain", "Lord", "Sir", "Baron", "Count", "Duke", "Paladin"
    };

    // Ýsimler: Karma (Hem yerel hem evrensel)
    public List<string> names = new List<string>
    {
        "Ada", "Yusuf", "George", "Ceren", "Fikret", "Burak", "Jean", "North",
        "Arthur", "Cedric", "Elena", "Richard", "William"
    };

    // Soyisimler: Orta Çað'da genellikle lakap, meslek veya yer belirtir
    public List<string> surnames = new List<string>
    {
        "the Brave", "of York", "Ironheart", "Blackwood", "Smith", "Baker",
        "the Swift", "of the North", "Stonefist", "Bridger", "the Lion", "Miller"
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public string GenerateRandomName()
    {
        if (titles.Count == 0 || names.Count == 0 || surnames.Count == 0) return "Adsýz Nefer";

        string t = titles[Random.Range(0, titles.Count)];
        string n = names[Random.Range(0, names.Count)];
        string s = surnames[Random.Range(0, surnames.Count)];

        return $"{t}. {n} {s}";
    }
}