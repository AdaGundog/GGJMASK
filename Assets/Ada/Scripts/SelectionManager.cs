using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public RectTransform selectionBox;
    public LayerMask unitLayer;
    public LayerMask enemyLayer;

    private Vector2 startPos;
    // Listeyi PlayerUnit tipine çevirdik
    public List<PlayerUnit> selectedUnits = new List<PlayerUnit>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            if (!TrySingleClick())
            {
                ClearSelection();
                selectionBox.gameObject.SetActive(true);
            }
        }

        if (Input.GetMouseButton(0) && selectionBox.gameObject.activeSelf)
        {
            UpdateSelectionBox(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (selectionBox.gameObject.activeSelf)
            {
                ReleaseSelectionBox();
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
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
                unit.isSelected = true;
                return true;
            }
        }
        return false;
    }

    void HandleRightClick()
    {
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

    void UpdateSelectionBox(Vector2 curMousePos)
    {
        if (!selectionBox.gameObject.activeSelf)
            selectionBox.gameObject.SetActive(true);

        float width = curMousePos.x - startPos.x;
        float height = curMousePos.y - startPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.position = startPos + new Vector2(width / 2, height / 2);
    }

    void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = startPos;
        Vector2 max = Input.mousePosition;
        Vector2 realMin = new Vector2(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y));
        Vector2 realMax = new Vector2(Mathf.Max(min.x, max.x), Mathf.Max(min.y, max.y));

        // Sahnedeki PlayerUnit'leri buluyoruz
        PlayerUnit[] allUnits = FindObjectsOfType<PlayerUnit>();
        foreach (PlayerUnit unit in allUnits)
        {
            if (((1 << unit.gameObject.layer) & unitLayer) != 0)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
                if (screenPos.x > realMin.x && screenPos.x < realMax.x && screenPos.y > realMin.y && screenPos.y < realMax.y)
                {
                    selectedUnits.Add(unit);
                    unit.isSelected = true;
                }
            }
        }
    }

    void ClearSelection()
    {
        foreach (var unit in selectedUnits) unit.isSelected = false;
        selectedUnits.Clear();
    }
}