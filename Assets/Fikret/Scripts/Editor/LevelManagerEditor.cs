using UnityEngine;
using UnityEditor;
using LevelEditor;

[CustomEditor(typeof(LevelEditorManager))]
public class LevelManagerEditor : Editor
{
    LevelEditorManager targetScript;

    private void OnEnable()
    {
        targetScript = (LevelEditorManager)target;
    }

    private void OnSceneGUI()
    {
        // Edit Mode kapalıysa hiçbir şey yapma
        if (!targetScript.isEditMode) return;

        // --- 1. GÖRSEL İPUÇLARI (HUD) ---
        Handles.BeginGUI();
        Vector2 mousePos = Event.current.mousePosition;

        // Kullanım talimatı kutusu
        string info = $"Seçili: {targetScript.currentSelection}\n[Sol Tık] Ekle\n[Shift+Sol Tık] Sil";

        // Yazının arkasına gri bir kutu koyalım ki okunsun
        GUI.Box(new Rect(mousePos.x + 20, mousePos.y + 20, 160, 55), GUIContent.none);
        GUI.Label(new Rect(mousePos.x + 25, mousePos.y + 25, 150, 50), info, EditorStyles.boldLabel);
        Handles.EndGUI();
        // -----------------------

        Event e = Event.current;

        // Sahne kontrolünü ele al (Tıkladığımızda arkadaki objeleri seçmesin diye)
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        // --- 2. TIKLAMA KONTROLÜ ---
        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
        {
            if (e.button == 0) // Sol Tık
            {
                // SHIFT basılıysa -> SİLME MODU
                if (e.shift)
                {
                    DeleteObjectUnderMouse(e);
                }
                // SHIFT yok ve ALT yok (Kamera hareketi değilse) -> EKLEME MODU
                else if (!e.alt)
                {
                    // Sadece tek tıklamada ekle (Sürüklerken spam yapmasın)
                    if (e.type == EventType.MouseDown)
                    {
                        AddObjectUnderMouse(e);
                    }
                }
            }
        }
    }

    // --- YARDIMCI FONKSİYON: EKLEME ---
    private void AddObjectUnderMouse(Event e)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane zPlane = new Plane(Vector3.forward, Vector3.zero);

        if (zPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            // GRID SNAPPING (HİZALAMA)
            // Koordinatları yuvarlayarak tam karelere (1.0, 2.0 gibi) oturmasını sağlar.
            hitPoint.x = Mathf.Round(hitPoint.x);
            hitPoint.y = Mathf.Round(hitPoint.y);

            targetScript.AddEnemyFromEditor(hitPoint);
            e.Use(); // Olayı kullandığımızı Unity'ye bildir
        }
    }

    // --- YARDIMCI FONKSİYON: SİLME ---
    private void DeleteObjectUnderMouse(Event e)
    {
        // Silme işlemi için Raycast farklıdır. Bir "Collider"a çarpmamız lazım.
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        // Sahnede bir 2D Collider'a çarptı mı?
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            // Çarptığı obje bizim düşmanların olduğu kapsayıcının (Parent) çocuğu mu?
            // (Yanlışlıkla zemini veya başka şeyi silmeyelim)
            if (hit.transform.parent == targetScript.enemiesParent)
            {
                // Undo sistemiyle sil (Ctrl+Z çalışsın)
                Undo.DestroyObjectImmediate(hit.transform.gameObject);

                // Unity'ye "Bir şey sildim, kaydetmeyi unutma" de
                EditorUtility.SetDirty(targetScript);

                e.Use();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        // --- BAŞLIK ---
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 15;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("⚔️ LEVEL DESIGN EDITOR ⚔️", titleStyle);
        GUILayout.Space(10);

        // --- STANDART DEĞİŞKENLER ---
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Birim Seçimi:", EditorStyles.boldLabel);

        // --- SEÇİM ÇUBUĞU (TOOLBAR) ---
        targetScript.currentSelection = (EnemyType)GUILayout.Toolbar((int)targetScript.currentSelection, new string[] { "🛡️ Yaya", "🏹 Okçu", "🐎 Atlı" });

        GUILayout.Space(20);

        // --- KAYIT BUTONLARI ---
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SAVE LEVEL", GUILayout.Height(30))) targetScript.SaveLevel();

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("LOAD LEVEL", GUILayout.Height(30))) targetScript.LoadLevel();

        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;

        // --- BİLGİLENDİRME ---
        if (targetScript.isEditMode)
        {
            EditorGUILayout.HelpBox("EDIT MODE AÇIK:\n- Ekleme: Sol Tık\n- Silme: Shift + Sol Tık\n- Geri Al: Ctrl + Z", MessageType.Info);
        }
    }
}
