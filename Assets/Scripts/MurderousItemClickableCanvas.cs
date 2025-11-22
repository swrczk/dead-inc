using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;

public class MurderousItemClickableCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Sprite Icon => itemImage.sprite;
    [Header("Item data")]
    public MurderousItemData itemData;
    
    [SerializeField]
    private Image itemImage;

    [Header("Range visualization (assign KillingRange here)")]
    public RectTransform rangeVisual;

    [Tooltip("Fallback distance check if rangeVisual is not set.")]
    public float rangeRadius = 150f;

    [Header("Hit tolerance")]
    public float npcHitSlack = 20f;

    [Header("Kill settings")]
    public bool killOnlyFirst = true;

    private RectTransform _rectTransform;

    [Header("Overheat")]
    public OverheatController overheat;

    [Header("Guide feedback")]
    public ChatMessageData guideMessage;
    public Animator guideIconAnimator;
    public AnimationSequencerController controller;

    private bool guideMessageShown = false;

    private Graphic _rangeGraphic;
    private SpriteRenderer _rangeSprite;

    private Canvas _canvas;
    private Camera _uiCamera;

    private void Awake()
    {
        itemData.WasPlayed = false;
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
            _uiCamera = _canvas.worldCamera;

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
            return;

        if (_rectTransform == null)
            return;

        Vector3 itemWorldPos = _rectTransform.TransformPoint(_rectTransform.rect.center);
        Vector2 itemPos2D = new Vector2(itemWorldPos.x, itemWorldPos.y);

        Npc[] allNpcs = FindObjectsOfType<Npc>(true);
        if (allNpcs == null || allNpcs.Length == 0)
            return;

        bool killedSomeone = false;
        bool wrongItemUsedOnNpc = false;

        foreach (var npc in allNpcs)
        {
            if (npc == null || !npc.gameObject.activeInHierarchy)
                continue;

            RectTransform npcRect = npc.GetComponent<RectTransform>();
            if (npcRect == null)
                continue;

            Vector3 npcWorldPos = npcRect.TransformPoint(npcRect.rect.center);

            bool inRange = false;

            // Range check using KillingRange + slack
            if (rangeVisual != null)
            {
                Vector2 screenCenter = RectTransformUtility.WorldToScreenPoint(_uiCamera, npcWorldPos);

                Vector2[] testPoints = new Vector2[]
                {
                    screenCenter,
                    screenCenter + new Vector2(npcHitSlack, 0),
                    screenCenter + new Vector2(-npcHitSlack, 0),
                    screenCenter + new Vector2(0, npcHitSlack),
                    screenCenter + new Vector2(0, -npcHitSlack),
                };

                foreach (var p in testPoints)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(rangeVisual, p, _uiCamera))
                    {
                        inRange = true;
                        break;
                    }
                }
            }
            else
            {
                // fallback to simple radius
                float dist = Vector2.Distance(
                    itemPos2D,
                    new Vector2(npcWorldPos.x, npcWorldPos.y)
                );
                inRange = dist <= rangeRadius;
            }

            if (!inRange)
                continue;

            // Vulnerability check
            if (npc.IsVulnerableTo(itemData))
            {
                npc.Kill(itemData);
                SoundManager.Instance.Play(itemData.Sound);

                killedSomeone = true;

                if (killOnlyFirst)
                    break;
            }
            else
            {
                // NPC is in range but not vulnerable to this item
                wrongItemUsedOnNpc = true;
            }
        }

        // Show guide message and highlight cheat sheet icon only once,
        // when player tried to use wrong item on an NPC in range.
        if (wrongItemUsedOnNpc && !guideMessageShown)
        {
            guideMessageShown = true;

            if (guideMessage != null && ChatPopupManager.Instance != null)
            {
                ChatPopupManager.Instance.ShowMessage(guideMessage);
                controller.Play();
            }

        }

        // Overheat rule: only when click did nothing useful
        if (!killedSomeone && overheat != null)
        {
            overheat.RegisterUse();
        }
    }

    private void ShowRange()
    {
        if (rangeVisual == null) return;

        rangeVisual.gameObject.SetActive(true);

        if (_rangeGraphic != null)
            _rangeGraphic.enabled = true;

        if (_rangeSprite != null)
            _rangeSprite.enabled = true;
    }

    private void HideRange()
    {
        if (rangeVisual == null) return;

        rangeVisual.gameObject.SetActive(false);

        if (_rangeGraphic != null)
            _rangeGraphic.enabled = false;

        if (_rangeSprite != null)
            _rangeSprite.enabled = false;
    }
}
