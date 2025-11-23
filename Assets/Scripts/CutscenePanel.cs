using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class CutscenePanel : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private AudioSource backgroundMusic;

    public async UniTask Play(VideoClip clip)
    {
        backgroundMusic.Pause();
        videoPlayer.source = VideoSource.Url;
        var path = Path.Combine(Application.streamingAssetsPath, clip.name + ".mp4");
        path = path.Replace("\\", "/");
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = path;
        videoPlayer.Play();
        await UniTask.WaitUntil(() => videoPlayer.isPlaying);
        await UniTask.WaitUntil(() => !videoPlayer.isPlaying);
        backgroundMusic.Play();
    }
}