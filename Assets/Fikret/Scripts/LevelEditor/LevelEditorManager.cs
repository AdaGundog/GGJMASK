using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelEditor
{
    public class LevelEditorManager : MonoBehaviour
    {
        [Header("Editor Tools")]
        public bool isEditMode = false;
        public EnemyType currentSelection = EnemyType.Infantry;

        [Header("Level Settings")]
        public string levelFileName = "Level_1";
        public float initialInk = 100f;
        public int levelBonus;

        [Header("References")]
        public GameObject archerPrefab;
        public GameObject infantryPrefab;
        public GameObject cavalryPrefab;
        public Transform enemiesParent;

        [HideInInspector]
        public LevelData currentLevelData = new LevelData();

        public void AddEnemyFromEditor(Vector3 position)
        {
#if UNITY_EDITOR
            position.z = 0;
            GameObject prefabToUse = null;

            switch (currentSelection)
            {
                case EnemyType.Infantry: prefabToUse = infantryPrefab; break;
                case EnemyType.Cavalry: prefabToUse = cavalryPrefab; break;
                case EnemyType.Archer: prefabToUse = archerPrefab; break;
            }

            if (prefabToUse == null) return;

            GameObject newEnemy = (GameObject)PrefabUtility.InstantiatePrefab(prefabToUse);
            newEnemy.transform.position = position;
            newEnemy.transform.parent = enemiesParent;
            newEnemy.name = $"Enemy_{currentSelection}_{currentLevelData.enemies.Count}";

            Undo.RegisterCreatedObjectUndo(newEnemy, "Create Enemy");

            EnemySpawnData data = new EnemySpawnData(currentSelection, position);
            currentLevelData.enemies.Add(data);

            EditorUtility.SetDirty(this);
#endif
        }

        // --- DEÐÝÞÝKLÝK BURADA ---
        private string GetSavePath()
        {
            // Kayýt yolunu StreamingAssets/Levels yaptýk
            string path = Path.Combine(Application.streamingAssetsPath, "Levels");

            // Eðer klasör yoksa oluþtur (Editörde ilk seferde lazým olabilir)
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return path;
        }
        // -------------------------

        public void SaveLevel()
        {
            currentLevelData.enemies.Clear();

            if (enemiesParent != null)
            {
                foreach (Transform child in enemiesParent)
                {
                    EnemyType type = EnemyType.Infantry;
                    if (child.name.Contains("Archer")) type = EnemyType.Archer;
                    else if (child.name.Contains("Cavalry")) type = EnemyType.Cavalry;
                    else if (child.name.Contains("Infantry")) type = EnemyType.Infantry;

                    currentLevelData.enemies.Add(new EnemySpawnData(type, child.position));
                }
            }

            currentLevelData.levelName = levelFileName;
            currentLevelData.startingInkAmount = initialInk;
            currentLevelData.levelStartBonus = levelBonus;

            string json = JsonUtility.ToJson(currentLevelData, true);
            string fullPath = Path.Combine(GetSavePath(), levelFileName + ".json");
            File.WriteAllText(fullPath, json);

            Debug.Log($"Bölüm Kaydedildi: {fullPath}");

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public void LoadLevel()
        {
            // Yolu GetSavePath() üzerinden alýyor, yani StreamingAssets'ten okuyacak
            string fullPath = Path.Combine(GetSavePath(), levelFileName + ".json");

            if (File.Exists(fullPath))
            {
                // 1. Sahneyi temizle
                if (enemiesParent != null)
                {
                    for (int i = enemiesParent.childCount - 1; i >= 0; i--)
                    {
                        if (Application.isPlaying)
                            Destroy(enemiesParent.GetChild(i).gameObject);
                        else
                            DestroyImmediate(enemiesParent.GetChild(i).gameObject);
                    }
                }

                // 2. JSON oku
                string json = File.ReadAllText(fullPath);
                currentLevelData = JsonUtility.FromJson<LevelData>(json);

                // 3. Düþmanlarý Yarat
                foreach (var enemyData in currentLevelData.enemies)
                {
                    GameObject prefab = null;
                    if (enemyData.type == EnemyType.Archer) prefab = archerPrefab;
                    else if (enemyData.type == EnemyType.Cavalry) prefab = cavalryPrefab;
                    else prefab = infantryPrefab;

                    if (prefab != null)
                    {
                        GameObject newEnemy;
#if UNITY_EDITOR
                        if (!Application.isPlaying)
                            newEnemy = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
                        else
                            newEnemy = Instantiate(prefab);
#else
                        newEnemy = Instantiate(prefab);
#endif
                        newEnemy.transform.position = enemyData.position;
                        newEnemy.transform.parent = enemiesParent;
                    }
                }
                Debug.Log("Bölüm StreamingAssets üzerinden yüklendi!");
            }
            else
            {
                Debug.LogError($"Bölüm dosyasý bulunamadý: {fullPath}");
            }
        }

        private void OnDrawGizmos()
        {
            if (!isEditMode) return;
            // Gizmos kodlarýn ayný kalabilir...
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            for (int x = -15; x <= 15; x++) Gizmos.DrawLine(new Vector3(x, -10, 0), new Vector3(x, 10, 0));
            for (int y = -10; y <= 10; y++) Gizmos.DrawLine(new Vector3(-15, y, 0), new Vector3(15, y, 0));

            if (currentLevelData != null && currentLevelData.enemies != null)
            {
                foreach (var enemy in currentLevelData.enemies)
                {
                    switch (enemy.type)
                    {
                        case EnemyType.Infantry: Gizmos.color = Color.red; break;
                        case EnemyType.Archer: Gizmos.color = Color.green; break;
                        case EnemyType.Cavalry: Gizmos.color = Color.blue; break;
                    }
                    Gizmos.DrawWireCube(enemy.position, Vector3.one);
                }
            }
        }
    }
}