using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MurderousItemClickableCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Item data")]
    public MurderousItemData itemData;

    [Header("Range visualization (Canvas)")]
    // Assign your KillingRange object here in the inspector
    public RectTransform rangeVisual;
    public float rangeRadius = 150f;

    [Header("Kill settings")]
    public bool killOnlyFirst = true;

    private RectTransform _rectTransform;

    [Header("Overheat")]
    public OverheatController overheat;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (rangeVisual != null)
            rangeVisual.gameObject.SetActive(false);

        if (overheat == null)
            overheat = GetComponent<OverheatController>();

        Debug.Log($"[MurderousItemClickableCanvas] Awake on {name}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Just show the pre-placed range visual
        if (rangeVisual != null)
        {
            rangeVisual.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide the range visual when cursor leaves the item
        if (rangeVisual != null)
        {
            rangeVisual.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[MurderousItemClickableCanvas] OnPointerClick on {name}");

        if (itemData == null)
        {
            Debug.LogWarning($"[MurderousItemClickableCanvas] {name}: itemData not assigned.");
            return;
        }

        if (_rectTransform == null)
        {
            Debug.LogError($"[MurderousItemClickableCanvas] {name}: Missing RectTransform.");
            return;
        }

        // Item center in canvas space
        Vector3 itemWorldPos = _rectTransform.TransformPoint(_rectTransform.rect.center);
        Vector2 itemPos2D = new Vector2(itemWorldPos.x, itemWorldPos.y);

        Debug.Log($"[MurderousItemClickableCanvas] Item pos {itemPos2D}, radius={rangeRadius}");

        // Find all NPCs
        Npc[] allNpcs = FindObjectsOfType<Npc>(true);
        if (allNpcs == null || allNpcs.Length == 0)
        {
            Debug.Log("[MurderousItemClickableCanvas] No NPCs found.");
            return;
        }

        bool killedSomeone = false;

        foreach (var npc in allNpcs)
        {
            if (npc == null || !npc.gameObject.activeInHierarchy)
                continue;

            RectTransform npcRect = npc.GetComponent<RectTransform>();
            if (npcRect == null)
            {
                Debug.Log($"NPC {npc.name} has no RectTransform.");
                continue;
            }

            Vector3 npcWorldPos = npcRect.TransformPoint(npcRect.rect.center);
            Vector2 npcPos2D = new Vector2(npcWorldPos.x, npcWorldPos.y);

            float dist = Vector2.Distance(itemPos2D, npcPos2D);

            if (dist <= rangeRadius)
            {
                bool vulnerable = npc.IsVulnerableTo(itemData);

                if (vulnerable)
                {
                    Debug.Log($"[MurderousItemClickableCanvas] Killing NPC {npc.name}");
                    npc.Kill(itemData);
                    killedSomeone = true;
                    
                    SoundManager.Instance.Play(itemData.Sound);

                    if (killOnlyFirst)
                        break;
                }
            }
        }

        // Overheat only on unnecessary click
        if (!killedSomeone && overheat != null)
        {
            Debug.Log("[MurderousItemClickableCanvas] Unnecessary click -> Overheat");
            overheat.RegisterUse();
        }
        else if (killedSomeone)
        {
            Debug.Log("[MurderousItemClickableCanvas] Correct click -> no overheat");
        }
    }

#if UNITY_EDITOR
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
