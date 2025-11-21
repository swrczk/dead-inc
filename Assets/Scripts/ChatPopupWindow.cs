using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatPopupWindow : MonoBehaviour
{
    [Header("UI References")]
    public Image avatarImage;
    public Text senderNameLabel;
    public Text bodyLabel;
    public Button closeButton;

    private Action onClosed;
    private float hideTime;
    private bool autoHideEnabled;

    public void Show(ChatMessageData data, Action onClosed)
    {
        this.onClosed = onClosed;

        if (avatarImage != null)
            avatarImage.sprite = data.senderAvatar;

        if (senderNameLabel != null)
            senderNameLabel.text = data.senderName;

        if (bodyLabel != null)
            bodyLabel.text = data.text;

        autoHideEnabled = data.autoHideAfterSeconds > 0f;
        hideTime = Time.time + data.autoHideAfterSeconds;

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePopup);
    }

    private void Update()
    {
        if (autoHideEnabled && Time.time >= hideTime)
        {
            ClosePopup();
        }
    }

    public void ClosePopup()
    {
        Destroy(gameObject);
        onClosed?.Invoke();
    }
}
