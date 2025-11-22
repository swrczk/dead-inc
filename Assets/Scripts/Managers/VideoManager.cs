using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<VideoManager>();
                _instance ??= new GameObject("VideoManager").AddComponent<VideoManager>();
            }

            return _instance;
        }
    }

    private static VideoManager _instance;
    private CutscenePanel _cutscenePanel; 

    private void Start()
    {
        _cutscenePanel = FindObjectOfType<CutscenePanel>(); 
        _cutscenePanel.gameObject.SetActive(false);
    }

    public async void Play(VideoClip clip)
    {
        
        _cutscenePanel.gameObject.SetActive(true);
        await _cutscenePanel.Play(clip);
        _cutscenePanel.gameObject.SetActive(false);
    }
}