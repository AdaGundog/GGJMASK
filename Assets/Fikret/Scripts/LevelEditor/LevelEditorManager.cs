using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class LevelEditorManager : MonoBehaviour
    {
        [Header("Editor Tools")]
        public bool isEditMode = false; // Bunu açýnca çizim yapacaðýz

        public EnemyType currentSelection = EnemyType.Infantry;

        [Header("Level Settings")]
        public string levelFileName = "Level_1";
        public float initialInk = 100f;

        [Header("References")]
        public GameObject archerPrefab;
        public GameObject infantryPrefab;
        public GameObject cavalryPrefab;
        public Transform enemiesParent;

        // Düþman verilerini tutan liste
        [HideInInspector] // Inspector'da kalabalýk etmesin, zaten çizerek görüyoruz
        public LevelData currentLevelData = new LevelData();

        // --- EDÝTÖRÜN ÇAÐIRACAÐI FONKSÝYON ---
        public void AddEnemyFromEditor(Vector3 position)
        {

            position.z = 0; // 2D düzeltmesi
            GameObject prefabToUse = null;

            switch (currentSelection)
            {
                case EnemyType.Infantry: prefabToUse = infantryPrefab; break;
                case EnemyType.Cavalry: prefabToUse = cavalryPrefab; break;
                case EnemyType.Archer: prefabToUse = archerPrefab; break;
            }

            if (prefabToUse == null)
            {
                Debug.LogError($"Hata: {currentSelection} için Prefab atanmamýþ!");
                return;
            }
            // 1. Görsel Obje Oluþtur
            // PrefabUtility.InstantiatePrefab kullanýmý, objenin "Prefab baðlantýsýný" korur.
            GameObject newEnemy = (GameObject)PrefabUtility.InstantiatePrefab(prefabToUse);
            newEnemy.transform.position = position;
            newEnemy.transform.parent = enemiesParent;
            newEnemy.name = $"Enemy_{currentSelection}_{currentLevelData.enemies.Count}";

            // UNDO (Geri Alma) Ýþlemi Kaydý
            // Buna Ctrl+Z dendiðinde objeyi silmesini saðlar.
            Undo.RegisterCreatedObjectUndo(newEnemy, "Create Enemy");

            // 2. Veriye Ekle
            EnemySpawnData data = new EnemySpawnData(currentSelection, position);
            currentLevelData.enemies.Add(data);

            // Sahne dosyasýný "Kirli" (Deðiþtirilmiþ) olarak iþaretle ki Unity "Kaydetmek istiyor musun?" diye sorsun.
            EditorUtility.SetDirty(this);

            Debug.Log($"Düþman Eklendi: {position}");
        }

        // --- KAYIT SÝSTEMÝ (Deðiþmedi) ---
        private string GetSavePath()
        {
            string path = Path.Combine(Application.dataPath, "Fikret/Levels");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }

        public void SaveLevel()
        {
            // 1. Önce eski listeyi tamamen temizle (Çünkü içinde silinenler kalmýþ olabilir)
            currentLevelData.enemies.Clear();

            // 2. Sahnedeki "EnemiesContainer" içindeki tüm yaþayan objeleri tek tek gez
            // (Destroy edilenler zaten hiyerarþiden gitmiþ oluyor)
            if (enemiesParent != null)
            {
                foreach (Transform child in enemiesParent)
                {
                    // Objenin isminden veya tag'inden tipini anlayabiliriz.
                    // Ama þimdilik basit bir yöntem kullanalým: Objenin adýna bakarak tipini bulalým.
                    // (Prefab isimlerini "Enemy_Archer_..." yapmýþtýk hatýrlarsan)

                    EnemyType type = EnemyType.Infantry; // Varsayýlan

                    if (child.name.Contains("Archer")) type = EnemyType.Archer;
                    else if (child.name.Contains("Cavalry")) type = EnemyType.Cavalry;
                    else if (child.name.Contains("Infantry")) type = EnemyType.Infantry;

                    // 3. Bu yaþayan objeyi taze listeye ekle
                    currentLevelData.enemies.Add(new EnemySpawnData(type, child.position));
                }
            }

            // 4. Diðer verileri güncelle
            currentLevelData.levelName = levelFileName;
            currentLevelData.startingInkAmount = initialInk;

            // 5. Kaydet
            string json = JsonUtility.ToJson(currentLevelData, true);
            string fullPath = Path.Combine(GetSavePath(), levelFileName + ".json");
            File.WriteAllText(fullPath, json);

            Debug.Log($"<color=green>Bölüm Kaydedildi (Güncel):</color> {fullPath}");

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public void LoadLevel()
        {
            string fullPath = Path.Combine(GetSavePath(), levelFileName + ".json");
            if (File.Exists(fullPath))
            {
                // Önce sahneyi temizle (Eski düþmanlarý sil)
                if (enemiesParent != null)
                {
                    // Editör modunda DestroyImmediate kullanýlýr
                    for (int i = enemiesParent.childCount - 1; i >= 0; i--)
                    {
                        DestroyImmediate(enemiesParent.GetChild(i).gameObject);
                    }
                }

                string json = File.ReadAllText(fullPath);
                currentLevelData = JsonUtility.FromJson<LevelData>(json);

                // Düþmanlarý tekrar yarat
                foreach (var enemyData in currentLevelData.enemies)
                {
                    GameObject newEnemy = (GameObject)PrefabUtility.InstantiatePrefab(archerPrefab);
                    newEnemy.transform.position = enemyData.position;
                    newEnemy.transform.parent = enemiesParent;
                }
                Debug.Log("Bölüm Yüklendi!");
            }
        }
        // --- GÖRSEL YARDIMCILAR (GIZMOS) ---
        private void OnDrawGizmos()
        {
            // Sadece Edit Mode açýksa çizelim
            if (!isEditMode) return;

            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f); // Yarý saydam gri

            // Örnek: -10 ile +10 arasý bir alan çizelim (Oyun alanýna göre deðiþtirebilirsin)
            // Dikey Çizgiler
            for (int x = -15; x <= 15; x++)
            {
                Gizmos.DrawLine(new Vector3(x, -10, 0), new Vector3(x, 10, 0));
            }

            // Yatay Çizgiler
            for (int y = -10; y <= 10; y++)
            {
                Gizmos.DrawLine(new Vector3(-15, y, 0), new Vector3(15, y, 0));
            }

            // Mevcut Düþmanlarýn Altýna Ýþaret Koy (Daha belirgin olsun diye)
            if (currentLevelData != null && currentLevelData.enemies != null)
            {
                foreach (var enemy in currentLevelData.enemies)
                {
                    // Asker tipine göre renk verelim
                    switch (enemy.type)
                    {
                        case EnemyType.Infantry: Gizmos.color = Color.red; break;
                        case EnemyType.Archer: Gizmos.color = Color.green; break;
                        case EnemyType.Cavalry: Gizmos.color = Color.blue; break;
                    }
                    // Askerin olduðu yere tel kafes çiz
                    Gizmos.DrawWireCube(enemy.position, Vector3.one);
                }
            }
        }
    }
}