using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class CutscenePanel : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    public async UniTask Play(VideoClip clip)
    {
        videoPlayer.clip = clip;
        videoPlayer.Play();
        await UniTask.WaitUntil(() => videoPlayer.isPlaying);
        await UniTask.WaitUntil(() => !videoPlayer.isPlaying);
    }
}