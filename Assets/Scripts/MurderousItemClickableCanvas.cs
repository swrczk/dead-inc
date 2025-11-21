using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MurderousItemClickableCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Dane przedmiotu")]
    [Tooltip("Typ ataku / obra?e?, jaki zadaje ten przedmiot.")]
    public MurderousItemData itemData;

    [Header("Zasi?g dzia?ania (Canvas)")]
    [Tooltip("Wizualizacja zasi?gu (okr?g jako child na Canvasie).")]
    public RectTransform rangeVisual;

    [Tooltip("Zasi?g w jednostkach Canvas (piksele w przybli?eniu).")]
    public float rangeRadius = 150f;

    [Tooltip("Czy zabi? tylko pierwszego trafionego NPC.")]
    public bool killOnlyFirst = true;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform == null)
        {
            Debug.LogError($"[MurderousItemClickableCanvas] {name}: Brak RectTransform! Ten skrypt dzia?a tylko na UI (Canvas).");
        }

        if (rangeVisual != null)
        {
            rangeVisual.gameObject.SetActive(false);
        }

        Debug.Log($"[MurderousItemClickableCanvas] Awake na {name}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rangeVisual != null)
        {
            rangeVisual.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (rangeVisual != null)
        {
            rangeVisual.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[MurderousItemClickableCanvas] OnPointerClick na {name}");

        if (itemData == null)
        {
            Debug.LogWarning($"[MurderousItemClickableCanvas] {name}: itemData NIE jest ustawione.");
            return;
        }

        if (_rectTransform == null)
        {
            Debug.LogError($"[MurderousItemClickableCanvas] {name}: Brak RectTransform ? nie mog? policzy? zasi?gu.");
            return;
        }

        // ?rodek przedmiotu na Canvasie (w przestrzeni ?wiata Canvasu)
        Vector3 itemWorldPos = _rectTransform.TransformPoint(_rectTransform.rect.center);
        Vector2 itemPos2D = new Vector2(itemWorldPos.x, itemWorldPos.y);

        Debug.Log($"[MurderousItemClickableCanvas] Pozycja itemu: {itemPos2D}, radius={rangeRadius}");

        // Szukamy wszystkich NPC ze skryptem Npc (na Canvasie)
        Npc[] allNpcs = FindObjectsOfType<Npc>(true);
        if (allNpcs == null || allNpcs.Length == 0)
        {
            Debug.Log("[MurderousItemClickableCanvas] Brak obiekt?w z komponentem Npc w scenie.");
            return;
        }

        bool killedSomeone = false;

        foreach (var npc in allNpcs)
        {
            if (npc == null)
                continue;

            if (!npc.gameObject.activeInHierarchy)
            {
                Debug.Log($"[MurderousItemClickableCanvas] NPC {npc.name} jest nieaktywny ? pomijam.");
                continue;
            }

            RectTransform npcRect = npc.GetComponent<RectTransform>();
            if (npcRect == null)
            {
                Debug.Log($"[MurderousItemClickableCanvas] NPC {npc.name} nie ma RectTransform (nie jest na Canvasie?) ? pomijam.");
                continue;
            }

            Vector3 npcWorldPos = npcRect.TransformPoint(npcRect.rect.center);
            Vector2 npcPos2D = new Vector2(npcWorldPos.x, npcWorldPos.y);

            float dist = Vector2.Distance(itemPos2D, npcPos2D);

            Debug.Log($"[MurderousItemClickableCanvas] Sprawdzam NPC {npc.name}: dist={dist}, range={rangeRadius}");

            if (dist <= rangeRadius)
            {
                bool vulnerable = npc.IsVulnerableTo(itemData);
                Debug.Log($"[MurderousItemClickableCanvas] NPC {npc.name} jest w zasi?gu, vulnerable={vulnerable}");

                if (vulnerable)
                {
                    Debug.Log($"[MurderousItemClickableCanvas] Zabijam NPC {npc.name} przy u?yciu {itemData.name}");
                    npc.Kill(itemData);
                    killedSomeone = true;

                    SoundManager.Instance.Play(itemData.Sound);

                    if (killOnlyFirst)
                        break;
                }
            }
        }

        if (!killedSomeone)
        {
            Debug.Log("[MurderousItemClickableCanvas] W zasi?gu nie by?o ?adnego podatnego NPC.");
        }
    }

#if UNITY_EDITOR
    // Dla wygody ? mo?na podgl?dn?? promie? w Scene view, ale tylko orientacyjnie.
    private void OnDrawGizmosSelected()
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform == null)
            return;

        Vector3 itemWorldPos = _rectTransform.TransformPoint(_rectTransform.rect.center);
        Vector3 p1 = itemWorldPos + Vector3.right * rangeRadius;
        Vector3 p2 = itemWorldPos + Vector3.left * rangeRadius;
        Vector3 p3 = itemWorldPos + Vector3.up * rangeRadius;
        Vector3 p4 = itemWorldPos + Vector3.down * rangeRadius;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p3, p4);
    }
#endif
}
