using UnityEngine;
using System.Collections.Concurrent;
using LevelEditor;
using System;

public class DeploymentManager : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask placementLayer; // Zemin (Ground) layer'ý
    public Collider2D spawnZoneCollider; // Az önce yarattýðýmýz kutu

    [Header("Unit Prefabs")]
    // Developer A'nýn hazýrladýðý gerçek askerler buraya gelecek
    public GameObject infantryPrefab;
    public GameObject archerPrefab;
    public GameObject cavalryPrefab;

    [Header("Visuals")]
    public Transform ghostObject; // Mouse'un ucundaki yarý saydam asker

    private GameObject selectedPrefab; // Þu an hangisini seçtik?

    [Header("Unit Costs")]
    public int infantryCost = 50;
    public int archerCost = 75;
    public int cavalryCost = 120;

    private int currentSelectedCost = 0;    
    private void Start()
    {
        // Baþlangýçta hayalet kapalý
        if (ghostObject) ghostObject.gameObject.SetActive(false);
        SpawnVeterans();
    }

    private void SpawnVeterans()
    {
        if (GameManager.Instance.veterans.Count > 0)
        {
            Debug.Log("Gaziler sahneye yerleþtiriliyor...");

            Vector3 spawnStartPos = spawnZoneCollider.bounds.center;
            spawnStartPos.x -= spawnZoneCollider.bounds.extents.x * 0.8f;
            int index = 0;

            foreach (VeteranData vet in GameManager.Instance.veterans)
            {
                GameObject prefabToSpawn = null;

                switch (vet.type)
                {
                    case EnemyType.Infantry: prefabToSpawn = infantryPrefab; break;
                    case EnemyType.Archer: prefabToSpawn = archerPrefab; break;
                    case EnemyType.Cavalry: prefabToSpawn = cavalryPrefab; break;
                }
                if (prefabToSpawn != null)
                {
                    Vector3 pos = spawnStartPos + new Vector3((index % 5) * 1.5f, (index / 5) * -1.5f, 0);
                    Instantiate(prefabToSpawn, pos, Quaternion.identity);
                }
                index++;
            }
        }
    }

    private void Update()
    {
        // Eðer savaþ baþladýysa artýk yerleþtirme yapamayýz, scripti kapat.
        if (GameManager.Instance.CurrentState != GameState.Battle)
        {
            HandlePlacementInput();
        }
        else
        {
            // Savaþ baþladýysa hayaleti gizle ve çýk
            if (ghostObject) ghostObject.gameObject.SetActive(false);
            this.enabled = false;
        }
    }

    // UI Butonlarý bu fonksiyonu çaðýracak
    public void SelectUnitToPlace(string unitType)
    {
        switch (unitType)
        {
            case "Infantry": 
                selectedPrefab = infantryPrefab;
                currentSelectedCost = infantryCost;
                break;
            case "Archer": 
                selectedPrefab = archerPrefab; 
                currentSelectedCost = archerCost;
                break;
            case "Cavalry": 
                selectedPrefab = cavalryPrefab; 
                currentSelectedCost = cavalryCost;
                break;
        }

        // Hayaleti aç (Ýleride sprite'ýný da seçili askere göre deðiþtirebiliriz)
        if (ghostObject) ghostObject.gameObject.SetActive(true);
        Debug.Log($"Seçildi: {unitType} - Fiyat: {currentSelectedCost}");
    }

    private void HandlePlacementInput()
    {
        if (selectedPrefab == null) return;

        // 1. Mouse Pozisyonunu Bul
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 2. Hayaleti Mouse'a Yapýþtýr (Görsel Yardýmcý)
        if (ghostObject) ghostObject.position = mousePos;

        // 3. Týklama Kontrolü
        if (Input.GetMouseButtonDown(0))
        {
            // Sadece Spawn Zone içindeyse izin ver
            if (spawnZoneCollider.OverlapPoint(mousePos))
            {
                if (GameManager.Instance.SpendMoney(currentSelectedCost))
                {
                    SpawnUnit(mousePos);
                }
                else
                {
                    Debug.Log("Yetersiz Bakiye");
                }
            }
            else
            {
                Debug.Log("Bu alana asker koyamazsýn!");
                // Buraya "Hata Sesi" veya kýrmýzý yanýp sönme efekti ekleyebilirsin.
            }
        }

        // Sað Týk ile seçimi iptal et
        if (Input.GetMouseButtonDown(1))
        {
            selectedPrefab = null;
            if (ghostObject) ghostObject.gameObject.SetActive(false);
        }
    }

    private void SpawnUnit(Vector3 position)
    {
        // Askeri Yarat
        Instantiate(selectedPrefab, position, Quaternion.identity);

        // Ýleride buraya "Para Düþme" veya "Limit Azaltma" kodu ekleyeceðiz.
        // Þimdilik sýnýrsýz koyalým.
    }
}