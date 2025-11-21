using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MurderousItemClickableCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Item data")]
    public MurderousItemData itemData;

    [Header("Range visualization (assign KillingRange here)")]
    public RectTransform rangeVisual;

    [Tooltip("Fallback distance check if rangeVisual is not set.")]
    public float rangeRadius = 150f;

    [Header("Hit tolerance")]
    [Tooltip("Extra slack around NPC in screen pixels when checking against KillingRange.")]
    public float npcHitSlack = 20f;

    [Header("Kill settings")]
    public bool killOnlyFirst = true;

    private RectTransform _rectTransform;

    [Header("Overheat")]
    public OverheatController overheat;

    // Cached renderers for the range visual
    private Graphic _rangeGraphic;
    private SpriteRenderer _rangeSprite;

    private Canvas _canvas;
    private Camera _uiCamera;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (rangeVisual != null)
        {
            _rangeGraphic = rangeVisual.GetComponentInChildren<Graphic>(true);
            _rangeSprite = rangeVisual.GetComponentInChildren<SpriteRenderer>(true);

            HideRange();
        }

        if (overheat == null)
            overheat = GetComponent<OverheatController>();

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas != null)
        {
            _uiCamera = _canvas.worldCamera;
        }

        Debug.Log($"[MurderousItemClickableCanvas] Awake on {name}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowRange();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideRange();
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

        // Center of item in world/canvas space (used only for fallback radius logic)
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

            bool inRange = false;

            // 1) Prefer checking against KillingRange rect, with slack around NPC
            if (rangeVisual != null)
            {
                Vector2 npcScreenCenter = RectTransformUtility.WorldToScreenPoint(_uiCamera, npcWorldPos);

                // Sample a few points around the center (center + 4 directions)
                Vector2[] testPoints = new Vector2[5];
                testPoints[0] = npcScreenCenter;
                testPoints[1] = npcScreenCenter + new Vector2(npcHitSlack, 0f);
                testPoints[2] = npcScreenCenter + new Vector2(-npcHitSlack, 0f);
                testPoints[3] = npcScreenCenter + new Vector2(0f, npcHitSlack);
                testPoints[4] = npcScreenCenter + new Vector2(0f, -npcHitSlack);

                for (int i = 0; i < testPoints.Length; i++)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(rangeVisual, testPoints[i], _uiCamera))
                    {
                        inRange = true;
                        break;
                    }
                }
            }
            else
            {
                // 2) Fallback: simple radius from item
                float dist = Vector2.Distance(itemPos2D, npcPos2D);
                inRange = dist <= rangeRadius;
            }

            if (!inRange)
                continue;

            bool vulnerable = npc.IsVulnerableTo(itemData);

            if (vulnerable)
            {
                Debug.Log($"[MurderousItemClickableCanvas] Killing NPC {npc.name}");
                npc.Kill(itemData);
                killedSomeone = true;

                if (killOnlyFirst)
                    break;
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

    private void ShowRange()
    {
        if (rangeVisual == null)
            return;

        rangeVisual.gameObject.SetActive(true);

        if (_rangeGraphic != null)
        {
            _rangeGraphic.enabled = true;
            var c = _rangeGraphic.color;
            c.a = 1f;
            _rangeGraphic.color = c;
        }

        if (_rangeSprite != null)
        {
            _rangeSprite.enabled = true;
            var c = _rangeSprite.color;
            c.a = 1f;
            _rangeSprite.color = c;
        }
    }

    private void HideRange()
    {
        if (rangeVisual == null)
            return;

        rangeVisual.gameObject.SetActive(false);

        if (_rangeGraphic != null)
            _rangeGraphic.enabled = false;

        if (_rangeSprite != null)
            _rangeSprite.enabled = false;
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
