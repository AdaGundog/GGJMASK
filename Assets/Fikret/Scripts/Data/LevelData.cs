using System.Collections.Generic;
using UnityEngine;

// Düþman tiplerini string yerine Enum ile tutmak hatayý önler ve performansý artýrýr.
public enum EnemyType
{
    Infantry, // Yaya
    Archer,   // Okçu
    Cavalry   // Atlý
}

[System.Serializable] // Unity'nin bu sýnýfý Inspector'da gösterebilmesi ve JSON'a çevirebilmesi için þart.
public class EnemySpawnData
{
    public EnemyType type; // Düþmanýn tipi
    public Vector2 position; // Sahnedeki konumu (2D)

    // Constructor: Yeni bir veri oluþtururken kolaylýk saðlar
    public EnemySpawnData(EnemyType _type, Vector2 _pos)
    {
        type = _type;
        position = _pos;
    }
}

[System.Serializable]
public class LevelData
{
    public string levelName; // Bölümün adý
    public float startingInkAmount; // Oyuncunun baþlangýçtaki boya miktarý
    public List<EnemySpawnData> enemies; // O bölümdeki tüm düþmanlarýn listesi

    // Constructor listeyi boþ baþlatýr ki "Null Reference" hatasý almayalým.
    public LevelData()
    {
        enemies = new List<EnemySpawnData>();
    }
}