using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartScreen : MonoBehaviour
{
    [FormerlySerializedAs("button")]
    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Button skipButton;

    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private VideoClip videoToPlay;

    [SerializeField]
    private GameObject screenToHide;

    private CancellationTokenSource _cts = new();

    private void Start()
    {
        playButton.onClick.AddListener(StartGame);
        skipButton.onClick.AddListener(SkipAnimation);
        PlayAndHide();
    }

    private void SkipAnimation()
    {
        _cts.Cancel();
        videoPlayer.Stop();
        screenToHide.SetActive(false);
        skipButton.gameObject.SetActive(false);
    }

    private async void PlayAndHide()
    {
        videoPlayer.source = VideoSource.Url;
        var path = Path.Combine(Application.streamingAssetsPath, videoToPlay.name + ".mp4");
        path = path.Replace("\\", "/");
        videoPlayer.url = path;

        await UniTask.WaitUntil(() => videoPlayer.isPlaying, cancellationToken: _cts.Token);
        await UniTask.WaitUntil(() => !videoPlayer.isPlaying, cancellationToken: _cts.Token);

        screenToHide.SetActive(false);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Market");
    }
}