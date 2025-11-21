using System.Collections.Generic;
using UnityEngine;

public class ChatPopupManager : MonoBehaviour
{
    public static ChatPopupManager Instance { get; private set; }

    [Header("Popup Settings")]
    public ChatPopupWindow popupPrefab;
    public Transform popupParent;

    [Tooltip("Seconds of cooldown for the same message to avoid spam.")]
    public float messageCooldown = 3f;

    private Dictionary<ChatMessageData, float> lastMessageTime = new Dictionary<ChatMessageData, float>();
    private Queue<ChatMessageData> messageQueue = new Queue<ChatMessageData>();

    private bool isDisplaying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowMessage(ChatMessageData data)
    {
        if (data == null)
            return;

        float now = Time.time;

        if (lastMessageTime.ContainsKey(data))
        {
            if (now - lastMessageTime[data] < messageCooldown)
                return; // avoid spam
        }

        lastMessageTime[data] = now;
        messageQueue.Enqueue(data);

        if (!isDisplaying)
            DisplayNextMessage();
    }

    private void DisplayNextMessage()
    {
        if (messageQueue.Count == 0)
        {
            isDisplaying = false;
            return;
        }

        isDisplaying = true;

        var data = messageQueue.Dequeue();
        var popup = Instantiate(popupPrefab, popupParent);

        popup.Show(data, onClosed: DisplayNextMessage);
    }
}
