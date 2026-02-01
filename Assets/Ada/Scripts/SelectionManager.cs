using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{

    public TilemapPainter painter;

    public RectTransform selectionBox;
    private Vector2 startClickPos; // Farenin ilk týklandýðý yer
    public LayerMask unitLayer;
    public LayerMask enemyLayer;

    private Vector2 startPos;
    // Listeyi PlayerUnit tipine çevirdik
    public List<PlayerUnit> selectedUnits = new List<PlayerUnit>();


    void Update()
    {
        if (painter.isPaintingMode && painter != null) return;


        if (Input.GetMouseButtonDown(0))
        {
            startClickPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox();
        }

        // SAÐ TIK KONTROLÜ
        if (Input.GetMouseButtonDown(1)) // 1 sað týk demektir
        {
            HandleRightClick();
        }
    }

    bool TrySingleClick()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // OverlapPoint 2D týklamalar için Raycast'ten daha garantidir
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, unitLayer);

        if (hit != null)
        {
            // Unit yerine PlayerUnit arýyoruz
            PlayerUnit unit = hit.GetComponent<PlayerUnit>();
            if (unit != null)
            {
                ClearSelection();
                selectedUnits.Add(unit);
                // unit.isSelected = true; <-- BUNU SÝL
                unit.SetSelection(true); // BU SATIRI EKLE (Halkayý Mavi yapar)
                return true;
            }
        }
        return false;
    }

    void HandleRightClick()
    {
        // Ölmüþ (null olmuþ) birimleri listeden temizle
        selectedUnits.RemoveAll(unit => unit == null);

        if (selectedUnits.Count == 0) return; // Seçili canlý birim yoksa iþlem yapma

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0, enemyLayer);

        if (hit.collider != null)
        {
            // Düþman bir EnemyUnit olsa da BaseUnit'ten türediði için BaseUnit olarak alabiliriz
            BaseUnit enemy = hit.collider.GetComponent<BaseUnit>();
            if (enemy != null)
            {
                foreach (var unit in selectedUnits) unit.SetTarget(enemy);
            }
        }
        else
        {
            foreach (var unit in selectedUnits)
            {
                unit.MoveTo(mouseWorldPos);
            }
        }
    }

    void UpdateSelectionBox(Vector2 currentMousePos)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        // 1. Farenin pozisyonunu Canvas üzerindeki yerel pozisyona çevir
        RectTransform canvasRect = selectionBox.parent as RectTransform;
        Vector2 localStartPos;
        Vector2 localCurrentPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, startClickPos, null, out localStartPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, currentMousePos, null, out localCurrentPos);

        // 2. Geniþlik ve yükseklik hesapla
        float width = localCurrentPos.x - localStartPos.x;
        float height = localCurrentPos.y - localStartPos.y;

        // 3. Boyutu mutlak deðerle ayarla
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

        // 4. Merkezi hesapla (Pivot 0.5, 0.5 ise)
        selectionBox.anchoredPosition = localStartPos + new Vector2(width / 2, height / 2);
    }

    void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        // 1. Kutunun sýnýrlarýný hesapla
        Vector2 min = startClickPos; // startPos yerine startClickPos kullanýyoruz
        Vector2 max = Input.mousePosition;

        // Eðer kutu çok küçükse (sadece týklanmýþsa sürüklenmemiþse)
        if (Vector2.Distance(min, max) < 5f)
        {
            if (!TrySingleClick())
            {
                ClearSelection(); // Boþluða týklandýysa her þeyi temizle
            }
            return;
        }

        // 2. Yeni seçim yapmadan önce eskileri temizle
        ClearSelection();

        // 3. Ekran koordinatlarýný doðrula
        Vector2 realMin = new Vector2(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y));
        Vector2 realMax = new Vector2(Mathf.Max(min.x, max.x), Mathf.Max(min.y, max.y));

        // 4. Tüm PlayerUnit'leri tara
        PlayerUnit[] allUnits = FindObjectsOfType<PlayerUnit>();
        foreach (PlayerUnit unit in allUnits)
        {
            // Birimin dünya pozisyonunu ekran pozisyonuna çevir
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            // Birim kutunun içinde mi?
            if (screenPos.x > realMin.x && screenPos.x < realMax.x && screenPos.y > realMin.y && screenPos.y < realMax.y)
            {
                selectedUnits.Add(unit);
                // unit.isSelected = true; <-- BUNU SÝL
                unit.SetSelection(true); // BU SATIRI EKLE (Halkayý Mavi yapar)
            }
        }
    }

    void ClearSelection()
    {
        foreach (var unit in selectedUnits)
        {
            if (unit != null) unit.SetSelection(false); // Halkayý Yeþil yapar
        }
        selectedUnits.Clear();
    }
}