using UnityEngine;

[CreateAssetMenu(menuName = "Chat/Message")]
public class ChatMessageData : ScriptableObject
{
    [TextArea] public string text;
    public Sprite senderAvatar;
    public string senderName = "Manager";
    public float autoHideAfterSeconds = 4f;
}
